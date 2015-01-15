using System.ComponentModel.DataAnnotations;

namespace CourseMessengerWeb.Models
{
    public class ConfirmAccountViewModel
    {
        [Required]
        public string EmailCode { get; set; }
        [Required]
        public string SmsCode { get; set; }
        [Required]
        public string UserId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}