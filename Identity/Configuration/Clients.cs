using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace Identity.Configuration
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    ClientName = "API",
                    ClientId = "00000000-0000-0000-0000-000000000001",
                    Flow = Flows.ClientCredentials,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("api-secret".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "api"
                    }
                },
                new Client
                {
                    ClientName = "Chat",
                    ClientId = "00000000-0000-0000-0000-000000000002",
                    Flow = Flows.Implicit,
                    RequireConsent = false,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44300"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:44300"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "email",
                        "roles",
                        "api"
                    }
                }
            };
        }
    }
}