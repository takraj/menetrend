using MTR.Common.Gtfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Common.POCO
{
    public class Route
    {
        public Int32 DbId { get; set; }
        public String OriginalId { get; set; }
        public String RouteShortName { get; set; }
        public String RouteDescription { get; set; }
        public E_RouteType RouteType { get; set; }
        public String RouteColor { get; set; }
        public String RouteTextColor { get; set; }
        public int AgencyId { get; set; }
    }
}
