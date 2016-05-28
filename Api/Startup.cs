using Api.Infrastructure;
using IdentityServer3.AccessTokenValidation;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orleans;
using Orleans.Runtime.Configuration;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Api.Startup))]

namespace Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", builder =>
            {
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

            var orleansConfig = ClientConfiguration.LocalhostSilo();
            GrainClient.Initialize(orleansConfig);
        }
    }
}