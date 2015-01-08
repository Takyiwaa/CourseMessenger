using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CourseMessengerWeb.Startup))]
namespace CourseMessengerWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
