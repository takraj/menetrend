using MTR.BusinessLogic.Common.POCO;
using MTR.DataAccess.EFDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.BusinessLogic.DataTransformer
{
    public class GraphTransformer
    {
        /// <summary>
        /// Creates and returns the stopgroups
        /// </summary>
        /// <param name="maxDistance">add element if the maximum distance from the
        /// group elements is less or equal to this (metres)</param>
        /// <returns>stopgroups</returns>
        public static List<StopGroup> GetStopGroups(double maxDistance, Dictionary<int, List<int>> stopsTripsDict = null, Dictionary<int, List<StopTime>> tripsTimesDict = null)
        {
            var result = new List<StopGroup>();
            var allStops = DbManager.GetAllStops();

            // default paraméterértéknél ne keressünk továbblépési pontokat, mert elszállna a program
            var dontFindNextStops = (stopsTripsDict == null || tripsTimesDict == null);

            // A feldolgozás szekvenciális, nem lehet hatékonyan párhuzamosítani!
            allStops.ForEach(stop =>
            {
                // Only if the current stop is not contained by a stopgroup
                if (!result.Exists(sg => sg.GetStops().Contains(stop)))
                {
                    bool createNewGroup = true;

                    foreach (StopGroup sg in result)
                    {
                        // Similar names (one contains the other & length difference is small)
                        // Example: "Cinkota" VS "Cinkota H"
                        if (sg.GetStops().Exists(s => s.HasSimilarNameTo(stop.StopName)))
                        {
                            if (dontFindNextStops)
                            {
                                sg.AddStop(stop);
                            }
                            else
                            {
                                // Adjuk hozzá ÉS keressük meg a lehetséges továbblépési pontokat is!
                                sg.AddStop(stop, stopsTripsDict, tripsTimesDict, allStops);
                            }
                            createNewGroup = false;
                            break;
                        }

                        // Not similar name, but it's near
                        else if (sg.GetMaxDistanceTo(stop.StopLatitude, stop.StopLongitude) <= maxDistance)
                        {
                            if (dontFindNextStops)
                            {
                                sg.AddStop(stop);
                            }
                            else
                            {
                                // Adjuk hozzá ÉS keressük meg a lehetséges továbblépési pontokat is!
                                sg.AddStop(stop, stopsTripsDict, tripsTimesDict, allStops);
                            }
                            createNewGroup = false;
                            break;
                        }
                    }

                    if (createNewGroup)
                    {
                        var sg = new StopGroup();
                        if (dontFindNextStops)
                        {
                            sg.AddStop(stop);
                        }
                        else
                        {
                            // Adjuk hozzá ÉS keressük meg a lehetséges továbblépési pontokat is!
                            sg.AddStop(stop, stopsTripsDict, tripsTimesDict, allStops);
                        }
                        result.Add(sg);
                    }
                }
            });

            // A visszatérési érték az összes létrehozott csoport.
            // Minden megálló bekerül valamilyen csoportba, ha más nem, akkor egy-eleműbe.
            return result;
        }
    }
}
