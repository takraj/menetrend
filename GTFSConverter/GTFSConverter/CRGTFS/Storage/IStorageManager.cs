using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS.Storage
{
    public interface IStorageManager
    {
        /// <summary>
        /// Létrehozza az adatbázist a TransitDB alapján.
        /// </summary>
        /// <param name="tdb"></param>
        void CreateDatabase(TransitDB tdb);

        /// <summary>
        /// Betölti az adatbázis részeit a memóriába és inicializálja.
        /// </summary>
        void LoadDatabase();

        string GetHeadsign(int headsignIndex);
        Route GetRoute(int routeIndex);
        Stop GetStop(int stopIndex);
        Trip GetTrip(int tripIndex);

        ShapeVector GetShape(int shapeIndex);
        int[] GetStopDistanceVector(int stopIndex);
        TripDate[] GetTripsForDate(int routeIndex, ushort day);

        int CountOfStops { get; }

        Proxies.StopListProxy Stops { get; }
        Proxies.TripListProxy Trips { get; }
        Proxies.ShapeVectorListProxy Shapes { get; }
        Proxies.StopDistanceVectorListProxy StopDistanceMatrix { get; }
        Proxies.HeadsignListProxy Headsigns { get; }
        Proxies.RouteListProxy Routes { get; }

        IEnumerator<Stop> CreateStopEnumerator();
        IEnumerator<Trip> CreateTripEnumerator();
        IEnumerator<ShapeVector> CreateShapeVectorEnumerator();
        IEnumerator<int[]> CreateStopDistanceVectorEnumerator();
        IEnumerator<string> CreateHeadsignEnumerator();
        IEnumerator<Route> CreateRouteEnumerator();
    }
}
