using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.PathfinderCore
{
    public interface INode<N, C>
        where N : INode<N, C>
        where C : IComparable
    {
        IEnumerable<KeyValuePair<N, C>> GetNeighbours();
    }
}
