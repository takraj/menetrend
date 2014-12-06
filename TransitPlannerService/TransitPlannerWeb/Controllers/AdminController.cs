using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransitPlannerContracts;
using TransitPlannerUtilityLibrary;
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
            var vm = new ReportsAdminModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Username = WebSecurity.CurrentUserName,
                Reports = new List<ReportsAdminModel.Report>()
            };

            using (var context = new AdminContext())
            {
                RestfulCoreService coreSvc;
                try
                {
                    coreSvc = CreateClient();
                }
                catch
                {
                    vm.ErrorCode = TuszErrorCode.CORE_SERVICE_IS_UNREACHABLE;
                    vm.ErrorMessage = "Az alapszolgáltatás nem elérhető.";
                    return View("FatalError", vm);
                }

                foreach (var report in context.TroubleReports)
                {
                    string stopName = "";

                    if (report.StopId != null)
                    {
                        var stop = coreSvc.GetStop((int)report.StopId).Result;
                        stopName = String.Format("{0} #{1}", stop.stop.name, stop.stop.id);
                    }

                    vm.Reports.Add(new ReportsAdminModel.Report
                    {
                        Id = report.Id,
                        Created = report.Created,
                        Received = report.Received,
                        Read = report.FirstRead ?? new DateTime(),
                        WasRead = report.FirstRead != null,
                        StopName = stopName,
                        Category = GetReportCategoryName(report.Category)
                    });
                }
            }

            return View(vm);
        }

        public ActionResult ReportDetails(int id)
        {
            var vm = new ReportDetailsAdminModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Username = WebSecurity.CurrentUserName
            };

            using (var context = new AdminContext())
            {
                RestfulCoreService coreSvc;
                try
                {
                    coreSvc = CreateClient();
                }
                catch
                {
                    vm.ErrorCode = TuszErrorCode.CORE_SERVICE_IS_UNREACHABLE;
                    vm.ErrorMessage = "Az alapszolgáltatás nem elérhető.";
                    return View("FatalError", vm);
                }

                var report = context.TroubleReports.Single(r => r.Id == id);

                vm.Id = report.Id;
                vm.Category = GetReportCategoryName(report.Category);
                vm.Created = report.Created;
                vm.Message = report.Message;
                vm.Received = report.Received;

                if (report.StopId != null)
                {
                    var stop = coreSvc.GetStop((int)report.StopId).Result;
                    var stopName = String.Format("{0} #{1}", stop.stop.name, stop.stop.id);

                    vm.Location = new ReportDetailsAdminModel.LocationData
                    {
                        Latitude = report.Latitude ?? 0,
                        Longitude = report.Longitude ?? 0,
                        StopName = stopName
                    };
                }

                if (report.FirstRead == null)
                {
                    report.FirstRead = DateTime.Now;
                    context.SaveChanges();
                }
            }

            return View(vm);
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
                        vm.OtherParametersModel.Algorithms.Single(v => v.Name == setting.Value).IsSelected = true;
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
            var vm = new DisabledRoutesAdminModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Username = WebSecurity.CurrentUserName,
                Routes = new List<DisabledRoutesAdminModel.Route>()
            };

            using (var context = new AdminContext())
            {
                RestfulCoreService coreSvc;
                try
                {
                    coreSvc = CreateClient();
                }
                catch
                {
                    vm.ErrorCode = TuszErrorCode.CORE_SERVICE_IS_UNREACHABLE;
                    vm.ErrorMessage = "Az alapszolgáltatás nem elérhető.";
                    return View("FatalError", vm);
                }

                var allRoutes = coreSvc.GetRoutes("").Result;
                var disabledRouteIds = new HashSet<int>(context.DisabledRoutes.Select(dr => dr.RouteId));

                foreach (var route in allRoutes)
                {
                    vm.Routes.Add(new DisabledRoutesAdminModel.Route
                    {
                        Id = route.id,
                        ShortName = route.ShortName,
                        LongName = route.LongName,
                        IsDisabled = disabledRouteIds.Contains(route.id),
                        RouteType = GetRouteTypeName(route.RouteType)
                    });
                }
            }

            return View(vm);
        }

        public ActionResult TripDelays()
        {
            var vm = new TripDelaysAdminModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Username = WebSecurity.CurrentUserName,
                Delays = new List<TripDelaysAdminModel.TripDelay>(),
                RouteTree = new Dictionary<string, List<TripDelaysAdminModel.RouteTreeItem>>(),
                SelectedDate = new JsDateTime
                {
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month - 1,
                    Day = DateTime.Now.Day
                }
            };

            RestfulCoreService coreSvc;
            try
            {
                coreSvc = CreateClient();
            }
            catch
            {
                vm.ErrorCode = TuszErrorCode.CORE_SERVICE_IS_UNREACHABLE;
                vm.ErrorMessage = "Az alapszolgáltatás nem elérhető.";
                return View("FatalError", vm);
            }
            var allRoutes = coreSvc.GetRoutes("").Result;

            foreach (var route in allRoutes)
            {
                string routeTypeName = GetRouteTypeName(route.RouteType);

                if (!vm.RouteTree.ContainsKey(routeTypeName))
                {
                    vm.RouteTree[routeTypeName] = new List<TripDelaysAdminModel.RouteTreeItem>();
                }

                vm.RouteTree[routeTypeName].Add(new TripDelaysAdminModel.RouteTreeItem
                {
                    RouteId = route.id,
                    RouteName = route.ShortName
                });
            }

            var metadata = coreSvc.GetMetadata().Result;

            vm.DatabaseMinDate = new JsDateTime
            {
                Year = metadata.valid_from.year,
                Month = metadata.valid_from.month - 1,
                Day = metadata.valid_from.day
            };

            vm.DatabaseMaxDate = new JsDateTime
            {
                Year = metadata.valid_to.year,
                Month = metadata.valid_to.month - 1,
                Day = metadata.valid_to.day
            };

            vm.SelectedRoute = null;

            return View(vm);
        }

        [HttpPost]
        public ActionResult TripDelays(int year, int month, int day, int route_id)
        {
            var vm = new TripDelaysAdminModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Username = WebSecurity.CurrentUserName,
                Delays = new List<TripDelaysAdminModel.TripDelay>(),
                RouteTree = new Dictionary<string, List<TripDelaysAdminModel.RouteTreeItem>>(),
                SelectedDate = new JsDateTime
                {
                    Year = year,
                    Month = month,
                    Day = day
                },
                SelectedRoute = route_id
            };

            RestfulCoreService coreSvc;
            try
            {
                coreSvc = CreateClient();
            }
            catch
            {
                vm.ErrorCode = TuszErrorCode.CORE_SERVICE_IS_UNREACHABLE;
                vm.ErrorMessage = "Az alapszolgáltatás nem elérhető.";
                return View("FatalError", vm);
            }
            var allRoutes = coreSvc.GetRoutes("").Result;

            foreach (var route in allRoutes)
            {
                string routeTypeName = GetRouteTypeName(route.RouteType);

                if (!vm.RouteTree.ContainsKey(routeTypeName))
                {
                    vm.RouteTree[routeTypeName] = new List<TripDelaysAdminModel.RouteTreeItem>();
                }

                vm.RouteTree[routeTypeName].Add(new TripDelaysAdminModel.RouteTreeItem
                {
                    RouteId = route.id,
                    RouteName = route.ShortName
                });
            }

            var metadata = coreSvc.GetMetadata().Result;

            vm.DatabaseMinDate = new JsDateTime
            {
                Year = metadata.valid_from.year,
                Month = metadata.valid_from.month - 1,
                Day = metadata.valid_from.day
            };

            vm.DatabaseMaxDate = new JsDateTime
            {
                Year = metadata.valid_to.year,
                Month = metadata.valid_to.month - 1,
                Day = metadata.valid_to.day
            };

            using (var db = new AdminContext())
            {
                var delays = db.TripDelays.ToList();
                var schedule = coreSvc.GetSchedule(
                    vm.SelectedRoute ?? 0,
                    new TransitDate
                    {
                        year = year,
                        month = month + 1,
                        day = day
                    }
                    ).Result;

                foreach (var sequence_group in schedule)
                {
                    var sequence_info = sequence_group.sequence_info;

                    for (int i = 0; i < sequence_group.sequence_base_times.Count; i++ )
                    {
                        var base_time = sequence_group.sequence_base_times[i];
                        int trip_id = sequence_group.sequence_trip_ids[i];

                        int delay_amount = delays.Count(d => d.TripId == trip_id) == 0 ? 0 : delays.Single(d => d.TripId == trip_id).DelayInMinutes;

                        vm.Delays.Add(new TripDelaysAdminModel.TripDelay
                        {
                            TripId = trip_id,
                            TripDirection = sequence_info.last_stop.name,
                            CountOfStops = sequence_info.count_of_stops,
                            Amount = delay_amount,
                            StartTime = base_time.AsDateTime.ToString("HH:mm")
                        });
                    }
                }
            }

            return View(vm);
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
                context.SaveChanges();
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
            using (var db = new AdminContext())
            {
                if (db.DisabledRoutes.Count(dr => dr.RouteId == id) == 0)
                {
                    db.DisabledRoutes.Add(new DisabledRoute
                    {
                        RouteId = id
                    });
                    db.SaveChanges();
                }
            }

            return RedirectToAction("DisabledRoutes");
        }

        public ActionResult EnableRoute(int id)
        {
            using (var db = new AdminContext())
            {
                var dr_to_delete = db.DisabledRoutes.Single(dr => dr.RouteId == id);
                db.DisabledRoutes.Remove(dr_to_delete);
                db.SaveChanges();
            }

            return RedirectToAction("DisabledRoutes");
        }

        public ActionResult DeleteReport(int id)
        {
            using (var context = new AdminContext())
            {
                var report = context.TroubleReports.Single(tr => tr.Id == id);
                context.TroubleReports.Remove(report);
                context.SaveChanges();
            }

            return RedirectToAction("Reports");
        }

        [HttpPost]
        public ActionResult SetTripDelay(int year, int month, int day, int trip_id, int trip_delay_amount)
        {
            var when = new DateTime(year, month + 1, day);

            using (var db = new AdminContext())
            {
                if (db.TripDelays.Count(td => (td.TripId == trip_id && td.When == when)) == 0)
                {
                    db.TripDelays.Add(new TripDelay
                    {
                        TripId = trip_id,
                        When = when,
                        DelayInMinutes = trip_delay_amount
                    });
                    db.SaveChanges();
                }
            }

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

        /*
         * Helpers
         */

        private string GetReportCategoryName(int categoryId)
        {
            switch (categoryId)
            {
                case 0: return "Bűncselekmény";
                case 1: return "Baleset";
                case 2: return "Késés";
                case 3: return "Műszaki hiba";
                case 4: return "Alkalmazáshiba";
                case 5: return "Egyéb";

                default: return "Ismeretlen";
            }
        }

        private string GetRouteTypeName(int typeId)
        {
            switch (typeId)
            {
                case 0: return "Villamos";
                case 1: return "Metró";
                case 2: return "Vasút";
                case 3: return "Busz";
                case 4: return "Hajó";
                case 5: return "Függővasút";
                case 6: return "Gondola";
                case 7: return "Fogaskerekű";

                default: return "Ismeretlen";
            }
        }

        private RestfulCoreService CreateClient()
        {
            var services = new List<CoreService>();

            using (var db = new AdminContext())
            {
                var list_of_service_records = db.CoreServices.ToList();
                foreach (var service_record in list_of_service_records)
                {
                    for (int i = 0; i < service_record.Weight; i++)
                    {
                        services.Add(service_record);
                    }
                }
            }

            var rnd = new Random(DateTime.Now.Millisecond);
            int selected_service_index = rnd.Next(services.Count);
            var selected_service = services[selected_service_index];

            return new RestfulCoreService(selected_service.BaseAddress);
        }
    }
}
