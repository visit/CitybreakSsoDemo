using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CitybreakSsoDemo.Startup))]
namespace CitybreakSsoDemo
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
