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

namespace CourseMessengerWeb.Controllers
{
    [Authorize(Roles = ApplicationRoles.Administrator)]
    public class LectureHoursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LectureHours
        public async Task<ActionResult> Index()
        {
            ViewBag.Count = await db.LectureHours.CountAsync();
            var lectureHours = db.LectureHours.Include(l => l.Course);
            return View(await lectureHours.ToListAsync());
        }

        // GET: LectureHours/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LectureHour lectureHour = await db.LectureHours.FindAsync(id);
            if (lectureHour == null)
            {
                return HttpNotFound();
            }
            return View(lectureHour);
        }

        // GET: LectureHours/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code");
            return View();
        }

        // POST: LectureHours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CourseId,StartTime,Duration,DayOfWeek")] LectureHour lectureHour)
        {

            var req = Request;

       
            var startTime = req["StartTime"];
          

            var newTime = DateTime.Parse(startTime);

            lectureHour.StartTime = newTime.TimeOfDay;
            ModelState.Clear();
            
            if (ModelState.IsValid)
            {
                lectureHour.ReminderType = StatusCodes.ReminderTypes.LectureHours;
                lectureHour.Status = 1;
                db.LectureHours.Add(lectureHour);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", lectureHour.CourseId);
            return View(lectureHour);
        }

        // GET: LectureHours/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LectureHour lectureHour = await db.LectureHours.FindAsync(id);
            if (lectureHour == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(StatusCodes.ReminderStatusCodes.All, "Key", "Value", lectureHour.Status);
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", lectureHour.CourseId);
            return View(lectureHour);
        }

        // POST: LectureHours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CourseId,StartTime,Duration,DayOfWeek,ReminderType,Status")] LectureHour lectureHour)
        {

            var req = Request;


            var startTime = req["StartTime"];


            var newTime = DateTime.Parse(startTime);

            lectureHour.StartTime = newTime.TimeOfDay;
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                db.Entry(lectureHour).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Code", lectureHour.CourseId);
            return View(lectureHour);
        }

        // GET: LectureHours/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LectureHour lectureHour = await db.LectureHours.FindAsync(id);
            if (lectureHour == null)
            {
                return HttpNotFound();
            }
            return View(lectureHour);
        }

        // POST: LectureHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LectureHour lectureHour = await db.LectureHours.FindAsync(id);
            db.LectureHours.Remove(lectureHour);
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
