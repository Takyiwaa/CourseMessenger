using System.Collections.Generic;
using System.Configuration;
using CourseMessengerWeb.Components;
using CourseMessengerWeb.Controllers;
using Hangfire;
using Hangfire.Dashboard;
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
                config.UseAuthorizationFilters(new MyRestrictiveAuthorizationFilter());
            });

           

            RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["ExamTimeTable.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);

            RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["LectureHours.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);

            RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["NewsTips.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Daily(7,30));
        }
    }

    public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line.
            // `OwinContext` class is defined in the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            return true; // or `true` to allow access
        }
    }
}
