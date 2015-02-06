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
     [Authorize(Roles = ApplicationRoles.Administrator)]
    public class ExamTimeTableController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ExamTimeTables

        public async Task<ActionResult> Index()
        {
            var reminders = db.ExamTimeTables.Include(r => r.Course);
            return View(await reminders.ToListAsync());
        }

        // GET: ExamTimeTables/Details/5
     
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamTimeTable examTimeTable = await db.ExamTimeTables.FindAsync(id);
            if (examTimeTable == null)
            {
                return HttpNotFound();
            }
            return View(examTimeTable);
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

        // GET: ExamTimeTables/Create
  
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code");
            ViewBag.fromDate = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.toDate = DateTime.Now.AddHours(2).ToString("MM/dd/yyyy");
            return View();
        }

        // POST: ExamTimeTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Create([Bind(Include = "Id,CourseId,StartTime,EndTime")] ExamTimeTable examTimeTable)
        {
            if (ModelState.IsValid)
            {
                examTimeTable.ReminderType = StatusCodes.ReminderTypes.ExamTimeTable;
                examTimeTable.Status = StatusCodes.ReminderStatusCodes.Active;
                db.ExamTimeTables.Add(examTimeTable);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", examTimeTable.CourseId);
            return View(examTimeTable);
        }

        // GET: ExamTimeTables/Edit/5

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamTimeTable examTimeTable = await db.ExamTimeTables.FindAsync(id);
            if (examTimeTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", examTimeTable.CourseId);
            ViewBag.Status = new SelectList(StatusCodes.ReminderStatusCodes.All, "Key", "Value", examTimeTable.Status);
            return View(examTimeTable);
        }

        // POST: ExamTimeTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Edit([Bind(Include = "Id,CourseId,StartTime,EndTime,Status")] ExamTimeTable examTimeTable)
        {
            if (ModelState.IsValid)
            {
                examTimeTable.ReminderType = StatusCodes.ReminderTypes.ExamTimeTable;
                db.Entry(examTimeTable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", examTimeTable.CourseId);
            return View(examTimeTable);
        }

        // GET: ExamTimeTables/Delete/5

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamTimeTable examTimeTable = await db.ExamTimeTables.FindAsync(id);
            if (examTimeTable == null)
            {
                return HttpNotFound();
            }
            return View(examTimeTable);
        }

        // POST: ExamTimeTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ExamTimeTable examTimeTable = await db.ExamTimeTables.FindAsync(id);
            db.ExamTimeTables.Remove(examTimeTable);
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
