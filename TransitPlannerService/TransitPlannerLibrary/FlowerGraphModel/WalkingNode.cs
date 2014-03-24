using FlowerDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerGraphModel
{
    public class WalkingNode : FlowerNode
    {
        public WalkingNode(FlowerGraph graph, DijkstraPathfinderState state, int stopId) : base(graph, state, stopId)
        {
            return;
        }

        public WalkingNode(FlowerGraph graph, int stopId) : base(graph, stopId)
        {
            return;
        }

        protected static T Min<T>(T subj1, T subj2) where T : IComparable
        {
            return (subj1.CompareTo(subj2) <= 0) ? subj1 : subj2;
        }

        protected static DateTime GetMidnight(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
        }

        public override IEnumerable<KeyValuePair<FlowerNode, DateTime>> GetNeighbours()
        {
            var result = new List<KeyValuePair<FlowerNode, DateTime>>();

            var now = _state.GetCost(this);
            var maxDeparture = Min(_graph.Repository.MetaInfo.MaxDate, now + _graph.MaxWaitingTimePerTransfer);
            var todayMidnight = GetMidnight(now);
            var stop = _graph.Repository.GetStopById(_stopId);

            foreach (var dst in stop.Distances)
            {
                if (!_state.IsClosedRaw(dst.StopIdx))
                {
                    var walkToNode = new WalkingNode(_graph, _state, dst.StopIdx);
                    var walkToCost = now.AddHours(dst.Distance / 1000.0 / _graph.WalkingSpeed);
                    result.Add(new KeyValuePair<FlowerNode, DateTime>(walkToNode, walkToCost));
                }
            }

            var alreadyAcceptedSequences = new HashSet<int>();

            foreach (int routeIdx in stop.Routes)
            {
                var route = _graph.Repository.GetRouteById(routeIdx);

                foreach (int sequenceId in route.TripsBySequence.Keys)
                {
                    if (_state.IsClosedRaw(new IntegerPair(sequenceId, _stopId)))
                    {
                        continue;
                    }

                    if (alreadyAcceptedSequences.Contains(sequenceId))
                    {
                        continue;
                    }

                    if (_state.IsSequenceMinEndTimeKnown(sequenceId) && _state.GetSequenceMinEndTime(sequenceId) < now)
                    {
                        continue;
                    }

                    bool foundTrip = false;

                    foreach (int tripId in route.TripsBySequence[sequenceId])
                    {
                        if (foundTrip)
                        {
                            break;
                        }

                        var lookupResult = _graph.Repository.LookupNextStop(sequenceId, _stopId);

                        if (lookupResult == null)
                        {
                            break;
                        }

                        var trip = _graph.Repository.GetTripById(tripId);
                        var currentMidnight = todayMidnight;

                        if (trip.DayOverlap)
                        {
                            currentMidnight = now.AddMinutes((-1) * trip.Duration);
                        }

                        while (currentMidnight < maxDeparture)
                        {
                            var endtime = currentMidnight.AddMinutes(trip.IntervalTo);

                            if (now <= endtime)
                            {
                                var serviceDay = (currentMidnight - _graph.Repository.MetaInfo.MinDate).Days;

                                if ((serviceDay >= 0) && (_graph.Repository.IsServiceAvailableOnDay(trip.ServiceIdx, serviceDay)))
                                {
                                    var tripBaseTime = currentMidnight.AddMinutes(trip.IntervalFrom);

                                    if (tripBaseTime <= maxDeparture)
                                    {
                                        var thisStopDeparture = tripBaseTime.AddMinutes(lookupResult.DepartureTime);

                                        if (thisStopDeparture >= (now + _graph.GetOnOffTimePerTransfer))
                                        {
                                            var node = new TravellingNode(_graph, _state, _stopId, sequenceId, tripId, tripBaseTime);
                                            var nodeArrival = thisStopDeparture;

                                            result.Add(new KeyValuePair<FlowerNode, DateTime>(node, nodeArrival));
                                            alreadyAcceptedSequences.Add(sequenceId);
                                            _state.UpdateSequenceMinEndTime(sequenceId, endtime);

                                            foundTrip = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            currentMidnight = currentMidnight.AddDays(1);
                        }
                    }
                }
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is WalkingNode)
            {
                var other = (WalkingNode)obj;
                return _stopId == other._stopId;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _stopId.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("WalkingNode(stop={0})", _stopId);
        }

        public override object DistanceVectorKey
        {
            get { return _stopId; }
        }
    }
}
