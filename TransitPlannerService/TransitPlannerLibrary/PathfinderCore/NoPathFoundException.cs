using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.PathfinderCore
{
    public class NoPathFoundException : Exception
    {
        public NoPathFoundException() : base() { }
    }
}
