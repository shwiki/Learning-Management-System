using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(sys.Startup))]
namespace sys
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
