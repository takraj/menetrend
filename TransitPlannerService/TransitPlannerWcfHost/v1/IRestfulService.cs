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
        List<TransitStop> GetAllStops();

        [OperationContract]
        [WebGet]
        List<TransitStop> GetStops(string filter);

        [OperationContract]
        [WebGet]
        TransitStop GetStop(int id);

        [OperationContract]
        [WebInvoke]
        TransitPlan GetPlan(TransitPlanRequestParameters parameters);

        [OperationContract]
        [WebGet]
        TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute);
    }
}
