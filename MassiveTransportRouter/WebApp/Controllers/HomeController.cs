using MTR.BusinessLogic.DataManager;
using MTR.BusinessLogic.Pathfinder;
using MTR.DataAccess.Helpers;
using MTR.WebApp.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MTR.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult TestMap(string stopFrom = "", string stopTo = "")
        {
            var rnd = new Random();
            var src = rnd.Next(100, 5000);
            var dst = rnd.Next(100, 5000);
            var allStops = DbDataManager.GetAllStops();

            if (stopFrom != "" && stopTo != "")
            {
                try
                {
                    src = allStops.First(s => s.StopName == stopFrom).StopId;
                    dst = allStops.First(s => s.StopName == stopTo).StopId;
                    ViewBag.Message = "";
                }
                catch
                {
                    ViewBag.Message = "Random útvonal";
                }
            }

            var now = DateTime.Now.TimeOfDay;
            //var instructionsToShow = PathfinderManager.GetRoute(src, dst, new DateTime(2013, 03, 01, now.Hours, now.Minutes, now.Seconds));
            var instructionsToShow = PathfinderManager.GetRoute(src, dst, DateTime.Now);
            ViewBag.StopNames = allStops.Select(s => s.StopName).Distinct().ToArray();
            
            Utility.SetupCulture();
            return View(instructionsToShow);
        }

        public ActionResult StopGroups()
        {
            var allStops = DbDataManager.GetAllStops();
            var stopGroups = DbDataManager.GetStopGroups(50);

            ViewBag.Message = "Stop Groups teszt: " + stopGroups.Count + " / " + allStops.Count;
            return View(stopGroups);
        }
    }
}
