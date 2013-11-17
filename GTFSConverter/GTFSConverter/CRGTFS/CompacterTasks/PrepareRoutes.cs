using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFSConverter.CRGTFS
{
    public partial class RGTFSCompacter
    {
        void PrepareRoutes(ref TransitDB tdb, ref OriginalMaps originalMaps)
        {
            var routes = new List<Route>();

            foreach (var croute in db.routes)
            {
                var rroute = new Route
                {
                    name = croute.route_short_name,
                    description = croute.route_desc,
                    type = croute.route_type,
                    colour = new RGB
                    {
                        r = croute.route_color.red,
                        g = croute.route_color.green,
                        b = croute.route_color.blue
                    },
                    textColour = new RGB
                    {
                        r = croute.route_text_color.red,
                        g = croute.route_text_color.green,
                        b = croute.route_text_color.blue
                    },
                    dates = new ushort[0],
                    idx = routes.Count,
                    knownStops = new HashSet<int>()
                };

                routes.Add(rroute);
                originalMaps.originalRouteMap[croute.route_id] = rroute;
            }

            tdb.routes = routes.ToArray();
        }
    }
}
