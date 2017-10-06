using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Nsc.Wu.AdminApp.Startup))]
namespace Nsc.Wu.AdminApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
