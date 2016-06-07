using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;
using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Configuration
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "jcase@example.com",
                    Password = "Testing123",
                    Subject = "00000000-0000-0000-0000-000000000001",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Justin"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Case"),
                        new Claim(Constants.ClaimTypes.Email, "jcase@example.com"),
                        new Claim(Constants.ClaimTypes.Role, "patient")
                    }
                },
                new InMemoryUser
                {
                    Username = "gpoole@example.com",
                    Password = "Testing123",
                    Subject = "00000000-0000-0000-0000-000000000002",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Gene"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Poole"),
                        new Claim(Constants.ClaimTypes.Email, "gpoole@example.com"),
                        new Claim(Constants.ClaimTypes.Role, "patient")
                    }
                },
                new InMemoryUser
                {
                    Username = "dsnuggles@example.com",
                    Password = "Testing123",
                    Subject = "00000000-0000-0000-0000-000000000003",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Doctor"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Snuggles"),
                        new Claim(Constants.ClaimTypes.Email, "dsnuggles@example.com"),
                        new Claim(Constants.ClaimTypes.Role, "admin"),
                        new Claim(Constants.ClaimTypes.Role, "provider")
                    }
                },
                new InMemoryUser
                {
                    Username = "dnobody@example.com",
                    Password = "Testing123",
                    Subject = "00000000-0000-0000-0000-000000000004",
                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Doctor"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Snuggles"),
                        new Claim(Constants.ClaimTypes.Email, "dnobody@example.com"),
                        new Claim(Constants.ClaimTypes.Role, "admin"),
                        new Claim(Constants.ClaimTypes.Role, "provider")
                    }
                }
            };
        }
    }
}