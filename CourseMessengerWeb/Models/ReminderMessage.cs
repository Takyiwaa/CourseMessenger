using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class ReminderMessage
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
        public int ReminderType { get; set; }
    }
}