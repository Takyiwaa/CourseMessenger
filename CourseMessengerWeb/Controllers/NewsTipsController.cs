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
    public class NewsTipsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: NewsTips
        public async Task<ActionResult> Index()
        {
            return View(await db.NewsTips.ToListAsync());
        }

        // GET: NewsTips/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsTips newsTips = await db.NewsTips.FindAsync(id);
            if (newsTips == null)
            {
                return HttpNotFound();
            }
            return View(newsTips);
        }

        // GET: NewsTips/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsTips/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ReminderType,Status")] NewsTips newsTips)
        {
            if (ModelState.IsValid)
            {
                db.NewsTips.Add(newsTips);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(newsTips);
        }

        // GET: NewsTips/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsTips newsTips = await db.NewsTips.FindAsync(id);
            if (newsTips == null)
            {
                return HttpNotFound();
            }
            return View(newsTips);
        }

        // POST: NewsTips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ReminderType,Status")] NewsTips newsTips)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsTips).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(newsTips);
        }

        // GET: NewsTips/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsTips newsTips = await db.NewsTips.FindAsync(id);
            if (newsTips == null)
            {
                return HttpNotFound();
            }
            return View(newsTips);
        }

        // POST: NewsTips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NewsTips newsTips = await db.NewsTips.FindAsync(id);
            db.NewsTips.Remove(newsTips);
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
