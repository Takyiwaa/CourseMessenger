using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CourseMessengerWeb.Components;
using CourseMessengerWeb.Models;
using Elmah;
using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CourseMessengerWeb.Controllers
{
    [Authorize]
    public class SubscriptionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscriptions
          [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> Index()
          {

              var subscriptionsByStudents = db.Subscriptions.GroupBy(s=>s.IndexNumber);


              var subscriptionsLists = new List<SubscriptionItem>();
              subscriptionsLists.Clear();
              foreach (var subscriptions in subscriptionsByStudents)
              {
                  var item1 = subscriptions;
                  var subscriptionItem = new SubscriptionItem();
                  
                  subscriptionItem.StudentDetails = await db.Users.FirstOrDefaultAsync(s => s.StudentId == item1.Key);
              
                  foreach (var subscription in subscriptions)
                  {
                      string description = string.Empty;
                      var subType = new StudentSubscriptionType
                      {
                          SubscriptionTypeName = subscription.SubscriptionType,
                          Status = subscription.Status,
                          EntityId = subscription.EntityId
                      };
                      switch (subscription.SubscriptionType)
                      {
                          case SubscriptionType.ExamTimeTable:
                              var exam = await db.ExamTimeTables.FirstOrDefaultAsync(e => e.Id == subType.EntityId);
                              description = exam.Course.Name + " (" + exam.Course.Code + ")";
                        
                              break;
                          case SubscriptionType.LectureHours:
                               var lectureHour = await db.LectureHours.FirstOrDefaultAsync(e => e.Id == subType.EntityId);
                              description = lectureHour.Course.Name + " (" + lectureHour.Course.Code + ")";
                              break;
                          case SubscriptionType.LectureMaterialUrl:
                              break;
                          case SubscriptionType.NewsTips:
                               var newsTip = await db.NewsTips.FirstOrDefaultAsync(e => e.Id == subType.EntityId);
                              description = newsTip.Name + " (" + newsTip.Description + ")";
                              break;
                          default:
                              throw new ArgumentOutOfRangeException();
                      }
                      subType.Description = description;
                      subscriptionItem.SubscriptionLists.Add(subType);
                  }

                  subscriptionsLists.Add(subscriptionItem);

              }
             
              //return View("Index2", subscriptionsByStudents);
              return View("Index3", subscriptionsLists);
   
          }

        // GET: Subscriptions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "Administrator,Student")]
        public async Task<ActionResult> Subscribe(int id,SubscriptionType subscriptionType)
        {

           
            if (subscriptionType== SubscriptionType.ExamTimeTable)
            {
                var examInfo = await db.ExamTimeTables.FindAsync(id);
                if (examInfo == null)
                {
                    return HttpNotFound("couldn't find that exam time table");
                }

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id && e.SubscriptionType == SubscriptionType.ExamTimeTable && e.IndexNumber==User.Identity.Name);
                if (subscription == null)
                {
                    var subscription1 = new Subscription
                                        {
                                            IndexNumber = User.Identity.Name,
                                            SubscriptionType = SubscriptionType.ExamTimeTable,
                                            EntityId = examInfo.Id,
                                            Status = 1,
                                            SubscriptionDate = DateTime.Now,
                                        };

                    using (var context = new ApplicationDbContext())
                    {

                        context.Subscriptions.Add(subscription1);
                        await context.SaveChangesAsync();
                    }
                   
              
                }
                else
                {
                    subscription.Status = 1;
                    using (var context = new ApplicationDbContext())
                    {
                        context.Entry(subscription).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                   // RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["ExamTimeTable.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);
                }
               
            }

            if (subscriptionType == SubscriptionType.LectureHours)
            {
                var lectureHour = await db.LectureHours.FindAsync(id);
                if (lectureHour == null)
                {
                    return HttpNotFound("couldn't find that lecture hour slot");
                }

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id && e.SubscriptionType == SubscriptionType.LectureHours && e.IndexNumber == User.Identity.Name);
                if (subscription == null)
                {
                    var subscription1 = new Subscription
                    {
                        IndexNumber = User.Identity.Name,
                        SubscriptionType = SubscriptionType.LectureHours,
                        EntityId = lectureHour.Id,
                        Status = 1,
                        SubscriptionDate = DateTime.Now,
                    };

                    using (var context = new ApplicationDbContext())
                    {

                        context.Subscriptions.Add(subscription1);
                        await context.SaveChangesAsync();
                    }

                   // RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["LectureHours.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);
                }
                else
                {
                    subscription.Status = 1;
                    using (var context = new ApplicationDbContext())
                    {
                        context.Entry(subscription).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                   // RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["LectureHours.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);
                }

            }
            if (subscriptionType == SubscriptionType.NewsTips)
            {
                var newsTips = await db.NewsTips.FindAsync(id);
                if (newsTips == null)
                {
                    return HttpNotFound("couldn't find that news tips");
                }

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id && e.SubscriptionType == SubscriptionType.NewsTips && e.IndexNumber == User.Identity.Name);
                if (subscription == null)
                {
                    var subscription1 = new Subscription
                    {
                        IndexNumber = User.Identity.Name,
                        SubscriptionType = SubscriptionType.NewsTips,
                        EntityId = newsTips.Id,
                        Status = 1,
                        SubscriptionDate = DateTime.Now,
                    };

                    using (var context = new ApplicationDbContext())
                    {

                        context.Subscriptions.Add(subscription1);
                        await context.SaveChangesAsync();
                    }

                 //   RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["NewsTips.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Daily);
                }
                else
                {
                    subscription.Status = 1;
                    using (var context = new ApplicationDbContext())
                    {
                        context.Entry(subscription).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                   // RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["NewsTips.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Daily);
                }

            }

            if (User.IsInRole(ApplicationRoles.Administrator))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("MySubs", "subscriptions");
        }


        [Authorize(Roles = "Administrator,Student")]
        public async Task<ActionResult> Unsubscribe(int id, SubscriptionType subscriptionType)
        {

            if (subscriptionType == SubscriptionType.ExamTimeTable)
            {
                var examInfo = await db.ExamTimeTables.FindAsync(id);
                if (examInfo == null)
                {
                    return HttpNotFound("couldn't find that exam time table");
                }

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id && e.SubscriptionType == SubscriptionType.ExamTimeTable);
                if (subscription != null)
                {
                    subscription.Status = 0;

                    using (var context = new ApplicationDbContext())
                    {
                        context.Entry(subscription).State= EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                    RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["ExamTimeTable.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);
                }

            }

            if (subscriptionType == SubscriptionType.LectureHours)
            {
                var lectureHour = await db.LectureHours.FindAsync(id);
                if (lectureHour == null)
                {
                    return HttpNotFound("couldn't find that lecture hour slot");
                }

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id && e.SubscriptionType == SubscriptionType.LectureHours);
                if (subscription != null)
                {
                    subscription.Status = 0;

                    using (var context = new ApplicationDbContext())
                    {
                        context.Entry(subscription).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                    RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["LectureHours.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);
                }

            }
            if (subscriptionType == SubscriptionType.NewsTips)
            {
                var lectureHour = await db.NewsTips.FindAsync(id);
                if (lectureHour == null)
                {
                    return HttpNotFound("couldn't find that news tips");
                }

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id && e.SubscriptionType == SubscriptionType.NewsTips);
                if (subscription != null)
                {
                    subscription.Status = 0;

                    using (var context = new ApplicationDbContext())
                    {
                        context.Entry(subscription).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                    RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["NewsTips.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Daily);
                }

            }

            if (User.IsInRole(ApplicationRoles.Administrator))
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("MySubs", "subscriptions");
        }


        [Authorize(Roles = "Administrator,Student")]
        public async Task<ActionResult> MySubs()
        {

            //var mySubscriptions = db.Subscriptions.Where(s => s.ApplicationUser.UserName == User.Identity.Name);
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            if (user != null)
            {
                var mySubscriptions =
                    db.Subscriptions.Where(s => s.IndexNumber == user.StudentId).GroupBy(g => g.SubscriptionType);


                var examReminders = db.ExamTimeTables.Include(t => t.Course).Where(r => r.Course.DepartmentId == user.DepartmentId && r.Status==1).ToList();
                var lectureHoursReminders = db.LectureHours.Include(t => t.Course).Where(r => r.Course.DepartmentId == user.DepartmentId && r.Status==1).ToList();
                var newsTipsReminders = db.NewsTips.Where(r => r.Status==1).ToList();

                var subscriptionList = new List<SubscriptionViewModel>();

                foreach (var examReminder in examReminders)
                {

                    if (examReminder.StartTime < DateTime.Now)
                    {
                        continue;
                    }
                    var sub = new SubscriptionViewModel
                              {
                                  EntityId = examReminder.Id,
                                  Name = examReminder.Course.Code + " (" +  examReminder.Course.Name + ")",
                                  SubscriptionType =  SubscriptionType.ExamTimeTable,
                                  Status = 0,
                                  Description = examReminder.Course.Name + " starts at " + examReminder.StartTime.ToString("F") + " and ends at " + examReminder.EndTime.ToString("F"),
                                  
                              };

                        foreach (var mySubscription in mySubscriptions)
                        {
                            if (mySubscription.Any())
                            {
                                if (mySubscription.Key == SubscriptionType.ExamTimeTable)
                                {
                                    foreach (var subscription in mySubscription)
                                    {
                                        if (subscription.EntityId == examReminder.Id)
                                        {
                                            sub.EntityId = examReminder.Id;
                                            sub.Status = subscription.Status;
                                            subscriptionList.Add(sub);
                                        }
                                     
                                    }

                                }
                            }
                            else
                            {
                                subscriptionList.Add(sub);
                            }
                           
                        }

                        if (!subscriptionList.Any(l => l.EntityId == examReminder.Id && l.SubscriptionType == SubscriptionType.ExamTimeTable))
                    {
                        subscriptionList.Add(sub);
                    }
                        
                    
                }


                foreach (var lectureHour in lectureHoursReminders)
                {

                    //if (lectureHour.StartTime < DateTime.Now.TimeOfDay)
                    //{
                    //    continue;
                    //}
                    var sub = new SubscriptionViewModel
                    {
                        EntityId = lectureHour.Id,
                        Name = lectureHour.Course.Code + " (" + lectureHour.Course.Name + ")",
                        SubscriptionType = SubscriptionType.LectureHours,
                        Status = 0,
                        Description = lectureHour.Course.Name + " starts at " + lectureHour.StartTime.ToString("t") + " and lasts for" + lectureHour.Duration.ToString("F") + " hours",

                    };

                    foreach (var mySubscription in mySubscriptions)
                    {
                        if (mySubscription.Any())
                        {
                            if (mySubscription.Key == SubscriptionType.LectureHours)
                            {
                                foreach (var subscription in mySubscription)
                                {
                                    if (subscription.EntityId == lectureHour.Id)
                                    {
                                        sub.EntityId = lectureHour.Id;
                                        sub.Status = subscription.Status;
                                        subscriptionList.Add(sub);
                                    }

                                }

                            }
                            else
                            {
                                sub.EntityId = lectureHour.Id;
                                sub.Status = 0;
                                subscriptionList.Add(sub);
                            }
                        }
                        else
                        {

                            subscriptionList.Add(sub);
                        }

                    }

                    if (!subscriptionList.Any(l => l.EntityId == lectureHour.Id && l.SubscriptionType == SubscriptionType.LectureHours))
                    {
                        subscriptionList.Add(sub);
                    }


                }

                foreach (var newsTips in newsTipsReminders)
                {

                    //if (lectureHour.StartTime < DateTime.Now.TimeOfDay)
                    //{
                    //    continue;
                    //}
                    var sub = new SubscriptionViewModel
                    {
                        EntityId = newsTips.Id,
                        Name = newsTips.Name ,
                        SubscriptionType = SubscriptionType.NewsTips,
                        Status = 0,
                        Description = newsTips.Description + ". Reminder starts at " + newsTips.StartTime.ToString("t") + " everyday.",

                    };

                    foreach (var mySubscription in mySubscriptions)
                    {
                        if (mySubscription.Any())
                        {
                            if (mySubscription.Key == SubscriptionType.NewsTips)
                            {
                                foreach (var subscription in mySubscription)
                                {
                                    if (subscription.EntityId == newsTips.Id)
                                    {
                                        sub.EntityId = newsTips.Id;
                                        sub.Status = subscription.Status;
                                        subscriptionList.Add(sub);
                                    }

                                }

                            }
                            else
                            {
                                sub.EntityId = newsTips.Id;
                                sub.Status = 0;
                                subscriptionList.Add(sub);
                            }
                        }
                        else
                        {
                            subscriptionList.Add(sub);
                        }

                    }

                    if (subscriptionList.All(l => l.EntityId != newsTips.Id && l.SubscriptionType==SubscriptionType.NewsTips))
                    {
                        subscriptionList.Add(sub);
                    }


                }

                // var examReminders = db.ExamTimeTables.Include(t=>t.Course).Where(r => r.Course.DepartmentId == user.DepartmentId);



                return View(subscriptionList);
            }


            return View();
        }

        // GET: Subscriptions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,IndexNumber,EntityId,SubscriptionType,SubscriptionDate,UnsubscribeDate,Status")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                db.Subscriptions.Add(subscription);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,IndexNumber,EntityId,SubscriptionType,SubscriptionDate,UnsubscribeDate,Status")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subscription).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            db.Subscriptions.Remove(subscription);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class StudentSubscriptionType
    {
        public SubscriptionType SubscriptionTypeName { get; set; }
        public int Status { get; set; }
        public int EntityId { get; set; }
        public string Description { get; set; }
    }

    public class SubscriptionItem
    {
        public SubscriptionItem()
        {
            SubscriptionLists = new List<StudentSubscriptionType>();
            StudentDetails = new ApplicationUser();
        }
        public ApplicationUser StudentDetails { get; set; }
        public List<StudentSubscriptionType> SubscriptionLists { get; set; }
    }

    public class AdminSubscriptionViewModel
    {
        public string Student { get; set; }
    }
}
