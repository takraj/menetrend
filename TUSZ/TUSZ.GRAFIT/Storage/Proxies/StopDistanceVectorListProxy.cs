using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSZ.GRAFIT.Storage.Proxies
{
    public class StopDistanceVectorListProxy : ListProxyBase<int[]>
    {
        public StopDistanceVectorListProxy(IStorageManager storageManager, int countOfStops)
        {
            this.storageManager = storageManager;
            this.count = countOfStops;
        }

        override public IEnumerator<int[]> GetEnumerator()
        {
            return storageManager.CreateStopDistanceVectorEnumerator();
        }

        public int[] this[int i]
        {
            get { return storageManager.GetStopDistanceVector(i); }
        }
    }
}
