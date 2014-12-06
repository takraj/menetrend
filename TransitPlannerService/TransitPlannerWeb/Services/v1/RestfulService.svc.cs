using System;
using System.Collections.Generic;
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
                trip_delays = db.TripDelays
                    .Where(d => d.When.Date.Equals(parameters.when.AsDateTime))
                    .Select(d => new IntegerPair { key = d.TripId, value = d.DelayInMinutes }).ToList();
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
                walking_speed = Int32.Parse(settings["WALKING_SPEED"])
            };

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
