using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public interface IPathfinder
    {
        List<Action> CalculateShortestRoute(Stop sourceStop, Stop destinationStop, DateTime now);
    }
}
