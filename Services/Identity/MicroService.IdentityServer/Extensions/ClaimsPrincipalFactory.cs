using AspNetCore.Identity.MongoDbCore.Models;
using MicroService.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MicroService.IdentityServer.Extensions
{
    public sealed class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, MongoIdentityRole>
    {
        public ClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<MongoIdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {

        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            return principal;
        }

        protected override Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            return base.GenerateClaimsAsync(user);
        }
    }
}
