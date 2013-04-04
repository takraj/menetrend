using MTR.BusinessLogic.Common.POCO;
using MTR.BusinessLogic.DataTransformer;
using MTR.DataAccess.CsvDataManager;
using MTR.DataAccess.EFDataManager;
using MTR.WebApp.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.DataManager
{
    public class DbDataManager
    {
        public static void initDatabase(string BasePath)
        {
            GtfsDatabase.initDatabase(BasePath);
        }

        /// <summary>
        /// Returns all stops from the database
        /// </summary>
        /// <returns></returns>
        public static List<VMDL_Stop> GetAllStops()
        {
            var result = new List<MTR.WebApp.Common.ViewModels.VMDL_Stop>();
            DbManager.GetAllStops().ForEach(s => result.Add(new VMDL_Stop(s.StopId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, s.ParentStation, s.WheelchairBoarding)));
            return result;
        }

        /// <summary>
        /// Creates and returns the stopgroups
        /// </summary>
        /// <param name="maxDistance">add element if the maximum distance from the
        /// group elements is less or equal to this (metres)</param>
        /// <returns>stopgroups</returns>
        public static List<VMDL_StopGroup> GetStopGroups(double maxDistance)
        {
            var result = new List<VMDL_StopGroup>();

            GraphTransformer.GetStopGroups(maxDistance).ForEach(sg => {
                var stops = new List<VMDL_Stop>();
                sg.GetStops().ForEach(s => stops.Add(new VMDL_Stop(s.StopId, s.StopName, s.StopLatitude, s.StopLongitude, s.LocationType, s.ParentStation, s.WheelchairBoarding)));
                result.Add(new VMDL_StopGroup(stops, sg.HasDifferentNames, sg.avgLatitude, sg.avgLongitude, sg.name));
            });

            return result;
        }

        /// <summary>
        /// Todo...
        /// </summary>
        public static void GetRoute()
        {
            var stopsTripsDict = new Dictionary<int, List<int>>();  // StopId -> List<-TripId->
            var tripsTimesDict = new Dictionary<int, List<StopTime>>();  // TripId -> List<StopTime>

            {
                var times = DbManager.GetAllStopTimesForDate(new DateTime(2013, 3, 1));
                times.ForEach(st =>
                {
                    // minden megállóhoz, hogy milyen útvonalakon (trip) szerepel
                    {
                        List<int> stopsTripsValue;
                        if (stopsTripsDict.TryGetValue(st.stopId, out stopsTripsValue))
                        {
                            stopsTripsValue.Add(st.tripId);
                        }
                        else
                        {
                            stopsTripsValue = new List<int>();
                            stopsTripsValue.Add(st.tripId);
                            stopsTripsDict.Add(st.stopId, stopsTripsValue);
                        }
                    }

                    // minden útvonalhoz (trip), hogy sorban milyen StopTime-ok szerepelnek benne
                    {
                        List<StopTime> tripsTimesValue;
                        if (tripsTimesDict.TryGetValue(st.tripId, out tripsTimesValue))
                        {
                            tripsTimesValue.Add(st);
                        }
                        else
                        {
                            tripsTimesValue = new List<StopTime>();
                            tripsTimesValue.Add(st);
                            tripsTimesDict.Add(st.tripId, tripsTimesValue);
                        }
                    }
                });
            }

            // setup stopgroups
            var stopGroups = GraphTransformer.GetStopGroups(100, stopsTripsDict, tripsTimesDict);
        }
    }
}
