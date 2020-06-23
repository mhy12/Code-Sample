using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.IdentityServer.Data.MongoDb
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API #1"),
                new ApiResource("api2", "My API #2")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            var clientList = new List<Client>();

            clientList.Add(new Client
            {
                RequireConsent = false,
                ClientId = "angular_spa",
                ClientName = "Angular 4 Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                AllowedScopes = new List<string> { "openid", "profile", "api1" },
                RedirectUris = new List<string> { "http://localhost:4200/auth-callback", "http://localhost:4200/silent-refresh.html" },
                PostLogoutRedirectUris = new List<string> { "http://localhost:4200/" },
                AllowedCorsOrigins = new List<string> { "http://localhost:4200" },
                AllowAccessTokensViaBrowser = true
            });

            clientList.Add(new Client
            {
                ClientId = "Supplier",

                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets = { new Secret("Supplier".Sha256()) },

                AllowedScopes = { "api2" }
            });

            return clientList;
        }
    }
}
