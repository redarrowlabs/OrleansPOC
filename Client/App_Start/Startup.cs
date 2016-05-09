using Client.Infrastructure;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Newtonsoft.Json;
using Owin;

[assembly: OwinStartup(typeof(Client.Startup))]

namespace Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                var settings = new JsonSerializerSettings { ContractResolver = new SignalRContractResolver() };
                var serializer = JsonSerializer.Create(settings);
                GlobalHost.DependencyResolver.Register(
                   typeof(JsonSerializer),
                   () => serializer
                );

                map.RunSignalR();
            });
        }
    }
}