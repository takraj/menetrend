using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class NoPathFoundException : Exception
    {
        private static readonly string message = "Nem találtam a beállításoknak megfelelő útvonalat \"{0}\" és \"{1}\" között ekkor: {2}.";

        public NoPathFoundException(Stop sourceStop, Stop destinationStop, DateTime when)
            : base(String.Format(message, sourceStop.ToString(), destinationStop.ToString(), when.ToString()))
        { }
    }
}
