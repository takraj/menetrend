using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void PrepareStops(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            var stops = new List<Stop>();

            foreach (var cstop in db.stops)
            {
                var rstop = new Stop
                {
                    name = cstop.stop_name,
                    position = new LatLng
                    {
                        latitude = (float)cstop.stop_lat,
                        longitude = (float)cstop.stop_lon
                    },
                    knownRoutes = new HashSet<int>(),
                    idx = stops.Count
                };

                stops.Add(rstop);
                originalMaps.originalStopMap[cstop.stop_id] = rstop;
            }

            tdb.stops = stops.ToArray();
        }
    }
}
