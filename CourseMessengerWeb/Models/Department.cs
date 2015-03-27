using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class Department
    {
        public Department()
        {
            Courses = new List<Course>();
            ApplicationUsers = new List<ApplicationUser>();
        }
        [Key]
        public int Id { get; set; }

        [Required (ErrorMessage = "enter a name for the department")]
        public string Name { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}