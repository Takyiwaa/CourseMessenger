﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
        public async Task<ActionResult> Index()
        {
            return View(await db.Subscriptions.ToListAsync());
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

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id);
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
                   
                    RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["ExamTimeTable.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);
                }
                else
                {
                    subscription.Status = 1;
                    using (var context = new ApplicationDbContext())
                    {
                        context.Entry(subscription).State = EntityState.Modified;
                        await context.SaveChangesAsync();
                    }

                    RecurringJob.AddOrUpdate(ConfigurationManager.AppSettings["ExamTimeTable.CronJob.Id"], () => new SmsEngine().NotifyStudents(), Cron.Hourly);
                }
               
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

                var subscription = await db.Subscriptions.FirstOrDefaultAsync(e => e.EntityId == id);
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

                    if (subscriptionList.All(l => l.EntityId != examReminder.Id))
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
}