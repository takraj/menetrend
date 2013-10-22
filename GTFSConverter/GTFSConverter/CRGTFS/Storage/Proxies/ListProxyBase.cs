using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Storage.Proxies
{
    public abstract class ListProxyBase<T> : IEnumerable<T>
    {
        protected IStorageManager storageManager;
        protected int count;

        public int Count
        {
            get { return count; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public abstract IEnumerator<T> GetEnumerator();
    }
}
