using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourseMessengerWeb.Models;

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
        public ActionResult Run()
        {

            return View();
        }
    }
}