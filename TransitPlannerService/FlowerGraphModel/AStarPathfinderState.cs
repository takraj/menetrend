using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerGraphModel
{
    public class AStarPathfinderState : DijkstraPathfinderState
    {
        protected Dictionary<int, TimeSpan> _heuristicsCache;

        public AStarPathfinderState(FlowerGraph graph, FlowerNode source, FlowerNode destination, DateTime when)
            : base(source, destination, when)
        {
            _heuristicsCache = new Dictionary<int, TimeSpan>();
        }

        public override IComparable GetPriority(FlowerNode node)
        {
            throw new NotImplementedException();
        }
    }
}
