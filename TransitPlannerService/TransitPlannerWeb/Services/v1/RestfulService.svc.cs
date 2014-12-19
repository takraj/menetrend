using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TransitPlannerContracts;
using TransitPlannerUtilityLibrary;
using TransitPlannerWeb.Models;
using TransitPlannerWebContracts;

namespace TransitPlannerWeb.Services.v1
{
    public class RestfulService : IRestfulService
    {
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

        public IList<TransitStop> GetAllStops()
        {
            var coreSvc = CreateClient();
            return coreSvc.GetAllStops();
        }

        public IList<TransitStop> GetStops(string filter)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetStops(filter);
        }

        public TransitStopInfo GetStop(int id)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetStop(id);
        }

        public IList<TransitRoute> GetRoutes(string filter)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetRoutes(filter);
        }

        public TransitRoute GetRoute(int id)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetRoute(id);
        }

        public TransitMetadata GetMetadata()
        {
            var coreSvc = CreateClient();
            return coreSvc.GetMetadata();
        }

        public IList<TransitSequenceGroup> GetSchedule(int route_id, int year, int month, int day)
        {
            var coreSvc = CreateClient();
            var when = new TransitDate
            {
                year = year,
                month = month,
                day = day
            };
            return coreSvc.GetSchedule(route_id, when);
        }

        public IList<TransitSequenceInfo> GetSequences(int route_id, int year, int month, int day)
        {
            var coreSvc = CreateClient();
            var when = new TransitDate
            {
                year = year,
                month = month,
                day = day
            };
            return coreSvc.GetSequences(route_id, when);
        }

        public IList<TransitSequenceElement> GetSequence(int id)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetSequence(id);
        }

        public TransitPlan GetPlan(WebTransitPlanRequestParameters parameters)
        {
            var coreSvc = CreateClient();

            var settings = new Dictionary<string, string>();
            var disabled_route_ids = new List<int>();
            var trip_delays = new List<IntegerPair>();

            using (var db = new AdminContext())
            {
                settings = db.Settings.ToDictionary(s => s.Key, s => s.Value);
                disabled_route_ids = db.DisabledRoutes.Select(id => id.RouteId).ToList();
                var delays_for_current_date = db.TripDelays.Where(d =>
                    d.When.Year == parameters.when.year &&
                    d.When.Month == parameters.when.month &&
                    d.When.Day == parameters.when.day
                );
                trip_delays = delays_for_current_date.Select(d => new IntegerPair { key = d.TripId, value = d.DelayInMinutes }).ToList();
            }

            var allRoutes = coreSvc.GetRoutes("");

            foreach (var route in allRoutes)
            {
                if ((route.RouteType == 0) && parameters.disabled_route_types.Contains(0) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
                if ((route.RouteType == 1) && parameters.disabled_route_types.Contains(1) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
                if ((route.RouteType == 2) && parameters.disabled_route_types.Contains(2) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
                if ((route.RouteType == 3) && parameters.disabled_route_types.Contains(3) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
                if ((route.RouteType == 4) && parameters.disabled_route_types.Contains(4) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
                if ((route.RouteType == 5) && parameters.disabled_route_types.Contains(5) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
                if ((route.RouteType == 6) && parameters.disabled_route_types.Contains(6) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
                if ((route.RouteType == 7) && parameters.disabled_route_types.Contains(7) && (!disabled_route_ids.Contains(route.id)))
                {
                    disabled_route_ids.Add(route.id);
                }
            }

            var coreParameters = new TransitPlanRequestParameters
            {
                from = parameters.from,
                to = parameters.to,
                when = parameters.when,
                max_waiting_time = parameters.max_waiting_time,
                needs_wheelchair_support = parameters.needs_wheelchair_support,

                trip_delays = trip_delays,
                disabled_route_ids = disabled_route_ids,
                use_algorithm = settings["ALGORITHM"],
                get_on_off_time = Int32.Parse(settings["GET_ON_OFF_TIME"]),
                walking_speed = double.Parse(settings["NORMAL_WALKING_SPEED"], CultureInfo.InvariantCulture)
            };

            if (parameters.walking_speed_category == 1)
            {
                // slow
                coreParameters.walking_speed = double.Parse(settings["SLOW_WALKING_SPEED"], CultureInfo.InvariantCulture);
            }

            if (parameters.walking_speed_category == 2)
            {
                // fast
                coreParameters.walking_speed = double.Parse(settings["FAST_WALKING_SPEED"], CultureInfo.InvariantCulture);
            }

            return coreSvc.GetPlan(coreParameters);
        }

        public TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetSimplePlan(from, to, year, month, day, hour, minute);
        }

        public void SendTroubleReport(SendTroubleReportRequest trouble_report_parameters)
        {
            var coreSvc = CreateClient();

            var report_entity = new TroubleReport
            {
                Category = trouble_report_parameters.category,
                Created = trouble_report_parameters.timestamp.AsDateTime,
                Message = trouble_report_parameters.description,
                Received = DateTime.Now,
                FirstRead = null,
                Latitude = null,
                Longitude = null,
                StopId = null
            };

            if (trouble_report_parameters.has_location_data)
            {
                report_entity.Latitude = trouble_report_parameters.latitude;
                report_entity.Longitude = trouble_report_parameters.longitude;

                var nearest_stop = coreSvc.GetNearestStop(report_entity.Latitude ?? 0, report_entity.Longitude ?? 0);
                report_entity.StopId = nearest_stop.id;
            }

            using (var db = new AdminContext())
            {
                db.TroubleReports.Add(report_entity);
                db.SaveChanges();
            }
        }
    }
}
