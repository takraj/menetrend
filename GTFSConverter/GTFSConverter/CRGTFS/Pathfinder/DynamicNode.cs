using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Pathfinder
{
    public class DynamicNode
    {
        public History history;
        public DateTime currentTime;
        public Stop stop;
        public TransitGraph graph;
        public bool onlyTravelActionNextTime;

        private List<DynamicNode> nextDynamicNodes;

        public List<DynamicNode> GetNextDynamicNodes()
        {
            if (nextDynamicNodes == null)
            {
                bool tryGetBackToLastRoute = false;

                nextDynamicNodes = new List<DynamicNode>();
                if (!this.NeedsGetOnAction)
                {
                    TravelAction lastAction = (TravelAction)history.actions.Last();

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
                    if (history.actions.Count > 0)
                    {
                        var lastAction = history.actions.Last();

                        nextDynamicNodes.AddRange(GetNextDynamicNodesByWalkAction(lastAction));
                        nextDynamicNodes.AddRange(GetNextDynamicNodesByChangeRouteAction(lastAction));

                        /*
                         * Ha véletlen garázsmenetet fogtunk ki, megpróbálhatunk visszaszállni.
                         */
                        if (tryGetBackToLastRoute)
                        {
                            var getBackNode = GetNextDynamicNodeByGetBackAction((TravelAction)lastAction);

                            if ((getBackNode != null)
                                && (getBackNode.history.actions.Last().trip.stopTimes.Length
                                            > lastAction.trip.stopTimes.Length))
                            {
                                nextDynamicNodes.Add(getBackNode);
                            }
                        }
                    }
                    else
                    {
                        nextDynamicNodes.AddRange(GetNextDynamicNodesByWalkAction(null));
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
                endDate = lastAction.endDate.AddMinutes(5)
            };
            #endregion

            #region Referencia DynamicNode inicializálása (GetOff benne van)
            var referenceNode = new DynamicNode
            {
                currentTime = getOffAction.endDate,
                graph = this.graph,
                onlyTravelActionNextTime = false,
                stop = this.stop,
                history = new History
                {
                    usedRoutes = this.history.usedRoutes,
                    lastUsedRoute = this.history.lastUsedRoute,
                    actions = new List<Action>(this.history.actions),
                    totalWalkingTime = this.history.totalWalkingTime
                }
            };
            referenceNode.history.actions.Add(getOffAction);
            #endregion

            var changeOption = graph.FindNextStopTimeOnRoute(referenceNode.stop,
                referenceNode.currentTime, referenceNode.history.lastUsedRoute);

            if (changeOption != null)
            {
                #region GetOnAction inicializálása
                var tripOption = graph.GetTripByIndex(changeOption.stopTime.tripIndex);
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
                    history = new History
                    {
                        actions = new List<Action>(referenceNode.history.actions),
                        lastUsedRoute = getOnAction.route,
                        usedRoutes = new HashSet<Route>(referenceNode.history.usedRoutes),
                        totalWalkingTime = referenceNode.history.totalWalkingTime
                    }
                };
                addDynamicNode.history.actions.Add(getOnAction);
                addDynamicNode.history.usedRoutes.Add(getOnAction.route);
                #endregion

                return addDynamicNode;
            }

            return null;
        }

        private List<DynamicNode> GetNextDynamicNodesByChangeRouteAction(Action lastAction)
        {
            var result = new List<DynamicNode>();
            Action getOffAction = null;
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
                    endDate = lastAction.endDate.AddMinutes(5)
                };
                #endregion

                #region Referencia DynamicNode inicializálása (GetOff benne van)
                referenceNode = new DynamicNode
                {
                    currentTime = getOffAction.endDate,
                    graph = this.graph,
                    onlyTravelActionNextTime = false,
                    stop = this.stop,
                    history = new History
                    {
                        usedRoutes = this.history.usedRoutes,
                        lastUsedRoute = this.history.lastUsedRoute,
                        actions = new List<Action>(this.history.actions),
                        totalWalkingTime = this.history.totalWalkingTime
                    }
                };
                referenceNode.history.actions.Add(getOffAction);
                #endregion
            }

            var changeOptions = graph.GetChangeOptions(referenceNode.stop,
                referenceNode.currentTime, referenceNode.history.usedRoutes);

            foreach (var changeOption in changeOptions)
            {
                #region GetOnAction inicializálása
                var tripOption = graph.GetTripByIndex(changeOption.stopTime.tripIndex);
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
                    history = new History
                    {
                        actions = new List<Action>(referenceNode.history.actions),
                        lastUsedRoute = getOnAction.route,
                        usedRoutes = new HashSet<Route>(referenceNode.history.usedRoutes),
                        totalWalkingTime = referenceNode.history.totalWalkingTime
                    }
                };
                addDynamicNode.history.actions.Add(getOnAction);
                addDynamicNode.history.usedRoutes.Add(getOnAction.route);
                #endregion

                result.Add(addDynamicNode);
            }

            return result;
        }

        private List<DynamicNode> GetNextDynamicNodesByWalkAction(Action lastAction)
        {
            var result = new List<DynamicNode>();
            Action getOffAction = null;

            if (!this.NeedsGetOnAction && (lastAction != null))
            {
                #region GetOffAction inicializálása
                getOffAction = new GetOffAction
                {
                    trip = lastAction.trip,
                    stop = lastAction.stop,
                    route = lastAction.route,
                    startDate = lastAction.endDate,
                    endDate = lastAction.endDate.AddMinutes(5)
                };
                #endregion
            }

            for (int i = 0; i < this.stop.nearbyStops.Length; i += 2 )
            {
                //if (this.stop.nearbyStops[i + 1] > 500)
                //{
                //    continue;
                //}

                Stop stopDest = graph.GetStopByIndex(this.stop.nearbyStops[i]);
                int cost = graph.GetWalkingCostBetween(this.stop, stopDest);

                #region DynamicNode inicializálása
                var addDynamicNode = new DynamicNode
                {
                    graph = this.graph,
                    history = this.history,
                    onlyTravelActionNextTime = false,
                    stop = stopDest,
                    currentTime = this.currentTime.AddMinutes(cost)
                };
                #endregion

                if (getOffAction != null)
                {
                    #region DynamicNode.History inicializálása
                    addDynamicNode.history = new History
                    {
                        usedRoutes = this.history.usedRoutes,
                        lastUsedRoute = this.history.lastUsedRoute,
                        actions = new List<Action>(),
                        totalWalkingTime = this.history.totalWalkingTime + cost
                    };
                    addDynamicNode.history.actions.AddRange(this.history.actions);
                    addDynamicNode.history.actions.Add(getOffAction);
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
                stop = this.graph.GetStopByIndex(nextStopTime.refIndices[0]),
                route = lastAction.route
            };
            #endregion

            #region Új history inicializálása
            var newHistory = new History()
            {
                actions = new List<Action>(),
                lastUsedRoute = this.history.lastUsedRoute,
                usedRoutes = this.history.usedRoutes,
                totalWalkingTime = this.history.totalWalkingTime
            };
            newHistory.actions.AddRange(this.history.actions);
            newHistory.actions.Add(addTravelAction);
            #endregion

            #region DynamicNode inicializálása
            var dynamicNode = new DynamicNode
            {
                stop = this.graph.GetStopByIndex(nextStopTime.refIndices[0]),
                graph = this.graph,
                onlyTravelActionNextTime = false,
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
                    if (history.actions.Count > 0)
                    {
                        if (history.actions.Last() is GetOffAction) { return null; }
                        return history.actions.Last().trip;
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
                    if (history.actions.Count == 0) { return true; }
                    if (history.actions.Last() is GetOffAction) { return true; }
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
    }
}
