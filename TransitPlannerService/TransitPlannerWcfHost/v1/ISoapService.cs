using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TransitPlannerContracts;

namespace TransitPlannerWcfHost.v1
{
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
        IList<TransitRoute> GeRoutes(string filter);

        [OperationContract]
        TransitRoute GetRoute(int id);

        [OperationContract]
        TransitMetadata GetMetadata();

        [OperationContract]
        TransitSequenceGroup GetSchedule(int route_id, TransitDate when);

        [OperationContract]
        IList<TransitSequenceInfo> GetSequences(int route_id, TransitDate when);

        [OperationContract]
        IList<TransitSequenceElement> GetSequence(int id);

        [OperationContract]
        TransitPlan GetPlan(TransitPlanRequestParameters parameters);

        [OperationContract]
        TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute);
    }
}
