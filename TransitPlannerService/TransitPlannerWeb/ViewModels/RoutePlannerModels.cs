using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransitPlannerWeb.ViewModels
{
    public class VM_Stop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class VM_Route
    {
        public string ShortName { get; set; }
        public string Direction { get; set; }
        public string Type { get; set; }
    }

    public class PlannerInputsModel : BaseModel
    {
        public List<VM_Stop> Stops { get; set; }

        public string FromStop { get; set; }
        public string ToStop { get; set; }

        public JsDateTime SelectedDateTime { get; set; }
        public JsDateTime MinDate { get; set; }
        public JsDateTime MaxDate { get; set; }

        public HashSet<string> EnabledRouteTypes { get; set; }
        public bool WheelchairSupport { get; set; }
        public string WalkingSpeedCategory { get; set; }
        public string MaxWaitingTimeCategory { get; set; }
    }

    public class PlanModel : BaseModel
    {
        public DateTime PlannedStartTime { get; set; }

        public string FirstActionTime { get; set; }
        public string LastActionTime { get; set; }

        public string UsedAlgorithm { get; set; }
        public int CalculationTime { get; set; }
        
        public int RouteLengthTime { get; set; }
        public double RouteLengthKm { get; set; }
        public int RouteLengthStops { get; set; }

        public List<Section> Sections { get; set; }

        public class Step
        {
            public string When { get; set; }
            public VM_Stop Stop { get; set; }
        }

        public class Section
        {
            public bool IsWalking { get; set; }
            public VM_Route RouteInfo { get; set; }
            public List<Step> Steps { get; set; }
            public RouteBadgeModel SectionBadge { get; set; }
        }
    }

    public class MakePlanModel : BaseModel
    {
        public PlannerInputsModel Inputs { get; set; }
        public PlanModel Plan { get; set; }
    }
}