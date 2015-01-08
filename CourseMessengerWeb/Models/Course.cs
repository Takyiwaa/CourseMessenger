using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class Course
    {
        public Course()
        {
            Reminders = new List<Reminder>();
        }
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public virtual  Department Department { get; set; }

        public string Description { get; set; }

        public virtual  ICollection<Reminder> Reminders { get; set; }

    }
}