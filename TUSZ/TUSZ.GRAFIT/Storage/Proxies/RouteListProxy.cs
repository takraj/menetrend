using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;

namespace TUSZ.GRAFIT.Storage.Proxies
{
    public class RouteListProxy : ListProxyBase<Route>
    {
        public RouteListProxy(IStorageManager storageManager, int countOfRoutes)
        {
            this.storageManager = storageManager;
            this.count = countOfRoutes;
        }

        override public IEnumerator<Route> GetEnumerator()
        {
            return storageManager.CreateRouteEnumerator();
        }

        public Route this[int i]
        {
            get { return storageManager.GetRoute(i); }
        }
    }
}
