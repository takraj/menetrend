using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Storage.Proxies
{
    public class ShapeVectorListProxy : ListProxyBase<ShapeVector>
    {
        public ShapeVectorListProxy(IStorageManager storageManager, int countOfShapes)
        {
            this.storageManager = storageManager;
            this.count = countOfShapes;
        }

        override public IEnumerator<ShapeVector> GetEnumerator()
        {
            return storageManager.CreateShapeVectorEnumerator();
        }

        public ShapeVector this[int i]
        {
            get { return storageManager.GetShape(i); }
        }
    }
}
