using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder.Dijkstra
{
    public class FastDijkstraPathfinder : DijkstraPathfinder
    {
        private Dictionary<Thread, PriorityQueue> _priorityQueues;
        private Dictionary<Thread, HashSet<CompleteNode>> _completeSet;

        public FastDijkstraPathfinder()
        {
            Initialize();
            _priorityQueues = new Dictionary<Thread, PriorityQueue>();
            _completeSet = new Dictionary<Thread, HashSet<CompleteNode>>();
            _parallel = false;
        }

        public override void GetShortestRoute(int sourceStopId, int destinationStopId, DateTime when)
        {
            if (!_priorityQueues.ContainsKey(Thread.CurrentThread))
            {
                var storage = new BinaryHeapPriorityQueue();
                _priorityQueues.Add(Thread.CurrentThread, storage);
                _completeSet.Add(Thread.CurrentThread, new HashSet<CompleteNode>());
            }

            base.GetShortestRoute(sourceStopId, destinationStopId, when);

            if (_priorityQueues.ContainsKey(Thread.CurrentThread))
            {
                _priorityQueues.Remove(Thread.CurrentThread);
                _completeSet.Remove(Thread.CurrentThread);
            }
        }

        protected override CompleteNode GetNextCompleteNode(CompleteNode currentNode, List<Node> incompleteNodes, DateTime when)
        {
            if (_completeSet[Thread.CurrentThread].Contains(currentNode))
            {
                return null;
            }
            else
            {
                _completeSet[Thread.CurrentThread].Add(currentNode);
            }

            var queue = _priorityQueues[Thread.CurrentThread];
            var candidates = new List<CompleteNode>();
            
            candidates.AddRange(GetRouteCandidates(currentNode, incompleteNodes, when));
            candidates.AddRange(GetTransferCandidates(currentNode, incompleteNodes, when));

            foreach (var cnode in candidates)
            {
                var qnode = queue.GetNode(cnode.stopId);

                if ((qnode == null) || (qnode.departureTime > cnode.departureTime))
                {
                    queue.UpdateNode(cnode);
                }
            }

            return queue.Pop();
        }
    }
}
