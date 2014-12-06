using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransitPlannerWeb.ViewModels
{
    public class DisabledRoutesAdminModel : AdminBaseModel
    {
        public List<Route> Routes { get; set; }

        public class Route
        {
            public int Id { get; set; }
            public string ShortName { get; set; }
            public string LongName { get; set; }
            public string RouteType { get; set; }
            public bool IsDisabled { get; set; }
        }
    }

    public class ReportDetailsAdminModel : AdminBaseModel
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Received { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public LocationData Location { get; set; }

        public class LocationData
        {
            public string StopName { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}