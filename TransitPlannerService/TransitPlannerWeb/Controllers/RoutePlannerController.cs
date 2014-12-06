using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransitPlannerWeb.Controllers
{
    public class RoutePlannerController : Controller
    {
        //
        // GET: /RoutePlanner/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MakePlan()
        {
            return View();
        }

    }
}
