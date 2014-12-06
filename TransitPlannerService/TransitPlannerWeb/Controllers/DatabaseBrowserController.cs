using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransitPlannerWeb.Controllers
{
    public class DatabaseBrowserController : Controller
    {
        //
        // GET: /DatabaseBrowser/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BrowseRoutesOfStop()
        {
            return View();
        }

        public ActionResult BrowseStopsOfRoute()
        {
            return View();
        }

        public ActionResult BrowseTimetable()
        {
            return View();
        }

        /*
         * Actions without a View: redirects to an action with a view
         */

        [HttpPost]
        public ActionResult SetupBrowser()
        {
            return RedirectToAction("Index");
        }

        public ActionResult SetDirection()
        {
            return RedirectToAction("Index");
        }

    }
}
