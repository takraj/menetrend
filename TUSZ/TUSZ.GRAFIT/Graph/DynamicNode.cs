﻿using System;
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
        public TransitGraph graph;
        public bool onlyTravelActionNextTime;
        public bool mustGetOn;

        private List<DynamicNode> nextDynamicNodes;

        public List<DynamicNode> GetNextDynamicNodes()
        {
            if (nextDynamicNodes == null)
            {
                bool tryGetBackToLastRoute = false;

                nextDynamicNodes = new List<DynamicNode>();
                if (!this.NeedsGetOnAction)
                {
                    TravelAction lastAction = (TravelAction)history.instructions.Last();

                    try
                    {
                        nextDynamicNodes.Add(GetNextDynamicNodeByTravelAction(lastAction));
                    }
                    catch (Exception) {
                        tryGetBackToLastRoute = true;
                    }
                }

                if (!onlyTravelActionNextTime)
                {
                    if (history.instructions.Count > 0)
                    {
                        var lastAction = history.instructions.Last();

                        if (!(lastAction is GetOnAction))
                        {
                            if (!mustGetOn)
                            {
                                nextDynamicNodes.AddRange(GetNextDynamicNodesByWalkAction(lastAction));
                            }

                            nextDynamicNodes.AddRange(GetNextDynamicNodesByChangeRouteAction(lastAction));
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

            #region Referencia DynamicNode inicializálása (GetOff benne van)
            var referenceNode = new DynamicNode
            {
                currentTime = getOffAction.endDate,
                graph = this.graph,
                onlyTravelActionNextTime = false,
                mustGetOn = false,
                stop = this.stop,
                history = new History
                {
                    usedRoutes = this.history.usedRoutes,
                    lastUsedRoute = this.history.lastUsedRoute,
                    instructions = new List<Instruction>(this.history.instructions),
                    totalWalkingTime = this.history.totalWalkingTime
                }
            };
            referenceNode.history.instructions.Add(getOffAction);
            #endregion

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

                #region DynamicNode inicializálása
                var addDynamicNode = new DynamicNode
                {
                    stop = referenceNode.stop,
                    onlyTravelActionNextTime = false,
                    graph = referenceNode.graph,
                    currentTime = getOnAction.endDate,
                    mustGetOn = false,
                    history = new History
                    {
                        instructions = new List<Instruction>(referenceNode.history.instructions),
                        lastUsedRoute = getOnAction.route,
                        usedRoutes = new HashSet<Route>(referenceNode.history.usedRoutes),
                        totalWalkingTime = referenceNode.history.totalWalkingTime,
                        totalDistance = referenceNode.history.totalDistance
                    }
                };
                addDynamicNode.history.instructions.Add(getOnAction);
                addDynamicNode.history.usedRoutes.Add(getOnAction.route);
                #endregion

                return addDynamicNode;
            }

            return null;
        }

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

                #region Referencia DynamicNode inicializálása (GetOff benne van)
                referenceNode = new DynamicNode
                {
                    currentTime = getOffAction.endDate,
                    graph = this.graph,
                    onlyTravelActionNextTime = false,
                    mustGetOn = false,
                    stop = this.stop,
                    history = new History
                    {
                        usedRoutes = this.history.usedRoutes,
                        lastUsedRoute = this.history.lastUsedRoute,
                        instructions = new List<Instruction>(this.history.instructions),
                        totalWalkingTime = this.history.totalWalkingTime,
                        totalDistance = this.history.totalDistance
                    }
                };
                referenceNode.history.instructions.Add(getOffAction);
                #endregion
            }

            if (((referenceNode.stop.firstTripArrives - referenceNode.currentTime.TimeOfDay.TotalMinutes) > graph.maxWaitingMinutesForNextTrip)
                && ((referenceNode.currentTime.TimeOfDay.TotalMinutes - referenceNode.stop.lastTripArrives) > graph.maxWaitingMinutesForNextTrip))
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

                #region DynamicNode inicializálása
                var addDynamicNode = new DynamicNode
                {
                    stop = referenceNode.stop,
                    onlyTravelActionNextTime = false,
                    mustGetOn = false,
                    graph = referenceNode.graph,
                    currentTime = getOnAction.endDate,
                    history = new History
                    {
                        instructions = new List<Instruction>(referenceNode.history.instructions),
                        lastUsedRoute = getOnAction.route,
                        usedRoutes = new HashSet<Route>(referenceNode.history.usedRoutes),
                        totalWalkingTime = referenceNode.history.totalWalkingTime,
                        totalDistance = referenceNode.history.totalDistance
                    }
                };
                addDynamicNode.history.instructions.Add(getOnAction);
                addDynamicNode.history.usedRoutes.Add(getOnAction.route);
                #endregion

                result.Add(addDynamicNode);
            }

            return result;
        }

        private List<DynamicNode> GetNextDynamicNodesByWalkAction(Instruction lastAction)
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
                if (this.stop.nearbyStops[i + 1] > graph.maxWalkingDistancePerChange)
                {
                    continue;
                }

                Stop stopDest = graph.GetStopByIndex(this.stop.nearbyStops[i]);
                double cost = graph.GetWalkingCostBetween(this.stop, stopDest);

                #region DynamicNode inicializálása
                var addDynamicNode = new DynamicNode
                {
                    graph = this.graph,
                    history = this.history,
                    onlyTravelActionNextTime = false,
                    stop = stopDest,
                    currentTime = (getOffAction == null) ? this.currentTime.AddMinutes(cost) : getOffAction.endDate.AddMinutes(cost),
                    mustGetOn = true
                };
                #endregion

                if (getOffAction != null)
                {
                    #region DynamicNode.History inicializálása
                    addDynamicNode.history = new History
                    {
                        usedRoutes = this.history.usedRoutes,
                        lastUsedRoute = this.history.lastUsedRoute,
                        instructions = new List<Instruction>(),
                        totalWalkingTime = this.history.totalWalkingTime + cost,
                        totalDistance = this.history.totalDistance + graph.GetDistanceBetween(this.stop, stopDest)
                    };
                    addDynamicNode.history.instructions.AddRange(this.history.instructions);
                    addDynamicNode.history.instructions.Add(getOffAction);
                    #endregion
                }

                result.Add(addDynamicNode);
            }

            return result;
        }

        private DynamicNode GetNextDynamicNodeByTravelAction(TravelAction lastAction)
        {
            var nextStopTime = lastAction.trip.stopTimes.First(
                            st => (st.arrivalTime > lastAction.toStopTime.arrivalTime));

            var addMinutes = nextStopTime.arrivalTime - lastAction.toStopTime.arrivalTime - lastAction.toStopTime.waitingTime;

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

            #region Új history inicializálása
            var newHistory = new History()
            {
                instructions = new List<Instruction>(),
                lastUsedRoute = this.history.lastUsedRoute,
                usedRoutes = this.history.usedRoutes,
                totalWalkingTime = this.history.totalWalkingTime,
                totalDistance = this.history.totalDistance + graph.GetDistanceBetween(this.stop, addTravelAction.stop)
            };
            newHistory.instructions.AddRange(this.history.instructions);
            newHistory.instructions.Add(addTravelAction);
            #endregion

            #region DynamicNode inicializálása
            var dynamicNode = new DynamicNode
            {
                stop = this.graph.GetStopByIndex(nextStopTime.StopIndex),
                graph = this.graph,
                onlyTravelActionNextTime = false,
                mustGetOn = false,
                currentTime = lastAction.endDate.AddMinutes(addMinutes),
                history = newHistory
            };
            #endregion

            return dynamicNode;
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
    }
}