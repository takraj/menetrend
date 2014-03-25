using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.PathfinderCore
{
    public interface IPathfinder<N, C, S>
        where N : INode<N, C>
        where C : IComparable
        where S : IPathfinderState<N, C>
    {
        IEnumerable<KeyValuePair<N, C>> GetPath(S state);
    }
}
