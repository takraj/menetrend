using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.WebApp.Common.ViewModels
{
    public class VMDL_RouteInstruction
    {
        public VMDL_Stop stop;
        public string routeName;
        public string routeColor;
        public string routeTextColor;
        public string timeString;
        public long timeTicks;
        public bool isTransfer;
    }
}
