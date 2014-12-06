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

    public class ReportsAdminModel : AdminBaseModel
    {
        public List<Report> Reports { get; set; }

        public class Report
        {
            public int Id { get; set; }
            public DateTime Created { get; set; }
            public DateTime Received { get; set; }
            public bool WasRead { get; set; }
            public DateTime Read { get; set; }
            public string Category { get; set; }
            public string StopName { get; set; }
        }
    }

    public class LoadBalancersAdminModel : AdminBaseModel
    {
        public List<LoadBalancer> LoadBalancers { get; set; }

        public class LoadBalancer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string  Url { get; set; }
            public int Weight { get; set; }
        }
    }

    public class SetupOtherParametersAdminModel : AdminBaseModel
    {
        public List<Algorithm> Algorithms { get; set; }
        public WakingSpeedSettings WalkingSpeeds { get; set; }
        public int GetOnOffTime { get; set; }

        public class Algorithm
        {
            public string Name { get; set; }
            public bool IsSelected { get; set; }
        }

        public class WakingSpeedSettings
        {
            public double Fast { get; set; }
            public double Normal { get; set; }
            public double Slow { get; set; }
        }
    }

    public class ServerSettingsAdminModel : AdminBaseModel
    {
        public LoadBalancersAdminModel LoadBalancersModel { get; set; }
        public SetupOtherParametersAdminModel OtherParametersModel { get; set; }
    }

    public class TripDelaysAdminModel : AdminBaseModel
    {
        public Dictionary<string, List<string>> RouteTree { get; set; }
        public List<TripDelay> Delays { get; set; }

        public int? SelectedRoute { get; set; }

        public JsDateTime SelectedDate { get; set; }
        public JsDateTime DatabaseMinDate { get; set; }
        public JsDateTime DatabaseMaxDate { get; set; }

        public class TripDelay
        {
            public int TripId { get; set; }
            public string TripDirection { get; set; }
            public int CountOfStops { get; set; }
            public string StartTime { get; set; }
            public int Amount { get; set; }
        }

        public class RouteTreeItem
        {
            public int RouteId { get; set; }
            public string RouteName { get; set; }
        }
    }
}