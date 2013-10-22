using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Storage.Proxies
{
    public class StopListProxy : ListProxyBase<Stop>
    {
        public StopListProxy(IStorageManager storageManager, int countOfStops)
        {
            this.storageManager = storageManager;
            this.count = countOfStops;
        }

        override public IEnumerator<Stop> GetEnumerator()
        {
            return storageManager.CreateStopEnumerator();
        }

        public Stop this[int i]
        {
            get { return storageManager.GetStop(i); }
        }
    }
}
