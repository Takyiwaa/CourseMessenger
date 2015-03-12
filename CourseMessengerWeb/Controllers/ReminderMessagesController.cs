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
    public class ReminderMessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReminderMessages
        public async Task<ActionResult> Index()
        {
            return View(await db.ReminderMessages.ToListAsync());
        }

        // GET: ReminderMessages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReminderMessage reminderMessage = await db.ReminderMessages.FindAsync(id);
            if (reminderMessage == null)
            {
                return HttpNotFound();
            }
            return View(reminderMessage);
        }

        // GET: ReminderMessages/Create
        public ActionResult Create()
        {
            ViewBag.ReminderType = new SelectList(StatusCodes.ReminderTypes.All, "Key", "Value");
            return View();
        }

        // POST: ReminderMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Message,ReminderType")] ReminderMessage reminderMessage)
        {
            if (ModelState.IsValid)
            {
                db.ReminderMessages.Add(reminderMessage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(reminderMessage);
        }

        // GET: ReminderMessages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReminderMessage reminderMessage = await db.ReminderMessages.FindAsync(id);
            if (reminderMessage == null)
            {
                return HttpNotFound();
            }

            ViewBag.ReminderType = new SelectList(StatusCodes.ReminderTypes.All, "Key", "Value");
            return View(reminderMessage);
        }

        // POST: ReminderMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Message,ReminderType")] ReminderMessage reminderMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reminderMessage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(reminderMessage);
        }

        // GET: ReminderMessages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReminderMessage reminderMessage = await db.ReminderMessages.FindAsync(id);
            if (reminderMessage == null)
            {
                return HttpNotFound();
            }
            return View(reminderMessage);
        }

        // POST: ReminderMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ReminderMessage reminderMessage = await db.ReminderMessages.FindAsync(id);
            db.ReminderMessages.Remove(reminderMessage);
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
