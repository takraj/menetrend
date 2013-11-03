using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TUSZ.Common.ViewModels;
using TUSZ.WebApp.Models;

namespace TUSZ.WebApp.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RoutePlannerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RoutePlannerService.svc or RoutePlannerService.svc.cs at the Solution Explorer and start debugging.
    public class RoutePlannerService : IRoutePlannerService
    {
        public List<VM_Stop> GetStops(string name)
        {
            return RoutePlannerSingleton.Instance.Stops.Where(s => s.name.Contains(name)).ToList();
        }

        public VM_PlanResult Plan(int fromStopIdx, int toStopIdx)
        {
            try
            {
                var now = DateTime.Now;
                var result = RoutePlannerSingleton.Instance.Plan(fromStopIdx, toStopIdx, new DateTime(2013, 03, 01, now.TimeOfDay.Hours, now.TimeOfDay.Minutes, now.TimeOfDay.Seconds), "AgressiveParallelAStar");
                return new VM_PlanResult
                {
                    isFailed = false,
                    failMessage = "",
                    plan = result
                };
            }
            catch (Exception e)
            {
                return new VM_PlanResult
                {
                    isFailed = true,
                    failMessage = e.Message,
                    plan = null
                };
            }
        }
    }
}
