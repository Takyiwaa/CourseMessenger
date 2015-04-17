using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class ExamTimeTable
    {
        public ExamTimeTable()
        {
            Subscriptions = new List<Subscription>();
        }
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int Status { get; set; }
        public int ReminderType { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}