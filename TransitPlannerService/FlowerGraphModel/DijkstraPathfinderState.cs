using PathfinderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowerGraphModel
{
    public class DijkstraPathfinderState : IPathfinderState<FlowerNode, DateTime>
    {
        protected Dictionary<int, DateTime> _minSeqEndTimes;
        protected Dictionary<object, DateTime> _distanceVector;
        protected Dictionary<FlowerNode, FlowerNode> _previousVector;
        protected HashSet<object> _closedNodes;
        protected FlowerNode _startNode, _currentNode, _targetNode;

        public DijkstraPathfinderState(FlowerNode source, FlowerNode destination, DateTime when)
        {
            _minSeqEndTimes = new Dictionary<int, DateTime>();
            _distanceVector = new Dictionary<object, DateTime>();
            _previousVector = new Dictionary<FlowerNode, FlowerNode>();
            _closedNodes = new HashSet<object>();

            _startNode = source;
            _targetNode = destination;

            this.SetCost(_startNode, when);
            this.CurrentNode = _startNode;
        }

        public bool IsSequenceMinEndTimeKnown(int sequenceId)
        {
            return _minSeqEndTimes.ContainsKey(sequenceId);
        }

        public DateTime GetSequenceMinEndTime(int sequenceId)
        {
            return _minSeqEndTimes[sequenceId];
        }

        public void UpdateSequenceMinEndTime(int sequenceId, DateTime d)
        {
            if ((!_minSeqEndTimes.ContainsKey(sequenceId)) || (_minSeqEndTimes[sequenceId] > d))
            {
                _minSeqEndTimes[sequenceId] = d;
            }
        }

        public FlowerNode StartNode
        {
            get { return _startNode; }
        }

        public FlowerNode TargetNode
        {
            get { return _targetNode; }
        }

        public FlowerNode CurrentNode
        {
            get
            {
                return _currentNode;
            }
            set
            {
                _currentNode = value;
            }
        }

        public DateTime CurrentCost
        {
            get { return GetCost(_currentNode); }
        }

        public bool IsAtStartNode
        {
            get { return _currentNode.Equals(_startNode); }
        }

        public bool IsAtTargetNode
        {
            get { return _currentNode.Equals(_targetNode); }
        }

        public bool IsCurrentNodeClosed
        {
            get { return _closedNodes.Contains(_currentNode.DistanceVectorKey); }
        }

        public void SetClosed(FlowerNode node)
        {
            _closedNodes.Add(node.DistanceVectorKey);
        }

        public bool IsClosed(FlowerNode node)
        {
            return this.IsClosedRaw(node.DistanceVectorKey);
        }

        public bool IsClosedRaw(object rawData)
        {
            return _closedNodes.Contains(rawData);
        }

        public void SetPrevious(FlowerNode node, FlowerNode prevNode)
        {
            _previousVector[node] = prevNode;
        }

        public FlowerNode GetPrevious(FlowerNode node)
        {
            return _previousVector[node];
        }

        public void SetCost(FlowerNode node, DateTime cost)
        {
            _distanceVector[node.DistanceVectorKey] = cost;
        }

        public DateTime GetCost(FlowerNode node)
        {
            return _distanceVector[node.DistanceVectorKey];
        }

        public bool IsCostKnown(FlowerNode node)
        {
            return _distanceVector.ContainsKey(node.DistanceVectorKey);
        }

        public virtual IComparable GetPriority(FlowerNode node)
        {
            return this.GetCost(node);
        }
    }
}
