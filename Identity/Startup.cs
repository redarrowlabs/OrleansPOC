using Identity.Configuration;
using Identity.Properties;
using IdentityServer3.Core.Configuration;
using Microsoft.Owin;
using Owin;
using Serilog;
using System.Security.Cryptography.X509Certificates;

[assembly: OwinStartup(typeof(Identity.Startup))]

namespace Identity
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();

            var factory = new IdentityServerServiceFactory()
                .UseInMemoryUsers(Users.Get())
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());

            var options = new IdentityServerOptions
            {
                SigningCertificate = new X509Certificate2(Resources.IdentityCertificate, "Testing123"),
                Factory = factory,
                EnableWelcomePage = false,
                SiteName = "OrleansPOC"
            };

            app.UseHsts();
            app.UseIdentityServer(options);
        }
    }
}