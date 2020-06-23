using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using IdentityServer4.Models;
using MicroService.IdentityServer.Data.MongoDb;
using MicroService.IdentityServer.Data.MongoDb.IdentityServer4Extensions;
using MicroService.IdentityServer.Data.MongoDb.Infrastructure;
using MicroService.IdentityServer.Extensions;
using MicroService.IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;
using MongoDbGenericRepository;

namespace MicroService.IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddControllersWithViews();

            services.AddHttpContextAccessor();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipalFactory>();

            #region Identity Server Config

            var settings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            var mongoDbContext = new MongoDbContext(settings.ConnectionString, settings.DatabaseName);

            services.AddIdentity<ApplicationUser, MongoIdentityRole>()
                    .AddMongoDbStores<ApplicationUser, MongoIdentityRole, Guid>(mongoDbContext)
                    .AddDefaultTokenProviders();

            services.Configure<MongoSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoDbSettings:ConnectionString").Value;
                options.DatabaseName = Configuration.GetSection("MongoDbSettings:DatabaseName").Value;
            });


            services.AddIdentityServer(options => { options.Events.RaiseSuccessEvents = true; })
                    .AddDeveloperSigningCredential()
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddMongoRepository()
                    .AddClients()
                    .AddIdentityApiResources();

            #endregion  
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseCors(corsPolicyBuilder => corsPolicyBuilder
                   .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            // Add this before any other middleware that might write cookies
            app.UseCookiePolicy();


            app.UseIdentityServer();
            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            #region DATABASE CONFIGURATION

            ConfigureMongoDriver2IgnoreExtraElements();
            InitializeDatabase(app);

            #endregion
        }

        #region Database

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            bool createdNewRepository = false;

            var repository = app.ApplicationServices.GetService<Data.MongoDb.Infrastructure.IMongoRepository>();

            if (!repository.CollectionExists<Client>())
            {
                foreach (var client in Config.GetClients())
                {
                    repository.Add<Client>(client);
                }
                createdNewRepository = true;
            }

            if (!repository.CollectionExists<IdentityResource>())
            {
                foreach (var res in Config.GetIdentityResources())
                {
                    repository.Add<IdentityResource>(res);
                }
                createdNewRepository = true;
            }

            if (!repository.CollectionExists<ApiResource>())
            {
                foreach (var api in Config.GetApiResources())
                {
                    repository.Add<ApiResource>(api);
                }
                createdNewRepository = true;
            }

            if (createdNewRepository)
            {
                var newRepositoryMsg = $"Mongo Repository created/populated! Please restart you website, so Mongo driver will be configured  to ignore Extra Elements.";
                throw new Exception(newRepositoryMsg);
            }
        }

        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
        #endregion
    }
}
