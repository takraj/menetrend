using System;
using System.Collections.Generic;
using System.Globalization;
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
            var vm = new ServerSettingsAdminModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Username = WebSecurity.CurrentUserName,
                OtherParametersModel = new SetupOtherParametersAdminModel
                {
                    Algorithms = new List<SetupOtherParametersAdminModel.Algorithm>(),
                    WalkingSpeeds = new SetupOtherParametersAdminModel.WakingSpeedSettings()
                },
                LoadBalancersModel = new LoadBalancersAdminModel
                {
                    LoadBalancers = new List<LoadBalancersAdminModel.LoadBalancer>()
                }
            };

            vm.OtherParametersModel.Algorithms.Add(new SetupOtherParametersAdminModel.Algorithm
                {
                    Name = "AStar",
                    IsSelected = false
                });

            vm.OtherParametersModel.Algorithms.Add(new SetupOtherParametersAdminModel.Algorithm
            {
                Name = "Dijkstra",
                IsSelected = false
            });

            using (var context = new AdminContext())
            {
                var settings = context.Settings;

                foreach (var setting in settings)
                {
                    if (setting.Key == "ALGORITHM")
                    {
                        vm.OtherParametersModel.Algorithms.Single(v => v.Name == setting.Key).IsSelected = true;
                    }

                    if (setting.Key == "GET_ON_OFF_TIME")
                    {
                        vm.OtherParametersModel.GetOnOffTime = int.Parse(setting.Value);
                    }

                    if (setting.Key == "NORMAL_WALKING_SPEED")
                    {
                        vm.OtherParametersModel.WalkingSpeeds.Normal = double.Parse(setting.Value, CultureInfo.InvariantCulture);
                    }

                    if (setting.Key == "FAST_WALKING_SPEED")
                    {
                        vm.OtherParametersModel.WalkingSpeeds.Fast = double.Parse(setting.Value, CultureInfo.InvariantCulture);
                    }

                    if (setting.Key == "SLOW_WALKING_SPEED")
                    {
                        vm.OtherParametersModel.WalkingSpeeds.Slow = double.Parse(setting.Value, CultureInfo.InvariantCulture);
                    }
                }

                var loadBalancers = context.CoreServices;

                foreach (var lb in loadBalancers)
                {
                    vm.LoadBalancersModel.LoadBalancers.Add(new LoadBalancersAdminModel.LoadBalancer
                    {
                        Id = lb.Id,
                        Name = lb.Name,
                        Url = lb.BaseAddress,
                        Weight = lb.Weight
                    });
                }
            }

            return View(vm);
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
            using (var context = new AdminContext())
            {
                var lb_to_delete = context.CoreServices.Single(c => c.Id == id);
                context.CoreServices.Remove(lb_to_delete);
            }

            return RedirectToAction("ServerSettings");
        }

        [HttpPost]
        public ActionResult AddLoadBalancer(string name, string url, int weight)
        {
            using (var context = new AdminContext())
            {
                var lb_to_add = new CoreService
                {
                    Name = name,
                    Description = "",
                    BaseAddress = url,
                    Weight = weight
                };
                context.CoreServices.Add(lb_to_add);
                context.SaveChanges();
            }

            return RedirectToAction("ServerSettings");
        }

        [HttpPost]
        public ActionResult SetOtherSettings(string algorithm, string normal_walkspeed, string slow_walkspeed, string fast_walkspeed, int getonoff_time)
        {
            using (var context = new AdminContext())
            {
                context.Settings.Single(s => s.Key == "ALGORITHM").Value = algorithm;
                context.Settings.Single(s => s.Key == "GET_ON_OFF_TIME").Value = getonoff_time.ToString();
                context.Settings.Single(s => s.Key == "NORMAL_WALKING_SPEED").Value = normal_walkspeed;
                context.Settings.Single(s => s.Key == "FAST_WALKING_SPEED").Value = fast_walkspeed;
                context.Settings.Single(s => s.Key == "SLOW_WALKING_SPEED").Value = slow_walkspeed;
                context.SaveChanges();
            }

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
