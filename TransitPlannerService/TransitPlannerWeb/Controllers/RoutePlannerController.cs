using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransitPlannerContracts;
using TransitPlannerUtilityLibrary;
using TransitPlannerWeb.Models;
using TransitPlannerWeb.ViewModels;

namespace TransitPlannerWeb.Controllers
{
    public class RoutePlannerController : Controller
    {
        //
        // GET: /RoutePlanner/

        public ActionResult Index()
        {
            var vm = new PlannerInputsModel
            {
                EnabledRouteTypes = new HashSet<string>(),
                ErrorCode = TuszErrorCode.NO_ERROR,
                FromStop = "",
                ToStop = "",
                MaxWaitingTimeCategory = "30",
                SelectedDateTime = new JsDateTime
                {
                    Year = DateTime.Now.Year,
                    Month = DateTime.Now.Month,
                    Day = DateTime.Now.Day,
                    Hour = DateTime.Now.Hour,
                    Minute = DateTime.Now.Minute
                },
                Stops = new List<VM_Stop>(),
                WheelchairSupport = false,
                WalkingSpeedCategory = "normal"
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
                return View(vm);
            }

            var allStops = coreSvc.GetAllStops();

            foreach (var stop in allStops)
            {
                vm.Stops.Add(new VM_Stop
                {
                    Id = stop.id,
                    Address = stop.street,
                    City = stop.city,
                    Latitude = stop.latitude,
                    Longitude = stop.longitude,
                    Name = stop.name + " #" + stop.id.ToString(),
                    PostalCode = stop.postal_code
                });
            }

            var metadata = coreSvc.GetMetadata();

            vm.MinDate = new JsDateTime
            {
                Year = metadata.valid_from.year,
                Month = metadata.valid_from.month - 1,
                Day = metadata.valid_from.day
            };

            vm.MaxDate = new JsDateTime
            {
                Year = metadata.valid_to.year,
                Month = metadata.valid_to.month - 1,
                Day = metadata.valid_to.day
            };

            vm.EnabledRouteTypes.Add("tram");
            vm.EnabledRouteTypes.Add("subway");
            vm.EnabledRouteTypes.Add("bus");
            vm.EnabledRouteTypes.Add("funicular");
            vm.EnabledRouteTypes.Add("rail");
            vm.EnabledRouteTypes.Add("ferry");

            return View(vm);
        }

        [HttpPost]
        public ActionResult MakePlan(
            int from_stop_id, int to_stop_id,
            int year, int month, int day, int hour, int minute,
            int tram_enabled, int subway_enabled, int rail_enabled, int bus_enabled,
            int ferry_enabled, int cable_car_enabled, int gondola_enabled, int funicular_enabled,
            int wheelchair, string walking_category, string waiting_category)
        {
            var vm = new MakePlanModel
            {
                ErrorCode = TuszErrorCode.NO_ERROR,
                Inputs = new PlannerInputsModel
                {
                    EnabledRouteTypes = new HashSet<string>(),
                    FromStop = "",
                    ToStop = "",
                    MaxWaitingTimeCategory = waiting_category,
                    SelectedDateTime = new JsDateTime
                    {
                        Year = year,
                        Month = month,
                        Day = day,
                        Hour = hour,
                        Minute = minute
                    },
                    Stops = new List<VM_Stop>(),
                    WheelchairSupport = (wheelchair == 1),
                    WalkingSpeedCategory = walking_category
                },
                Plan = new PlanModel()
            };

            RestfulCoreService coreSvc;
            try
            {
                coreSvc = CreateClient();
            }
            catch
            {
                vm.Inputs.ErrorCode = TuszErrorCode.CORE_SERVICE_IS_UNREACHABLE;
                vm.Inputs.ErrorMessage = "Az alapszolgáltatás nem elérhető.";
                return View("Index", vm.Inputs);
            }

            var allStops = coreSvc.GetAllStops();

            foreach (var stop in allStops)
            {
                vm.Inputs.Stops.Add(new VM_Stop
                {
                    Id = stop.id,
                    Address = stop.street,
                    City = stop.city,
                    Latitude = stop.latitude,
                    Longitude = stop.longitude,
                    Name = stop.name + " #" + stop.id.ToString(),
                    PostalCode = stop.postal_code
                });

                if (stop.id == from_stop_id)
                {
                    vm.Inputs.FromStop = stop.name + " #" + stop.id.ToString();
                }

                if (stop.id == to_stop_id)
                {
                    vm.Inputs.ToStop = stop.name + " #" + stop.id.ToString();
                }
            }

            var metadata = coreSvc.GetMetadata();

            vm.Inputs.MinDate = new JsDateTime
            {
                Year = metadata.valid_from.year,
                Month = metadata.valid_from.month - 1,
                Day = metadata.valid_from.day
            };

            vm.Inputs.MaxDate = new JsDateTime
            {
                Year = metadata.valid_to.year,
                Month = metadata.valid_to.month - 1,
                Day = metadata.valid_to.day
            };

            if (tram_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("tram"); }
            if (bus_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("bus"); }
            if (subway_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("subway"); }
            if (cable_car_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("cable-car"); }
            if (ferry_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("ferry"); }
            if (funicular_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("funicular"); }
            if (gondola_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("gondola"); }
            if (rail_enabled == 1) { vm.Inputs.EnabledRouteTypes.Add("rail"); }

            var settings = new Dictionary<string, string>();
            var disabled_route_ids = new List<int>();
            var trip_delays = new List<IntegerPair>();
            var when = new TransitDateTime
            {
                year = year,
                month = month + 1,
                day = day,
                hour = hour,
                minute = minute
            };

            using (var db = new AdminContext())
            {
                settings = db.Settings.ToDictionary(s => s.Key, s => s.Value);
                disabled_route_ids = db.DisabledRoutes.Select(id => id.RouteId).ToList();
                var delays_for_current_date = db.TripDelays.Where(d => 
                    d.When.Year == when.year &&
                    d.When.Month == when.month &&
                    d.When.Day == when.day
                );
                trip_delays = delays_for_current_date.Select(d => new IntegerPair { key = d.TripId, value = d.DelayInMinutes }).ToList();
            }

            double walking_speed = double.Parse(settings["NORMAL_WALKING_SPEED"], CultureInfo.InvariantCulture);

            if (walking_category == "slow")
            {
                walking_speed = double.Parse(settings["SLOW_WALKING_SPEED"], CultureInfo.InvariantCulture);
            }

            if (walking_category == "fast")
            {
                walking_speed = double.Parse(settings["FAST_WALKING_SPEED"], CultureInfo.InvariantCulture);
            }
            
            int max_waiting_time = 99999;

            if (waiting_category != "dontcare")
            {
                max_waiting_time = int.Parse(waiting_category);
            }

            var coreParameters = new TransitPlanRequestParameters
            {
                from = from_stop_id,
                to = to_stop_id,
                when = when,
                max_waiting_time = max_waiting_time,
                needs_wheelchair_support = (wheelchair == 1),
                trip_delays = trip_delays,
                disabled_route_ids = disabled_route_ids,
                use_algorithm = settings["ALGORITHM"],
                get_on_off_time = Int32.Parse(settings["GET_ON_OFF_TIME"]),
                walking_speed = walking_speed
            };

            TransitPlan planResponse = null;
            try
            {
                planResponse = coreSvc.GetPlan(coreParameters);
            }
            catch
            {
                vm.Inputs.ErrorCode = TuszErrorCode.NO_PLAN_CREATED;
                vm.Inputs.ErrorMessage = "Nincs útvonal.";
                return View("Index", vm.Inputs);
            }

            vm.Plan.CalculationTime = planResponse.plan_computation_time;
            vm.Plan.FirstActionTime = planResponse.base_time.AsDateTime.ToString("HH:mm");
            vm.Plan.LastActionTime = planResponse.end_time.AsDateTime.ToString("HH:mm");
            vm.Plan.PlannedStartTime = when.AsDateTime;
            vm.Plan.UsedAlgorithm = planResponse.algorithm;
            vm.Plan.RouteLengthTime = planResponse.route_duration;
            vm.Plan.RouteLengthKm = planResponse.route_length;
            vm.Plan.RouteLengthStops = planResponse.instructions.Select(i => i.stop.id).Distinct().Count();

            var tripIdsInOrder = new List<int>();
            var instructionsMatrix = new List<List<TransitPlanInstruction>>();
            foreach (var instruction in planResponse.instructions)
            {
                int tripId = instruction.trip_id;

                if (tripIdsInOrder.Count == 0)
                {
                    tripIdsInOrder.Add(tripId);
                    instructionsMatrix.Add(new List<TransitPlanInstruction>());
                }

                if (tripIdsInOrder.Last() != tripId)
                {
                    tripIdsInOrder.Add(tripId);
                    instructionsMatrix.Add(new List<TransitPlanInstruction>());
                }

                var lastInstructionsVector = instructionsMatrix.Last();

                lastInstructionsVector.Add(instruction);
            }

            // -------- SECTION ÉPÍTÉS --------

            vm.Plan.Sections = new List<PlanModel.Section>();

            foreach (var instructionsVector in instructionsMatrix)
            {
                var section = new PlanModel.Section
                {
                    IsWalking = instructionsVector.First().is_walking,
                    Steps = new List<PlanModel.Step>()
                };

                if (!section.IsWalking)
                {
                    var routeId = instructionsVector.First().route_id;
                    var tripId = instructionsVector.First().trip_id;

                    var route = planResponse.used_routes.First(r => r.id == routeId);
                    var trip = planResponse.used_trips.First(t => t.id == tripId);

                    var sequence = coreSvc.GetSequence(trip.sequence_id);

                    section.RouteInfo = new VM_Route
                    {
                        ShortName = route.ShortName,
                        Type = GetRouteTypeName(route.RouteType),
                        Direction = sequence.Last().stop.name
                    };

                    section.SectionBadge = new RouteBadgeModel
                    {
                        BadgeBackgroundColor = route.RouteColor,
                        BadgeLabel = route.ShortName,
                        BadgeLabelColor = route.RouteTextColor,
                        BadgeSize = 40,
                        FontSize = 16
                    };
                }

                foreach (var instruction in instructionsVector)
                {
                    var instruction_when = planResponse.base_time.AsDateTime.AddMinutes(instruction.plan_minute);
                    var step = new PlanModel.Step
                    {
                        When = instruction_when.ToString("HH:mm"),
                        Stop = new VM_Stop
                        {
                            Address = instruction.stop.street,
                            City = instruction.stop.city,
                            Id = instruction.stop.id,
                            Latitude = instruction.stop.latitude,
                            Longitude = instruction.stop.longitude,
                            Name = instruction.stop.name,
                            PostalCode = instruction.stop.postal_code
                        }
                    };
                    section.Steps.Add(step);
                }

                vm.Plan.Sections.Add(section);
            }

            return View(vm);
        }

        /*
         * Helpers
         */

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
