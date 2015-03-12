using System.Configuration;
using CourseMessengerWeb.Components;
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

           

            RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["ExamTimeTable.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);

            RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["LectureHours.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);

            RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["NewsTips.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Daily);
        }
    }
}
