using System;
using System.Configuration;
using System.Linq;
using CourseMessengerWeb.Components;
using CourseMessengerWeb.Models;
using NLog;

namespace CourseMessengerWeb.Controllers
{
    public class SmsEngine
    {
        private readonly ApplicationDbContext _context;
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public SmsEngine()
        {
            _context = new ApplicationDbContext();
        }
        public void NotifyStudents()
        {
            Logger.Debug("Started NotifyStudents() on {0}",DateTime.Now.ToString("F"));
            //fetch all active subscriptions from subscriptions table
            var activeSubsciptions = _context.Subscriptions.Where(s => s.Status == 1);

            //loop through and send SMS

            foreach (var activeSubsciption in activeSubsciptions)
            {
                if (activeSubsciption.SubscriptionType == SubscriptionType.ExamTimeTable)
                {
                    int remindAt = int.Parse(ConfigurationManager.AppSettings["ExamTimeTable.RemindAt.Minutes"]);
                    var examTimeTable = _context.ExamTimeTables.FirstOrDefault(e => e.Id == activeSubsciption.EntityId);

                    if (examTimeTable == null)
                    {
                        continue;
                    }

                    var timeDifference = DateTime.Now - examTimeTable.StartTime.AddMinutes(remindAt);
                    if (timeDifference.Days == 0 && timeDifference.Hours == 0 && timeDifference.Minutes >= 0)
                    {
                        Logger.Debug("reminder is due for id:{0}" +activeSubsciption.Id);
                        var reminderMessage =
                            _context.ReminderMessages.FirstOrDefault(
                                d => d.ReminderType == (int)SubscriptionType.ExamTimeTable);

                        if (reminderMessage != null)
                        {
                            string phoneNumber;
                            var msg = ReplaceExamTimeTablePlaceholders(reminderMessage.Message, activeSubsciption.EntityId, activeSubsciption.IndexNumber, out phoneNumber);

                            Logger.Debug("sending message: {0} to {1}",msg,phoneNumber);
                            new SmsModule().SendSms(phoneNumber, msg);
                        }
                    }
                    
                }
            }
        }

        private string ReplaceExamTimeTablePlaceholders(string message, int entityId, string indexNumber, out string phoneNumber)
        {
            var examTimeTable = _context.ExamTimeTables.FirstOrDefault(e => e.Id == entityId);

            if (examTimeTable != null)
            {
                message =
                    message.Replace("[Course]", examTimeTable.Course.Name + "(" + examTimeTable.Course.Code + ")")
                        .Replace("[StartTime]", examTimeTable.StartTime.ToString("HH:mm"))
                        .Replace("[EndTime]", examTimeTable.EndTime.ToString("HH:mm"));


                var student = _context.Users.FirstOrDefault(u => u.StudentId == indexNumber);
                var fon = string.Empty;
                if (student != null)
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