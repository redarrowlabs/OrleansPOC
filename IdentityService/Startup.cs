using IdentityServer3.Core.Configuration;
using IdentityService.Configuration;
using IdentityService.Properties;
using Owin;
using System.Security.Cryptography.X509Certificates;

namespace IdentityService
{
    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder app)
        {
            var authAuthority = (string)app.Properties["AuthAuthority"];

            var factory = new IdentityServerServiceFactory()
                .UseInMemoryUsers(Users.Get())
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());

            var options = new IdentityServerOptions
            {
                SigningCertificate = new X509Certificate2(Resources.IdentityCertificate, "Testing123"),
                Factory = factory,
                EnableWelcomePage = false,
                RequireSsl = false,
                SiteName = "OrleansPOC",
                PublicOrigin = authAuthority
            };

            app.UseIdentityServer(options);
        }
    }
}