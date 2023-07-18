using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WSB.Activity.Mvc.Startup))]
namespace WSB.Activity.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
