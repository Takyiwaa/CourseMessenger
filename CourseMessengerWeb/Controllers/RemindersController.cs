using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CourseMessengerWeb.Components;
using CourseMessengerWeb.Models;
using Microsoft.AspNet.Identity.Owin;

namespace CourseMessengerWeb.Controllers
{
    [Authorize]
    public class RemindersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reminders
          [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> Index()
        {
            var reminders = db.Reminders.Include(r => r.Course);
            return View(await reminders.ToListAsync());
        }

        // GET: Reminders/Details/5
          [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reminder reminder = await db.Reminders.FindAsync(id);
            if (reminder == null)
            {
                return HttpNotFound();
            }
            return View(reminder);
        }

        [Authorize(Roles = "Administrator,Student")]
        public async Task<ActionResult> StudentView()
        {
            
            //var mySubscriptions = db.Subscriptions.Where(s => s.ApplicationUser.UserName == User.Identity.Name);
            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            if (user!=null)
            {

                var examReminders = db.Reminders.Include(t=>t.Course).Where(r => r.Course.DepartmentId == user.DepartmentId);
                return View(examReminders);
            }
           

            return View();
        }

           [Authorize(Roles = "Administrator,Student")]
        public async Task<ActionResult> Subscribe(int id)
        {

            var user = await UserManager.FindByEmailAsync(User.Identity.Name);

            var reminder = await db.Reminders.FindAsync(id);
            if (reminder==null)
            {
                return HttpNotFound("couldn't find that exam time table");
            }

            var subscription = new Subscription
                               {
                                   IndexNumber = user.StudentId,
                                   ReminderId = id,
                                   Status = 1,
                                   SubscriptionDate = DateTime.Now,
                               };

               using (var context = new ApplicationDbContext())
               {
                   context.Subscriptions.Add(subscription);
                   await context.SaveChangesAsync();
               }
          
            return RedirectToAction("StudentView");
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

        // GET: Reminders/Create
          [Authorize(Roles = ApplicationRoles.Administrator)]
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code");
            return View();
        }

        // POST: Reminders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> Create([Bind(Include = "Id,CourseId,StartTime,EndTime,Status,ReminderType")] Reminder reminder)
        {
            if (ModelState.IsValid)
            {
                db.Reminders.Add(reminder);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", reminder.CourseId);
            return View(reminder);
        }

        // GET: Reminders/Edit/5
          [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reminder reminder = await db.Reminders.FindAsync(id);
            if (reminder == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", reminder.CourseId);
            return View(reminder);
        }

        // POST: Reminders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CourseId,StartTime,EndTime,Status,ReminderType")] Reminder reminder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reminder).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", reminder.CourseId);
            return View(reminder);
        }

        // GET: Reminders/Delete/5
          [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reminder reminder = await db.Reminders.FindAsync(id);
            if (reminder == null)
            {
                return HttpNotFound();
            }
            return View(reminder);
        }

        // POST: Reminders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrator)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reminder reminder = await db.Reminders.FindAsync(id);
            db.Reminders.Remove(reminder);
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
