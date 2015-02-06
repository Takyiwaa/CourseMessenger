using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseMessengerWeb.Components;
using CourseMessengerWeb.Models;
using Microsoft.Owin.Security.Provider;

namespace CourseMessengerWeb.Controllers
{
    public class SmsEngineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SmsEngineController()
        {
            _context = new ApplicationDbContext();
            
        }
        // GET: SmsEngine
        [AllowAnonymous]
        public ActionResult Run()
        {

            //fetch all active subscriptions from subscriptions table
            var activeSubsciptions = _context.Subscriptions.Where(s => s.Status == 1);

            //loop through and send SMS

            foreach (var activeSubsciption in activeSubsciptions)
            {
                //for every active subscription, if the reminder type is that of an exam timetable
                if (activeSubsciption.SubscriptionType== SubscriptionType.ExamTimeTable)
                {
                    int remindAt = int.Parse(ConfigurationManager.AppSettings["ExamTimeTable.RemindAt.Minutes"]);
                    var examTimeTable = _context.ExamTimeTables.FirstOrDefault(e => e.Id == activeSubsciption.EntityId);

                    if (examTimeTable==null)
                    {
                        continue;
                    }

                    var timeDifference = DateTime.Now - examTimeTable.StartTime.AddMinutes(remindAt);
                    if (timeDifference.Days==0 && timeDifference.Hours==0 && timeDifference.Minutes>=0)
                    {
                        var reminderMessage =
                       _context.ReminderMessages.FirstOrDefault(
                           d => d.ReminderType == (int)SubscriptionType.ExamTimeTable);

                        if (reminderMessage != null)
                        {
                            string phoneNumber;
                            var msg = ReplaceExamTimeTablePlaceholders(reminderMessage.Message, activeSubsciption.EntityId, activeSubsciption.IndexNumber, out phoneNumber);


                            new SmsModule().SendSms(phoneNumber, msg);
                        }
                    }
                   
                }    
            }


            return Content("ok");
        }

        private string ReplaceExamTimeTablePlaceholders(string message, int entityId, string indexNumber,out string phoneNumber)
        {
            var examTimeTable = _context.ExamTimeTables.FirstOrDefault(e => e.Id == entityId);

            if (examTimeTable!=null)
            {
                message =
                    message.Replace("[Course]", examTimeTable.Course.Name + "(" + examTimeTable.Course.Code + ")")
                        .Replace("[StartTime]", examTimeTable.StartTime.ToString("HH:mm"))
                        .Replace("[EndTime]", examTimeTable.EndTime.ToString("HH:mm"));


                var student = _context.Users.FirstOrDefault(u => u.StudentId == indexNumber);
                var fon =  string.Empty;
                if (student!=null)
                {
                    message = message.Replace("[Fullname]", student.Fullname);
                    fon = student.PhoneNumber;
                }

                phoneNumber = fon;
                return message;
            }

            phoneNumber = string.Empty;
            return message;
        }
    }
}