using GTFSConverter.CRGTFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter
{
    class RGTFSComapcter
    {
        private CompactGTFSDB db;

        public RGTFSComapcter(CompactGTFSDB db)
        {
            this.db = db;
        }

        public TransitDB CreateReferencedDB()
        {
            return null;
        }
    }
}
