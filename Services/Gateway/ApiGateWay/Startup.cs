using System;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotApiGateWay
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var identityBuilder = services.AddAuthentication();

            #region IDENTITY SERVER CONFIGURATION (CAR SERVICE)

            var authenticationCarKey = "CarKey";

            Action<IdentityServerAuthenticationOptions> carOptions = o =>
            {
                o.Authority = "http://localhost:5555";
                o.RequireHttpsMetadata = false;
                o.ApiName = "api1";
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "secret";
            };

            identityBuilder.AddIdentityServerAuthentication(authenticationCarKey, carOptions);

            #endregion

            #region IDENTITY SERVER CONFIGURATION (SUPPLIER SERVICE)

            var authenticationSupplierKey = "SupplierKey";

            Action<IdentityServerAuthenticationOptions> supplierOptions = o =>
            {
                o.Authority = "http://localhost:5555";
                o.RequireHttpsMetadata = false;
                o.ApiName = "api2";
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "secret";
            };

            identityBuilder.AddIdentityServerAuthentication(authenticationSupplierKey, supplierOptions);

            #endregion

            services.AddOcelot();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseOcelot().Wait();
        }
    }
}
