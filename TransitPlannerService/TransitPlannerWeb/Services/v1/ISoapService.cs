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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISoapService" in both code and config file together.
    [ServiceContract]
    public interface ISoapService
    {
        [OperationContract]
        IList<TransitStop> GetAllStops();

        [OperationContract]
        IList<TransitStop> GetStops(string filter);

        [OperationContract]
        TransitStopInfo GetStop(int id);

        [OperationContract]
        IList<TransitRoute> GetRoutes(string filter);

        [OperationContract]
        TransitRoute GetRoute(int id);

        [OperationContract]
        TransitMetadata GetMetadata();

        [OperationContract]
        IList<TransitSequenceGroup> GetSchedule(int route_id, TransitDate when);

        [OperationContract]
        IList<TransitSequenceInfo> GetSequences(int route_id, TransitDate when);

        [OperationContract]
        IList<TransitSequenceElement> GetSequence(int id);

        [OperationContract]
        TransitPlan GetPlan(WebTransitPlanRequestParameters parameters);

        [OperationContract]
        TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute);
    }
}
