using MTR.DataAccess.Classes;
using MTR.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
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

        public ActionResult TestMap()
        {
            Utility.SetupCulture();

            ViewBag.Message = "Google Maps teszt";
            var StopsToShow = new List<Stop>();
            StopsToShow.Add(GtfsDatabase.stops.First(l => l.StopName.Contains("Nyugati")));
            StopsToShow.Add(GtfsDatabase.stops.First(l => l.StopName.Contains("Oktogon")));
            StopsToShow.Add(GtfsDatabase.stops.First(l => l.StopName.Contains("Blaha")));
            StopsToShow.Add(GtfsDatabase.stops.First(l => l.StopName.Contains("Petőfi")));
            StopsToShow.Add(GtfsDatabase.stops.First(l => l.StopName.Contains("Budafoki")));
            ViewBag.Stops = StopsToShow;
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
