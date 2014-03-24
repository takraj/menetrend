using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerGraphModel
{
    public class AStarPathfinderState : DijkstraPathfinderState
    {
        protected Dictionary<int, double> _heuristicsCache;
        protected FlowerGraph _graph;

        public AStarPathfinderState(FlowerGraph graph, FlowerNode source, FlowerNode destination, DateTime when)
            : base(source, destination, when)
        {
            _heuristicsCache = new Dictionary<int, double>();
            _graph = graph;
        }

        public override IComparable GetPriority(FlowerNode node)
        {
            if (!_heuristicsCache.ContainsKey(node.StopId))
            {
                var dstToTarget = _graph.GetDistanceBetween(node.StopId, _targetNode.StopId) * 1000.0; // km -> m
                var estimatedHours = dstToTarget / ((float)_graph.Repository.MetaInfo.MaxSpeed);

                _heuristicsCache[node.StopId] = estimatedHours;
            }

            return _distanceVector[node.DistanceVectorKey].AddHours(_heuristicsCache[node.StopId]);
        }
    }
}
