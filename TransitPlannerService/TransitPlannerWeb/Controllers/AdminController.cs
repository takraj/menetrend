using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransitPlannerWeb.Filters;
using TransitPlannerWeb.Models;
using TransitPlannerWeb.ViewModels;
using WebMatrix.WebData;

namespace TransitPlannerWeb.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View(new AdminBaseModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Username = WebSecurity.CurrentUserName
            });
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new BaseModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            if (WebSecurity.Login(username, password, persistCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            return View(new BaseModel
            {
                ErrorCode = TuszErrorCode.LOGIN_ERROR
            });
        }

        public ActionResult Reports()
        {
            return View();
        }

        public ActionResult ReportDetails(int id)
        {
            return View();
        }

        public ActionResult ServerSettings()
        {
            return View();
        }

        public ActionResult DisabledRoutes()
        {
            return View();
        }

        public ActionResult TripDelays()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TripDelays(int year, int month, int day, int route_id)
        {
            return View();
        }

        /*
         * Actions without a View: redirects to an action with a view
         */

        [HttpPost]
        public ActionResult Logout()
        {
            WebSecurity.Logout();
            return RedirectToAction("Login");
        }

        public ActionResult DeleteLoadBalancer(int id)
        {
            return RedirectToAction("ServerSettings");
        }

        [HttpPost]
        public ActionResult AddLoadBalancer(string name, string url, int weight)
        {
            return RedirectToAction("ServerSettings");
        }

        [HttpPost]
        public ActionResult SetOtherSettings(string algorithm, string normal_walkspeed, string slow_walkspeed, string fast_walkspeed, int getonoff_time)
        {
            return RedirectToAction("ServerSettings");
        }

        public ActionResult DisableRoute(int id)
        {
            return RedirectToAction("DisabledRoutes");
        }

        public ActionResult EnableRoute(int id)
        {
            return RedirectToAction("DisabledRoutes");
        }

        public ActionResult DeleteReport(int id)
        {
            return RedirectToAction("Reports");
        }

        [HttpPost]
        public ActionResult SetTripDelay()
        {
            return RedirectToAction("TripDelays");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }
    }
}
