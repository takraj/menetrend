using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Storage.Proxies
{
    public class HeadsignListProxy : ListProxyBase<string>
    {
        public HeadsignListProxy(IStorageManager storageManager, int countOfHeadsigns)
        {
            this.storageManager = storageManager;
            this.count = countOfHeadsigns;
        }

        override public IEnumerator<string> GetEnumerator()
        {
            return storageManager.CreateHeadsignEnumerator();
        }

        public string this[int i]
        {
            get { return storageManager.GetHeadsign(i); }
        }
    }
}
