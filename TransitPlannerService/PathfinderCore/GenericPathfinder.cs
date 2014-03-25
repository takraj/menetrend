using TransitPlannerLibrary.PriorityQueues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransitPlannerLibrary.PathfinderCore
{
    public class GenericPathfinder<N, C, S, Q> : IPathfinder<N, C, S>
        where N : INode<N, C>
        where C : IComparable
        where S : IPathfinderState<N, C>
        where Q : IPriorityQueue<N>, new()
    {
        public IEnumerable<KeyValuePair<N, C>> GetPath(S state)
        {
            var q = new Q();
            q.AddWithPriority(state.StartNode, state.GetCost(state.StartNode));

            while (!q.IsEmpty())
            {
                state.CurrentNode = q.ExtractMin();

                if (state.IsAtTargetNode)
                {
                    break;
                }

                if (state.IsCurrentNodeClosed)
                {
                    continue;
                }

                state.SetClosed(state.CurrentNode);

                var neighbours = state.CurrentNode.GetNeighbours();
                foreach (var neighbour in neighbours)
                {
                    var v = neighbour.Key;
                    var vCost = neighbour.Value;

                    if ((!state.IsCostKnown(v)) || (state.GetCost(v).CompareTo(vCost) > 0))
                    {
                        state.SetCost(v, vCost);
                        state.SetPrevious(v, state.CurrentNode);
                        q.DecreasePriority(v, state.GetPriority(v));
                    }
                }
            }

            try
            {
                return this.Traceback(state).Reverse().ToArray();
            }
            catch (NoPreviousNodeException)
            {
                throw new NoPathFoundException();
            }
        }

        public IEnumerable<KeyValuePair<N, C>> Traceback(IPathfinderState<N, C> state)
        {
            var currentNode = state.TargetNode;

            while (!currentNode.Equals(state.StartNode))
            {
                yield return new KeyValuePair<N, C>(currentNode, state.GetCost(currentNode));
                currentNode = state.GetPrevious(currentNode);
            }

            yield return new KeyValuePair<N, C>(state.StartNode, state.GetCost(state.StartNode));
        }
    }
}
