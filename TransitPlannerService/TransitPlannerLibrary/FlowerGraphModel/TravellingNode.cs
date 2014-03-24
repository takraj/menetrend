using FlowerDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerGraphModel
{
    public class TravellingNode : FlowerNode
    {
        protected int _sequenceId;
        protected int _tripId;
        protected DateTime _baseTime;

        public TravellingNode(FlowerGraph graph, DijkstraPathfinderState state, int stopId, int sequenceId, int tripId, DateTime baseTime) : base(graph, state, stopId)
        {
            _sequenceId = sequenceId;
            _tripId = tripId;
            _baseTime = baseTime;
        }

        public override IEnumerable<KeyValuePair<FlowerNode, DateTime>> GetNeighbours()
        {
            var result = new List<KeyValuePair<FlowerNode, DateTime>>();

            if (!_state.IsClosedRaw(_stopId))
            {
                var walkNode = new WalkingNode(_graph, _state, _stopId);
                var cost = _state.GetCost(this) + _graph.GetOnOffTimePerTransfer;
                result.Add(new KeyValuePair<FlowerNode, DateTime>(walkNode, cost));
            }

            var lookupResult = _graph.Repository.LookupNextStop(_sequenceId, _stopId);

            if ((lookupResult != null) && (!_state.IsClosedRaw(new IntegerPair(lookupResult.NextStopIdx, _sequenceId))))
            {
                var nextNode = new TravellingNode(_graph, _state, lookupResult.NextStopIdx, _sequenceId, _tripId, _baseTime);
                var nextCost = _baseTime.AddMinutes(lookupResult.NextStopArrivalTime);
                result.Add(new KeyValuePair<FlowerNode, DateTime>(nextNode, nextCost));
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is TravellingNode)
            {
                var other = (TravellingNode)obj;
                return (_stopId == other._stopId) && (_sequenceId == other._sequenceId) && (_baseTime == other._baseTime);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _stopId.GetHashCode() ^ _sequenceId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("TravellingNode(stop={0}, seq={1}, base={2})", _stopId, _sequenceId, _baseTime);
        }

        public override object DistanceVectorKey
        {
            get { return new IntegerPair(_stopId, _sequenceId); }
        }

        public int TripId
        {
            get
            {
                return _tripId;
            }
        }
    }
}
