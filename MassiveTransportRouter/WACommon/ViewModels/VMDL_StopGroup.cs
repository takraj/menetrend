using MTR.DataAccess.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.WebApp.Common.ViewModels
{
    public class VMDL_StopGroup
    {
        private List<VMDL_Stop> stops = new List<VMDL_Stop>();
        private int hasDifferentNamesIndicator = -1;
        
        public double avgLatitude = 0.0;
        public double avgLongitude = 0.0;
        public string name = "";

        public bool HasDifferentNames
        {
            get
            {
                if (hasDifferentNamesIndicator < 0)
                {
                    if (stops.Exists(s => !s.HasSimilarNameTo(this.name)))
                    {
                        hasDifferentNamesIndicator = 1;
                    }
                    else
                    {
                        hasDifferentNamesIndicator = 0;
                    }
                }
                return (hasDifferentNamesIndicator == 1);
            }
        }

        public double GetMaxDistanceTo(double lat, double lon)
        {
            var lck = new Object();
            double result = 0.0;

            stops.AsParallel().ForAll(stop =>
            {
                var dst = Utility.measureDistance(lat, lon, stop.StopLatitude, stop.StopLongitude);

                lock (lck)
                {
                    if (result < dst)
                    {
                        result = dst;
                    }
                }
            });

            return result;
        }

        public void AddStop(VMDL_Stop stop)
        {
            hasDifferentNamesIndicator = -1;
            stops.Add(stop);
            avgLatitude = stops.Average(s => s.StopLatitude);
            avgLongitude = stops.Average(s => s.StopLongitude);

            if (this.name.Equals("")) {
                this.name = stop.StopName;
            }

            if (!stop.HasSimilarNameTo(this.name))
            {
                hasDifferentNamesIndicator = 1;
            }
            else if (hasDifferentNamesIndicator < 0)
            {
                hasDifferentNamesIndicator = 0;
            }
        }

        public List<VMDL_Stop> GetStops()
        {
            return new List<VMDL_Stop>(stops);
        }
    }
}
