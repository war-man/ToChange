using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ToChange.Startup))]

namespace ToChange
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR(); //Enable SignalR
        }
    }
}
