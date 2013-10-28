using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;

namespace TUSZ.GRAFIT.Storage.Proxies
{
    public class TripListProxy : ListProxyBase<Trip>
    {
        public TripListProxy(IStorageManager storageManager, int countOfTrips)
        {
            this.storageManager = storageManager;
            this.count = countOfTrips;
        }

        override public IEnumerator<Trip> GetEnumerator()
        {
            return storageManager.CreateTripEnumerator();
        }

        public Trip this[int i]
        {
            get { return storageManager.GetTrip(i); }
        }
    }
}
