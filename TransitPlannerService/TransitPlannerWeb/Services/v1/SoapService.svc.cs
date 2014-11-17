using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TransitPlannerContracts;
using TransitPlannerUtilityLibrary;
using TransitPlannerWebContracts;

namespace TransitPlannerWeb.Services.v1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SoapService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SoapService.svc or SoapService.svc.cs at the Solution Explorer and start debugging.
    public class SoapService : ISoapService
    {
        private RestfulCoreService CreateClient()
        {
            // -- TODO: Adatbázisból lekérni a címeket és terheléselosztást csinálni --
            return new RestfulCoreService("http://localhost");
        }

        public IList<TransitStop> GetAllStops()
        {
            var coreSvc = CreateClient();
            return coreSvc.GetAllStops().Result;
        }

        public IList<TransitStop> GetStops(string filter)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetStops(filter).Result;
        }

        public TransitStopInfo GetStop(int id)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetStop(id).Result;
        }

        public IList<TransitRoute> GetRoutes(string filter)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetRoutes(filter).Result;
        }

        public TransitRoute GetRoute(int id)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetRoute(id).Result;
        }

        public TransitMetadata GetMetadata()
        {
            var coreSvc = CreateClient();
            return coreSvc.GetMetadata().Result;
        }

        public IList<TransitSequenceGroup> GetSchedule(int route_id, TransitDate when)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetSchedule(route_id, when).Result;
        }

        public IList<TransitSequenceInfo> GetSequences(int route_id, TransitDate when)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetSequences(route_id, when).Result;
        }

        public IList<TransitSequenceElement> GetSequence(int id)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetSequence(id).Result;
        }

        public TransitPlan GetPlan(WebTransitPlanRequestParameters parameters)
        {
            var coreSvc = CreateClient();

            var coreParameters = new TransitPlanRequestParameters
            {
                from = parameters.from,
                to = parameters.to,
                when = parameters.when,
                max_waiting_time = parameters.max_waiting_time,
                needs_wheelchair_support = parameters.needs_wheelchair_support,

                // -- TODO: Adatbázisból lekérdezni a paramétereket --
                disabled_route_ids = new List<int>(), // TODO
                get_on_off_time = 1, // TODO
                trip_delays = new List<IntegerPair>(), // TODO
                use_algorithm = "AStar", // TODO
                walking_speed = 1 // TODO
            };

            return coreSvc.GetPlan(coreParameters).Result;
        }

        public TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
        {
            var coreSvc = CreateClient();
            return coreSvc.GetSimplePlan(from, to, year, month, day, hour, minute).Result;
        }

        public void SendTroubleReport(SendTroubleReportRequest trouble_report_parameters)
        {
            throw new NotImplementedException(); // TODO: adatbázisba beszúrni
        }
    }
}
