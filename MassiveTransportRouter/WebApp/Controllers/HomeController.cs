using MTR.BusinessLogic.DataManager;
using MTR.DataAccess.Helpers;
using MTR.WebApp.Common.ViewModels;
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

            DbDataManager.GetRoute();

            return View();
        }

        public ActionResult TestMap()
        {
            var StopsToShow = new List<VMDL_Stop>();
            var allStops = DbDataManager.GetAllStops();
            StopsToShow.Add(allStops.First(l => l.StopName.Contains("Nyugati")));
            StopsToShow.Add(allStops.First(l => l.StopName.Contains("Oktogon")));
            StopsToShow.Add(allStops.First(l => l.StopName.Contains("Blaha Lujza tér")));
            StopsToShow.Add(allStops.First(l => l.StopName.Contains("Petőfi híd")));
            StopsToShow.Add(allStops.First(l => l.StopName.Contains("Budafoki út")));

            ViewBag.Message = "Google Maps teszt";
            Utility.SetupCulture();
            return View(StopsToShow);
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
