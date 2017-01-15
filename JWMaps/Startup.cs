using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JWMaps.Startup))]
namespace JWMaps
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
