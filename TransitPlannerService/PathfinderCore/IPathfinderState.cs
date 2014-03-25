using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.PathfinderCore
{
    public interface IPathfinderState<N, C>
        where N : INode<N, C>
        where C : IComparable
    {
        N StartNode { get; }
        N TargetNode { get; }

        N CurrentNode { get; set; }
        C CurrentCost { get; }
        bool IsAtStartNode { get; }
        bool IsAtTargetNode { get; }
        bool IsCurrentNodeClosed { get; }

        void SetClosed(N node);
        bool IsClosed(N node);

        void SetPrevious(N node, N prevNode);
        N GetPrevious(N node);

        void SetCost(N node, C cost);
        C GetCost(N node);
        bool IsCostKnown(N node);

        IComparable GetPriority(N node);
    }
}
