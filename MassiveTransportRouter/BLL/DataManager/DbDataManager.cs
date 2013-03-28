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

            GetAllStops().ForEach(stop => {
                // Only if the current stop is not contained by a stopgroup
                if (!result.Exists(sg => sg.GetStops().Contains(stop)))
                {
                    bool createNewGroup = true; 

                    foreach (VMDL_StopGroup sg in result)
                    {
                        // Similar names (one contains the other & length difference is small)
                        // Example: "Cinkota" VS "Cinkota H"
                        if (sg.GetStops().Exists(s => s.HasSimilarNameTo(stop.StopName)))
                        {
                            sg.AddStop(stop);
                            createNewGroup = false;
                            break;
                        }

                        // Not similar name, but it's near
                        else if (sg.GetMaxDistanceTo(stop.StopLatitude, stop.StopLongitude) <= maxDistance)
                        {
                            sg.AddStop(stop);
                            createNewGroup = false;
                            break;
                        }
                    }

                    if (createNewGroup)
                    {
                        var sg = new VMDL_StopGroup();
                        sg.AddStop(stop);
                        result.Add(sg);
                    }
                }
            });

            return result;
        }
    }
}
