using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseMessengerWeb.Components;

namespace CourseMessengerWeb.Models
{
    public class LectureHour
    {
        public LectureHour()
        {
            Subscriptions = new List<Subscription>();   
        }
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public TimeSpan StartTime { get; set; }

        public float Duration { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public int ReminderType { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public int Status { get; set; }
    }
}