using System;
using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        public string IndexNumber { get; set; }
        public int EntityId { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        public DateTime SubscriptionDate { get; set; }
        public DateTime? UnsubscribeDate { get; set; }
        public int Status { get; set; }
    }


    public class SubscriptionViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public int EntityId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
    }
    public enum SubscriptionType
    {
        ExamTimeTable,
        LectureHours,
        LectureMaterialUrl,
        NewsTips,
    }
}