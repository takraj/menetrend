using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;
using TUSZ.GRAFIT.Graph;

namespace TUSZ.GRAFIT.Pathfinder
{
    public interface IPathfinder
    {
        List<Instruction> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now);
    }
}
