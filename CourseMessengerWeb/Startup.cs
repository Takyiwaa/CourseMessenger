using CourseMessengerWeb.Controllers;
using Hangfire;
using Hangfire.SqlServer;
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

            app.UseHangfire(config =>
            {
                config.UseSqlServerStorage("DefaultConnection");
                config.UseServer();
                
            });

            BackgroundJob.Enqueue(() =>
               new SmsEngine().NotifyStudents());
        }
    }
}
