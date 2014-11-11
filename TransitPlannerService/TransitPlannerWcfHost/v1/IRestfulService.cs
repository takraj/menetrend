using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TransitPlannerContracts;

namespace TransitPlannerWcfHost
{
    [ServiceContract]
    public interface IRestfulService
    {

        [OperationContract]
        [WebGet]
        IList<TransitStop> GetAllStops();

        [OperationContract]
        [WebGet]
        IList<TransitStop> GetStops(string filter);

        [OperationContract]
        [WebGet]
        TransitStopInfo GetStop(int id);

        [OperationContract]
        [WebGet]
        IList<TransitRoute> GeRoutes(string filter);

        [OperationContract]
        [WebGet]
        TransitRoute GetRoute(int id);

        [OperationContract]
        [WebGet]
        TransitMetadata GetMetadata();

        [OperationContract]
        [WebGet]
        IList<TransitSequenceGroup> GetSchedule(int route_id, TransitDate when);

        [OperationContract]
        [WebGet]
        IList<TransitSequenceInfo> GetSequences(int route_id, TransitDate when);

        [OperationContract]
        [WebGet]
        IList<TransitSequenceElement> GetSequence(int id);

        [OperationContract]
        [WebInvoke]
        TransitPlan GetPlan(TransitPlanRequestParameters parameters);

        [OperationContract]
        [WebGet]
        TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute);
    }
}
