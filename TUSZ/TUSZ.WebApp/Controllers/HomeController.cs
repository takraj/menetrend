using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TUSZ.WebApp.Models;

namespace TUSZ.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TestMap(string stopFrom = "", string stopTo = "", string algo = "AgressiveParallelAStar")
        {
            var rnd = new Random();
            var src = rnd.Next(100, 5000);
            var dst = rnd.Next(100, 5000);
            var allStops = RoutePlannerSingleton.Instance.Stops;

            if (stopFrom != "" && stopTo != "")
            {
                try
                {
                    src = allStops.First(s => s.name == stopFrom).id;
                    dst = allStops.First(s => s.name == stopTo).id;
                    ViewBag.Message = "";
                }
                catch
                {
                    ViewBag.Message = "Random útvonal";
                }
            }

            var now = DateTime.Now;

            var plan = RoutePlannerSingleton.Instance.Plan(src, dst, now, algo);
            //var plan = RoutePlannerSingleton.Instance.Plan(src, dst, new DateTime(2013, 11, 5, 12, 15, 0), algo);
            //var plan RoutePlannerSingleton.Instance.Plan(src, dst, DateTime.Now, algo);
            ViewBag.StopNames = allStops.Select(s => s.name).Distinct().ToArray();

            Utility.SetupCulture();
            return View(plan);
        }
    }
}
