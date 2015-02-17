using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class Course
    {
        public Course()
        {
            Reminders = new List<ExamTimeTable>();
            
        }
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public virtual  Department Department { get; set; }

        public bool ShowCourse { get; set; }

        public string Description { get; set; }

        public virtual  ICollection<ExamTimeTable> Reminders { get; set; }


        
    }
}