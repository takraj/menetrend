using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TransitPlannerContracts;

namespace TransitPlannerWcfHost.v1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISoapService" in both code and config file together.
    [ServiceContract]
    public interface ISoapService
    {
        [OperationContract]
        List<TransitStop> GetAllStops();

        [OperationContract]
        List<TransitStop> GetStops(string filter);

        [OperationContract]
        TransitStop GetStop(int id);

        [OperationContract]
        TransitPlan GetPlan(TransitPlanRequestParameters parameters);

        [OperationContract]
        TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute);
    }
}
