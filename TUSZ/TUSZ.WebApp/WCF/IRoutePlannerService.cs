using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TUSZ.Common.ViewModels;

namespace TUSZ.WebApp.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRoutePlannerService" in both code and config file together.
    [ServiceContract]
    public interface IRoutePlannerService
    {
        [OperationContract]
        List<VM_Stop> GetStops(string name);

        [OperationContract]
        VM_PlanResult Plan(int fromStopIdx, int toStopIdx);
    }
}
