using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalR_Server.Startup))]
namespace SignalR_Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Any Connection or Hub wire up and configuration should go here.
            app.MapSignalR();
        }
    }
}