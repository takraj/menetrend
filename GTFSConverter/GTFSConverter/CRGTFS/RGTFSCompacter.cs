using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        private CompactGTFSDB db;

        public RGTFSCompacter(CompactGTFSDB db)
        {
            this.db = db;
        }

        public TransitDB CreateReferencedDB(int countOfTransfersPerStop)
        {
            var tdb = new TransitDB();
            var originalMaps = new OriginalMaps();

            PrepareStops(ref tdb, ref originalMaps);
            Console.Write('|');

            PrepareShapes(ref tdb, ref originalMaps);
            Console.Write('|');

            PrepareRoutes(ref tdb, ref originalMaps);
            Console.Write('|');

            PrepareTrips(ref tdb, ref originalMaps);
            Console.Write('|');

            Parallel.Invoke(
                () =>
                {
                    CalculateStopRouteRelationships(ref tdb, ref originalMaps);
                    Console.Write('*');
                },

                () =>
                {
                    CalculateTripDates(ref tdb, ref originalMaps);
                    Console.Write('*');
                },

                () =>
                {
                    CalculateTransferDistances(ref tdb, ref originalMaps);
                    Console.Write('*');
                });

            CalculateFirstLastArrivals(tdb, originalMaps);
            Console.Write('#');

            SetupNearbyStops(ref tdb, countOfTransfersPerStop);
            Console.Write('#');

            CalculateTransferMap(ref tdb);
            Console.Write('#');

            CalculateOptimumStops(tdb);
            Console.Write('#');

            return tdb;
        }
    }
}
