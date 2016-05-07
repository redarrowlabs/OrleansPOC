using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Client.Startup))]

namespace Client
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}