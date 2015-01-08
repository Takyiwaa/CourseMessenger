using System;
using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int ReminderId { get; set; }
        public virtual  Reminder Reminder { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public DateTime? UnsubscribeDate { get; set; }
        public int Status { get; set; }
    }
}