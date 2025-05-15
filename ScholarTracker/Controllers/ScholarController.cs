using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScholarTracker.Controllers
{
    public class ScholarController : Controller
    {
        // GET: Scholar
        public ActionResult Index()
		{
			return RedirectToAction("Index", "Student");
		}
        public ActionResult ProfileodPhDScholar()
        {
            return View();
        }

        public ActionResult courseWD()
		{
			return RedirectToAction("courseWD", "Course");
		}
        public ActionResult svD()
		{
			return RedirectToAction("Index", "Supervisor");
		}

        public ActionResult Objectives()
        {
            return View();
        }

        public ActionResult PublicationD()
        {
            return RedirectToAction("PublicationD", "Publication");
        }

        public ActionResult RDC()
        {
            return View();
        }

        public ActionResult PrePhD()
        {
            return View();
        }


    }
}