using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(comp2007pmMusicStore.Startup))]
namespace comp2007pmMusicStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
