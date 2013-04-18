using MTR.BusinessLogic.Common.POCO;
using MTR.BusinessLogic.DataTransformer;
using MTR.BusinessLogic.Pathfinder.Dijkstra;
using MTR.DataAccess.EFDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder
{
    public abstract class PathfinderAlgorithm
    {
        private Dictionary<int, Stop> _allStops;                            // StopId -> Stop
        private Dictionary<int, Dictionary<int, List<int>>> _graphMap;      // StopId -> RouteId -> nextStopId
        private Dictionary<int, List<int>> _stopGroups;                     // GroupId -> StopId

        private bool isInitialized = false;

        private void ThrowExceptionIfNotInitialized()
        {
            if (!isInitialized)
            {
                throw new Exception("Pathfinder is not initialized.");
            }
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            var dbStops = DbManager.GetAllStops();

            _allStops = dbStops.ToDictionary(s => s.DbId);
            _graphMap = GraphTransformer.GetGraphMap();
            _stopGroups = new Dictionary<int, List<int>>();

            // StopGroup asszociatív lista inicializációja
            foreach (var stop in dbStops.Where(s => s.GroupId != null))
            {
                if (!_stopGroups.ContainsKey((int)stop.GroupId))
                {
                    _stopGroups.Add((int)stop.GroupId, new List<int>());
                }
                _stopGroups[(int)stop.GroupId].Add(stop.DbId);
            }

            isInitialized = true;
        }

        public abstract List<Edge> GetShortestRoute(int sourceStopId, int destinationStopId, DateTime when);

        protected IEnumerable<Stop> AllStops
        {
            get
            {
                ThrowExceptionIfNotInitialized();
                return _allStops.Values;
            }
        }

        protected IEnumerable<Stop> GetStops()
        {
            return AllStops;
        }

        protected Stop GetStop(int stopId)
        {
            ThrowExceptionIfNotInitialized();

            try
            {
                return _allStops[stopId];
            }
            catch
            {
                return null;
            }
        }

        protected IEnumerable<int> GetRoutes(int stopId)
        {
            ThrowExceptionIfNotInitialized();

            try
            {
                return _graphMap[stopId].Keys;
            }
            catch
            {
                return null;
            }
        }

        protected IEnumerable<Stop> GetEndpoints(int stopId, int routeId)
        {
            ThrowExceptionIfNotInitialized();

            try
            {
                return _graphMap[stopId][routeId].Select(sid => GetStop(sid));
            }
            catch
            {
                return null;
            }
        }

        protected IEnumerable<Stop> GetStopsInGroup(int? groupId)
        {
            if (groupId == null)
            {
                return new List<Stop>();
            }

            ThrowExceptionIfNotInitialized();

            try
            {
                return _stopGroups[(int)groupId].Select(sid => GetStop(sid));
            }
            catch
            {
                return null;
            }
        }
    }
}
