using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CourseMessengerWeb.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }

        public DbSet<ExamTimeTable> ExamTimeTables { get; set; }
        public DbSet<ReminderMessage> ReminderMessages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}