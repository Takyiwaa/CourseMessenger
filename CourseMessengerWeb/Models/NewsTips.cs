using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CourseMessengerWeb.Components;
using System.Linq;
using System.Web;

namespace CourseMessengerWeb.Models
{
    public class NewsTips
    {

        public NewsTips()
        {
            Subscriptions = new List<Subscription>(); 
        }
        [Key]
        public int Id { get; set; }

        public int ReminderType { get; set; }

        public string Name { get; set; }

        public TimeSpan StartTime { get; set; }

        
        public string Description { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        public int Status { get; set; }
    }
}