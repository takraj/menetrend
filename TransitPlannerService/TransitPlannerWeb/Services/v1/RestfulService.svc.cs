using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TransitPlannerContracts;
using TransitPlannerWebContracts;

namespace TransitPlannerWeb.Services.v1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RestfulService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RestfulService.svc or RestfulService.svc.cs at the Solution Explorer and start debugging.
    public class RestfulService : IRestfulService
    {
        public IList<TransitStop> GetAllStops()
        {
            throw new NotImplementedException();
        }

        public IList<TransitStop> GetStops(string filter)
        {
            throw new NotImplementedException();
        }

        public TransitStopInfo GetStop(int id)
        {
            throw new NotImplementedException();
        }

        public IList<TransitRoute> GetRoutes(string filter)
        {
            throw new NotImplementedException();
        }

        public TransitRoute GetRoute(int id)
        {
            throw new NotImplementedException();
        }

        public TransitMetadata GetMetadata()
        {
            throw new NotImplementedException();
        }

        public IList<TransitSequenceGroup> GetSchedule(int route_id, TransitDate when)
        {
            throw new NotImplementedException();
        }

        public IList<TransitSequenceInfo> GetSequences(int route_id, TransitDate when)
        {
            throw new NotImplementedException();
        }

        public IList<TransitSequenceElement> GetSequence(int id)
        {
            throw new NotImplementedException();
        }

        public TransitPlan GetPlan(WebTransitPlanRequestParameters parameters)
        {
            throw new NotImplementedException();
        }

        public TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
        {
            throw new NotImplementedException();
        }
    }
}
