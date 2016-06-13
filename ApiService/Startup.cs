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
            var authAuthority = (string)app.Properties["AuthAuthority"];
            var redisConnectionString = (string)app.Properties["RedisConnectionString"];
            var orleansConnectionString = (string)app.Properties["OrleansConnectionString"];
            var orleansFabricUri = (string)app.Properties["OrleansFabricUri"];

            app.UseOrleans(orleansConnectionString, orleansFabricUri);

            app.Map("/signalr", builder =>
            {
                var redisConfig = new RedisScaleoutConfiguration(redisConnectionString, "SignalR");
                GlobalHost.DependencyResolver.UseRedis(redisConfig);

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
                        Authority = authAuthority,
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
                        Authority = authAuthority,
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