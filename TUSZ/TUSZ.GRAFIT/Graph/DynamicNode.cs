using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUSZ.Common.GRAFIT;

namespace TUSZ.GRAFIT.Graph
{
    public class DynamicNode : IComparable<DynamicNode>
    {
        public History history;
        public DateTime currentTime;
        public Stop stop;
        public Stop targetStop;
        public TransitGraph graph;
        public bool onlyTravelActionNextTime;
        public bool mustGetOn;
        public HashSet<int> reachableStops;
        public TransferTree transferTree;

        private List<DynamicNode> nextDynamicNodes;

        public List<DynamicNode> GetNextDynamicNodes()
        {
            if (nextDynamicNodes == null)
            {
                bool tryGetBackToLastRoute = false;

                nextDynamicNodes = new List<DynamicNode>();
                if (!this.NeedsGetOnAction)
                {
                    TravelAction lastAction = (TravelAction)history.lastInstruction;
                    var byTravelAction = GetNextDynamicNodeByTravelAction(lastAction);
                    if (byTravelAction != null)
                    {
                        nextDynamicNodes.Add(byTravelAction);
                    }
                    else
                    {
                        tryGetBackToLastRoute = true;
                    }
                }

                if (!onlyTravelActionNextTime)
                {
                    if (history.instructions.Count > 0)
                    {
                        var lastAction = history.lastInstruction;

                        if (!(lastAction is GetOnAction))
                        {
                            if (!mustGetOn)
                            {
                                nextDynamicNodes.AddRange(GetNextDynamicNodesByWalkAction(lastAction));
                            }

                            if (this.transferTree == null || !this.transferTree.IsLeaf)
                            {
                                nextDynamicNodes.AddRange(GetNextDynamicNodesByChangeRouteAction(lastAction));
                            }
                        }

                        /*
                         * Ha véletlen garázsmenetet fogtunk ki, megpróbálhatunk visszaszállni.
                         */
                        if (tryGetBackToLastRoute)
                        {
                            var getBackNode = GetNextDynamicNodeByGetBackAction((TravelAction)lastAction);

                            if (getBackNode != null)
                            {
                                nextDynamicNodes.Add(getBackNode);
                            }
                        }
                    }
                    else
                    {
                        if (!mustGetOn)
                        {
                            nextDynamicNodes.AddRange(GetNextDynamicNodesByWalkAction(null));
                        }
                        nextDynamicNodes.AddRange(GetNextDynamicNodesByChangeRouteAction(null));
                    }
                }
            }
         
            return nextDynamicNodes;
        }

        /// <summary>
        /// Csomópont kifejtése az utolsó járatra visszaszállással.
        /// </summary>
        /// <param name="lastAction"></param>
        /// <returns></returns>
        private DynamicNode GetNextDynamicNodeByGetBackAction(TravelAction lastAction)
        {
            #region GetOffAction inicializálása
            var getOffAction = new GetOffAction
            {
                trip = lastAction.trip,
                stop = lastAction.stop,
                route = lastAction.route,
                startDate = lastAction.endDate,
                endDate = lastAction.endDate.AddMinutes(graph.costOfGettingOff)
            };
            #endregion

            // Referencia DynamicNode inicializálása (GetOff benne van)
            var referenceNode = this.AppendInstruction(getOffAction);

            var changeOption = graph.FindNextStopTimeOnRoute(referenceNode.stop,
                referenceNode.currentTime, referenceNode.history.lastUsedRoute);

            if (changeOption != null)
            {
                #region GetOnAction inicializálása
                var tripOption = graph.GetTripByIndex(changeOption.stopTime.TripIndex);
                if (tripOption.stopSequenceHint == lastAction.trip.stopSequenceHint)
                {
                    return null;
                }

                var getOnAction = new GetOnAction
                {
                    startDate = referenceNode.currentTime,
                    endDate = changeOption.arrivalTime,
                    stop = referenceNode.stop,
                    trip = tripOption,
                    route = graph.GetRouteByIndex(tripOption.routeIndex),
                    toStopTime = changeOption.stopTime
                };
                #endregion
                
                var addDynamicNode = referenceNode.AppendInstruction(getOnAction);
                addDynamicNode.history.lastStopTimeIndex = changeOption.stopTimeIndex;
                return addDynamicNode;
            }

            return null;
        }

        /// <summary>
        /// Csomópont kifejtése másik járatra átszállással.
        /// </summary>
        /// <param name="lastAction"></param>
        /// <returns></returns>
        private List<DynamicNode> GetNextDynamicNodesByChangeRouteAction(Instruction lastAction)
        {
            var result = new List<DynamicNode>();
            Instruction getOffAction = null;
            DynamicNode referenceNode = this;

            if (!this.NeedsGetOnAction && (lastAction != null))
            {
                #region GetOffAction inicializálása
                getOffAction = new GetOffAction
                {
                    trip = lastAction.trip,
                    stop = lastAction.stop,
                    route = lastAction.route,
                    startDate = lastAction.endDate,
                    endDate = lastAction.endDate.AddMinutes(graph.costOfGettingOff)
                };
                #endregion

                // Referencia DynamicNode inicializálása (GetOff benne van)
                referenceNode = this.AppendInstruction(getOffAction);
            }

            if (((referenceNode.stop.firstTripArrives - referenceNode.currentTime.TimeOfDay.TotalMinutes) > graph.maxWaitingMinutesForNextTrip)
                && (referenceNode.currentTime.TimeOfDay.TotalMinutes > referenceNode.stop.lastTripArrives))
            {
                return result;
            }

            var changeOptions = graph.GetChangeOptions(referenceNode.stop,
                referenceNode.currentTime, referenceNode.history.usedRoutes);

            foreach (var changeOption in changeOptions)
            {
                if ((changeOption.arrivalTime - referenceNode.currentTime).TotalMinutes > graph.maxWaitingMinutesForNextTrip)
                {
                    continue;
                }

                #region GetOnAction inicializálása
                var tripOption = graph.GetTripByIndex(changeOption.stopTime.TripIndex);
                var routeOption = graph.GetRouteByIndex(tripOption.routeIndex);

                if ((transferTree != null) && !transferTree.IsRouteAllowed(routeOption))
                {
                    continue;
                }

                var getOnAction = new GetOnAction
                {
                    startDate = referenceNode.currentTime,
                    endDate = changeOption.arrivalTime,
                    stop = referenceNode.stop,
                    trip = tripOption,
                    route = routeOption,
                    toStopTime = changeOption.stopTime
                };
                #endregion

                // DynamicNode inicializálása
                var addDynamicNode = referenceNode.AppendInstruction(getOnAction);

                if (this.transferTree != null)
                {
                    addDynamicNode.transferTree = this.transferTree.GetTree(routeOption);
                }

                addDynamicNode.history.lastStopTimeIndex = changeOption.stopTimeIndex;
                result.Add(addDynamicNode);
            }

            return result;
        }

        /// <summary>
        /// Csomópont kifejtése sétálással.
        /// </summary>
        /// <param name="lastAction"></param>
        /// <returns></returns>
        public List<DynamicNode> GetNextDynamicNodesByWalkAction(Instruction lastAction)
        {
            var result = new List<DynamicNode>();
            Instruction getOffAction = null;

            if (!this.NeedsGetOnAction && (lastAction != null))
            {
                #region GetOffAction inicializálása
                getOffAction = new GetOffAction
                {
                    trip = lastAction.trip,
                    stop = lastAction.stop,
                    route = lastAction.route,
                    startDate = lastAction.endDate,
                    endDate = lastAction.endDate.AddMinutes(graph.costOfGettingOff)
                };
                #endregion
            }

            for (int i = 0; i < this.stop.nearbyStops.Length; i += 2 )
            {
                if (this.reachableStops.Contains(this.stop.nearbyStops[i]))
                {
                    continue;
                }

                if (this.stop.nearbyStops[i + 1] != this.targetStop.idx)
                {
                    if (this.stop.nearbyStops[i + 1] > graph.maxWalkingDistancePerChange)
                    {
                        continue;
                    }

                    // Itt lenne optimális átsétálni a másik megállóba?
                    if (getOffAction != null)
                    {
                        var route = graph.GetRouteByIndex(this.CurrentTrip.routeIndex);
                        string key = this.CurrentTrip.stopSequenceHint.ToString() + "-" + this.stop.nearbyStops[i].ToString();

                        if (route.optimumStop[key] != this.stop.idx)
                        {
                            continue; // nem itt optimális átsétálni
                        }
                    }
                }

                Stop stopDest = graph.GetStopByIndex(this.stop.nearbyStops[i]);
                double cost = graph.GetWalkingCostBetween(this.stop, stopDest);

                // DynamicNode inicializálása
                var addDynamicNode = this.Clone();
                addDynamicNode.stop = stopDest;
                onlyTravelActionNextTime = false;
                addDynamicNode.mustGetOn = true;
                addDynamicNode.currentTime = (getOffAction == null) ? this.currentTime.AddMinutes(cost) : getOffAction.endDate.AddMinutes(cost);

                if (getOffAction != null)
                {
                    // DynamicNode.History inicializálása
                    addDynamicNode.history = this.history.AppendInstruction(getOffAction);
                    addDynamicNode.history.totalWalkingTime = this.history.totalWalkingTime + cost;
                }

                result.Add(addDynamicNode);
            }

            return result;
        }

        /// <summary>
        /// Csomópont kifejtése továbbutazással.
        /// </summary>
        /// <param name="lastAction"></param>
        /// <returns></returns>
        public DynamicNode GetNextDynamicNodeByTravelAction(TravelAction lastAction)
        {
            int nextIndex = this.history.lastStopTimeIndex + 1;
            if ((lastAction.trip.stopTimes.Length - 2) < nextIndex)
            {
                return null;
            }

            var nextStopTime = lastAction.trip.stopTimes[nextIndex];
            var addMinutes = nextStopTime.arrivalTime - lastAction.toStopTime.arrivalTime;

            #region TravelAction inicializálása
            var addTravelAction = new TravelAction
            {
                startDate = this.currentTime.AddMinutes(lastAction.toStopTime.waitingTime),
                endDate = this.currentTime.AddMinutes(addMinutes),
                fromStopTime = lastAction.toStopTime,
                toStopTime = nextStopTime,
                trip = lastAction.trip,
                stop = this.graph.GetStopByIndex(nextStopTime.StopIndex),
                route = lastAction.route
            };
            #endregion

            // DynamicNode inicializálása
            var addDynamicNode = this.AppendInstruction(addTravelAction);
            addDynamicNode.history.lastStopTimeIndex = nextIndex;
            return addDynamicNode;
        }
        
        public Trip CurrentTrip
        {
            get
            {
                try
                {
                    if (history.instructions.Count > 0)
                    {
                        if (history.instructions.Last() is GetOffAction) { return null; }
                        return history.instructions.Last().trip;
                    }

                    return null;
                }
                catch (Exception) { return null; }
            }
        }

        public bool NeedsGetOnAction
        {
            get
            {
                try
                {
                    if (history.instructions.Count == 0) { return true; }
                    if (history.instructions.Last() is GetOffAction) { return true; }
                }
                catch (Exception) { return true; }

                return false;
            }
        }

        public override int GetHashCode()
        {
            return currentTime.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DynamicNode))
            {
                return false;
            }

            var other = (DynamicNode)obj;

            if (this.currentTime.Equals(other.currentTime)
                && (this.graph == other.graph)
                && this.history.Equals(other.history)
                && this.onlyTravelActionNextTime.Equals(other.onlyTravelActionNextTime)
                && (this.stop == other.stop))
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            try
            {
                return String.Format("DynamicNode: stop={0}, currentTime={1}", stop.name, currentTime);
            }
            catch (Exception)
            {
                return "Uninitialized DynamicNode";
            }
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo((DynamicNode)obj);
        }

        public int CompareTo(DynamicNode other)
        {
            return this.currentTime.CompareTo(other.currentTime);
        }

        public DynamicNode Clone()
        {
            return new DynamicNode
            {
                currentTime = this.currentTime,
                graph = this.graph,
                history = this.history,
                mustGetOn = this.mustGetOn,
                onlyTravelActionNextTime = this.onlyTravelActionNextTime,
                stop = this.stop,
                targetStop = this.targetStop,
                reachableStops = this.reachableStops,
                transferTree = this.transferTree
            };
        }

        public DynamicNode AppendInstruction(Instruction instruction)
        {
            var result = this.Clone();
            result.stop = instruction.stop;
            result.currentTime = instruction.endDate;
            result.onlyTravelActionNextTime = false;
            result.mustGetOn = false;
            result.history = this.history.AppendInstruction(instruction);

            if ((this.history.lastInstruction == null)
                || (this.history.lastInstruction.trip.idx != instruction.trip.idx))
            {
                result.reachableStops = new HashSet<int>(result.reachableStops);

                foreach (int i in instruction.trip.stopTimes.Select(st => st.StopIndex))
                {
                    result.reachableStops.Add(i);
                }
            }

            return result;
        }

        public static DynamicNode CreateFirstDynamicNode(TransitGraph graph, Stop sourceStop, Stop targetStop, DateTime datetime)
        {
            return new DynamicNode
            {
                stop = sourceStop,
                targetStop = targetStop,
                onlyTravelActionNextTime = false,
                mustGetOn = false,
                history = History.CreateEmptyHistory(),
                graph = graph,
                currentTime = datetime,
                reachableStops = new HashSet<int>(),
                transferTree = null
            };
        }
    }
}
