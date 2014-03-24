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
        protected int _stopId;

        protected FlowerNode(FlowerGraph graph, DijkstraPathfinderState state, int stopId)
            : this(graph, stopId)
        {
            SetState(state);
        }

        public FlowerNode(FlowerGraph graph, int stopId)
        {
            _graph = graph;
            _stopId = stopId;
        }

        public void SetState(DijkstraPathfinderState state)
        {
            _state = state;
        }

        public int StopId
        {
            get
            {
                return _stopId;
            }
        }

        public abstract IEnumerable<KeyValuePair<FlowerNode, DateTime>> GetNeighbours();

        public abstract object DistanceVectorKey { get; }
    }
}
