using MTR.WebApp.Classes;
using MTR.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MTR.WebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            String BasePath = HttpRuntime.AppDomainAppPath + "budapest_gtfs/";
            GtfsDatabase.agencies.AddRange(GtfsReader.Read<Agency>(BasePath + Agency.SourceFilename));
            GtfsDatabase.calendar.AddRange(GtfsReader.Read<MTR.WebApp.Models.Calendar>(BasePath + MTR.WebApp.Models.Calendar.SourceFilename));
            GtfsDatabase.calendar_dates.AddRange(GtfsReader.Read<CalendarDate>(BasePath + CalendarDate.SourceFilename));
            GtfsDatabase.routes.AddRange(GtfsReader.Read<MTR.WebApp.Models.Route>(BasePath + MTR.WebApp.Models.Route.SourceFilename));
            GtfsDatabase.shapes.AddRange(GtfsReader.Read<Shape>(BasePath + Shape.SourceFilename));
            GtfsDatabase.stop_times.AddRange(GtfsReader.Read<StopTime>(BasePath + StopTime.SourceFilename));
            GtfsDatabase.stops.AddRange(GtfsReader.Read<Stop>(BasePath + Stop.SourceFilename));
            GtfsDatabase.trips.AddRange(GtfsReader.Read<Trip>(BasePath + Trip.SourceFilename));
        }
    }
}