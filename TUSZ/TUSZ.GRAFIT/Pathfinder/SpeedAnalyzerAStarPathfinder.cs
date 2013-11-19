using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.GRAFIT.Graph;

namespace TUSZ.GRAFIT.Pathfinder
{
    public class SpeedAnalyzerAStarPathfinder : AStarPathfinder
    {
        public SpeedAnalyzerAStarPathfinder(TransitGraph graph, int[] stopDistances, int fScale = 2000)
            : base(graph, stopDistances, fScale) { }

        protected override long fValue(DynamicNode node, Common.GRAFIT.Stop sourceStop, Common.GRAFIT.Stop destinationStop, DateTime epoch)
        {
            var effectiveDistanceSoFar = this.stopDistances[sourceStop.idx] - this.stopDistances[node.stop.idx];
            var timeSoFar = (node.currentTime - epoch).TotalSeconds;
            var effectiveSpeedSoFar = effectiveDistanceSoFar / timeSoFar;

            var effectiveDistanceLeft = this.stopDistances[node.stop.idx];
            var estimatedTotalTime = (effectiveDistanceSoFar + effectiveDistanceLeft) / effectiveSpeedSoFar;

            return (long)estimatedTotalTime;
        }
    }
}
