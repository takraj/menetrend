using MTR.BusinessLogic.DataManager;
using MTR.BusinessLogic.DataTransformer;
using MTR.BusinessLogic.Pathfinder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            DbDataManager.initDatabase(HttpRuntime.AppDomainAppPath + "budapest_gtfs/", HttpRuntime.AppDomainAppPath + Path.DirectorySeparatorChar);

            Parallel.Invoke(
                () => DbDataManager.LoadCache(HttpRuntime.AppDomainAppPath + Path.DirectorySeparatorChar),
                () => PathfinderManager.InitializePathfinders()
            );
        }
    }
}