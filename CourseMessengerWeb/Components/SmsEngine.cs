using System;
using System.Configuration;
using System.Linq;
using CourseMessengerWeb.Models;
using NLog;

namespace CourseMessengerWeb.Components
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
                                d => d.ReminderType == (int)StatusCodes.ReminderTypes.ExamTimeTable);

                        if (reminderMessage != null)
                        {
                            string phoneNumber;
                            var msg = ReplaceExamTimeTablePlaceholders(reminderMessage.Message, activeSubsciption.EntityId, activeSubsciption.IndexNumber, out phoneNumber);

                            Logger.Debug("sending message: {0} to {1}",msg,phoneNumber);
                            new SmsModule().SendSms(phoneNumber, msg);
                        }
                    }
                    
                }
                else if (activeSubsciption.SubscriptionType == SubscriptionType.LectureHours)
                {
                    int remindAt = int.Parse(ConfigurationManager.AppSettings["LectureHours.RemindAt.Minutes"]);
                    var lectureHour = _context.LectureHours.FirstOrDefault(e => e.Id == activeSubsciption.EntityId);

                    if (lectureHour == null)
                    {
                        continue;
                    }

                    if (DateTime.Now.DayOfWeek== lectureHour.DayOfWeek)
                    {
                         var timeDifference = DateTime.Now.TimeOfDay - lectureHour.StartTime.Add(new TimeSpan(0,0,remindAt,0));
                        if (timeDifference.Days == 0 && timeDifference.Hours == 0 && timeDifference.Minutes >= 0)
                        {
                            Logger.Debug("reminder is due for id:{0}" + activeSubsciption.Id);
                            var reminderMessage =
                                _context.ReminderMessages.FirstOrDefault(
                                    d => d.ReminderType == (int) StatusCodes.ReminderTypes.LectureHours);

                            if (reminderMessage != null)
                            {
                                string phoneNumber;
                                var msg = ReplaceLectureHoursPlaceholders(reminderMessage.Message,
                                    activeSubsciption.EntityId, activeSubsciption.IndexNumber, out phoneNumber);

                                Logger.Debug("sending message: {0} to {1}", msg, phoneNumber);
                                new SmsModule().SendSms(phoneNumber, msg);
                            }
                        }
                    }
                   

                }

                else if (activeSubsciption.SubscriptionType == SubscriptionType.NewsTips)
                {
                    int remindAt = int.Parse(ConfigurationManager.AppSettings["NewsTips.RemindAt.Minutes"]);
                    var newsTips = _context.NewsTips.FirstOrDefault(e => e.Id == activeSubsciption.EntityId);

                    if (newsTips == null)
                    {
                        continue;
                    }

                    var timeDifference = DateTime.Now.TimeOfDay - newsTips.StartTime.Add(new TimeSpan(0, 0, remindAt, 0));
                    if (timeDifference.Days == 0 && timeDifference.Hours == 0 && timeDifference.Minutes >= 0)
                    {
                        Logger.Debug("reminder is due for id:{0}" + activeSubsciption.Id);
                        var reminderMessage =
                            _context.ReminderMessages.FirstOrDefault(
                                d => d.ReminderType == (int)StatusCodes.ReminderTypes.NewsTips);

                        if (reminderMessage != null)
                        {
                            string phoneNumber;
                            var msg = ReplaceNewsTipsPlaceholders(reminderMessage.Message, activeSubsciption.EntityId, activeSubsciption.IndexNumber, out phoneNumber);

                            Logger.Debug("sending message: {0} to {1}", msg, phoneNumber);
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

        private string ReplaceLectureHoursPlaceholders(string message, int entityId, string indexNumber, out string phoneNumber)
        {
            var lectureHour = _context.LectureHours.FirstOrDefault(e => e.Id == entityId);

            if (lectureHour != null)
            {
                var fakeDate = DateTime.Now.Add(lectureHour.StartTime);

                message = message.Replace("[Course]", lectureHour.Course.Name + "(" + lectureHour.Course.Code + ")")
                        .Replace("[StartTime]", fakeDate.ToString("h:mm tt"))
                        .Replace("[Duration]", lectureHour.Duration.ToString(""));


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

        private string ReplaceNewsTipsPlaceholders(string message, int entityId, string indexNumber, out string phoneNumber)
        {
            var examTimeTable = _context.NewsTips.FirstOrDefault(e => e.Id == entityId);

            if (examTimeTable != null)
            {
             
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