using ApiService.Infrastructure;
using IdentityServer3.AccessTokenValidation;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Web.Http;

namespace ApiService
{
    public static class Startup
    {
        public static void ConfigureApp(IAppBuilder app)
        {
            app.UseOrleans();

            app.Map("/signalr", builder =>
            {
                GlobalHost.DependencyResolver.UseRedis("localhost", 6379, null, "OrleansPOC");

                var settings = new JsonSerializerSettings { ContractResolver = new SignalRContractResolver() };
                var serializer = JsonSerializer.Create(settings);
                GlobalHost.DependencyResolver.Register(
                   typeof(JsonSerializer),
                   () => serializer
                );

                GlobalHost.DependencyResolver.Register(
                    typeof(IUserIdProvider),
                    () => new SignalRUserIdProvider()
                );

                builder.UseCors(CorsOptions.AllowAll);

                builder.UseIdentityServerBearerTokenAuthentication(
                    new IdentityServerBearerTokenAuthenticationOptions
                    {
                        Authority = "https://localhost:44301",
                        ClientId = "00000000-0000-0000-0000-000000000001",
                        ClientSecret = "api-secret",
                        RequiredScopes = new[] { "api" },
                        TokenProvider = new QueryStringOAuthTokenProvider()
                    }
                );

                builder.RunSignalR();
            });

            app.Map("/api", builder =>
            {
                builder.UseCors(CorsOptions.AllowAll);

                builder.UseIdentityServerBearerTokenAuthentication(
                    new IdentityServerBearerTokenAuthenticationOptions
                    {
                        Authority = "https://localhost:44301",
                        ClientId = "00000000-0000-0000-0000-000000000001",
                        ClientSecret = "api-secret",
                        RequiredScopes = new[] { "api" }
                    }
                );

                var httpConfiguration = new HttpConfiguration();
                httpConfiguration.MapHttpAttributeRoutes();
                httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                httpConfiguration.Filters.Add(new System.Web.Http.AuthorizeAttribute());

                builder.UseWebApi(httpConfiguration);
            });
        }
    }
}