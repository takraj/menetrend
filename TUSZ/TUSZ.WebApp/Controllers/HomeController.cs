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

        public ActionResult TestMap(string stopFrom = "", string stopTo = "")
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

            var now = DateTime.Now.TimeOfDay;
            
            var plan = RoutePlannerSingleton.Instance.Plan(src, dst, new DateTime(2013, 03, 01, now.Hours, now.Minutes, now.Seconds), "AgressiveParallelAStar");
            //var instructionsToShow = PathfinderManager.GetRoute(src, dst, new DateTime(2013, 03, 01, 16, 21, now.Seconds));
            //var instructionsToShow = PathfinderManager.GetRoute(src, dst, DateTime.Now);
            ViewBag.StopNames = allStops.Select(s => s.name).Distinct().ToArray();

            Utility.SetupCulture();
            return View(plan);
        }
    }
}
