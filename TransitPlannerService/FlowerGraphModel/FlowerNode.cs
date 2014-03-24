using PathfinderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerGraphModel
{
    public abstract class FlowerNode : INode<FlowerNode, DateTime>
    {
        protected FlowerGraph _graph;
        protected DijkstraPathfinderState _state;

        public FlowerNode(FlowerGraph graph, DijkstraPathfinderState state)
        {
            _graph = graph;
            _state = state;
        }

        public abstract IEnumerable<KeyValuePair<FlowerNode, DateTime>> GetNeighbours();

        public abstract object DistanceVectorKey { get; }
    }
}
