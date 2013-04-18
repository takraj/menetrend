using MTR.BusinessLogic.Pathfinder.Dijkstra;
using MTR.DataAccess.EFDataManager;
using MTR.WebApp.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.Pathfinder
{
    /// <summary>
    /// Manages Pathfinder singletons
    /// </summary>
    public class PathfinderManager
    {
        private static PathfinderAlgorithm _pathfinder_ShortestTime = null;

        /// <summary>
        /// Inicializálja az összes útvonalkereső algoritmust
        /// </summary>
        public static void InitializePathfinders()
        {
            _pathfinder_ShortestTime = new Dijkstra.FastDijkstraPathfinder();
        }

        /// <summary>
        /// Legrövidebb időre optimalizáló algoritmus lekérése
        /// </summary>
        /// <returns></returns>
        private static PathfinderAlgorithm GetShortestTimePathfinder()
        {
            if (_pathfinder_ShortestTime == null)
            {
                _pathfinder_ShortestTime = new Dijkstra.FastDijkstraPathfinder();
            }

            return _pathfinder_ShortestTime;
        }

        public static List<VMDL_RouteInstruction> GetRoute(int src, int dst, DateTime datetime)
        {
            var result = new List<VMDL_RouteInstruction>();
            var edgeList = GetShortestTimePathfinder().GetShortestRoute(src, dst, datetime);

            {
                // kezdőpont
                var toStop = DbManager.GetStopById(src);
                var vmdlStop = new VMDL_Stop(toStop.DbId, toStop.StopName, toStop.StopLatitude, toStop.StopLongitude, toStop.LocationType, toStop.ParentStation, toStop.WheelchairBoarding);
                result.Add(new VMDL_RouteInstruction
                {
                    isTransfer = true,
                    routeName = "Lábbusz",
                    timeString = datetime.TimeOfDay.ToString(@"hh\:mm"),
                    stop = vmdlStop
                });
            }

            foreach (var edge in edgeList)
            {
                // érintett megállók (beleértve a célállomást is)
                var toStop = DbManager.GetStopById(edge.GetDestinationStopId());
                var vmdlStop = new VMDL_Stop(toStop.DbId, toStop.StopName, toStop.StopLatitude, toStop.StopLongitude, toStop.LocationType, toStop.ParentStation, toStop.WheelchairBoarding);
                result.Add(new VMDL_RouteInstruction
                {
                    isTransfer = (edge is TransferEdge),
                    routeName = (edge is TransferEdge) ? "Lábbusz" : DbManager.GetRouteById(((RouteEdge)edge).RouteId).RouteShortName,
                    timeString = edge.GetTimeString(),
                    stop = vmdlStop
                });
            }

            return result;
        }
    }
}
