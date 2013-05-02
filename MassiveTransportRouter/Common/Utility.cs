using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTR.Common
{
    public class Utility
    {
        /// <summary>
        /// Calculates the distance between two GPS coordinates with the Haversine formula
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns>distance in metres</returns>
        public static double measureDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const int R = 6371; // km

            double latDistance = (lat2 - lat1) / (180 / Math.PI);
            double lonDistance = (lon2 - lon1) / (180 / Math.PI);
            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
                    + Math.Cos(lat1 / (180 / Math.PI)) * Math.Cos(lat2 / (180 / Math.PI))
                    * Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c * 1000;    // --> m
        }

        // nem pontos
        public static double approximateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var pk = (180 / 3.14169);

            var a1 = lat1 / pk;
            var a2 = lon1 / pk;
            var b1 = lat2 / pk;
            var b2 = lon2 / pk;

            var t1 = Math.Sin(lat1) * Math.Sin(lat2);
            var t2 = Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(Math.Abs(lon2-lon1));
            var tt = Math.Acos(t1 + t2);

            return 6366000 * tt;
        }

        /// <summary>
        /// Recreates a directory path
        /// </summary>
        /// <param name="path"></param>
        public static void recreateFolder(string path)
        {
            bool IsExists = System.IO.Directory.Exists(path);
            if (IsExists)
            {
                Directory.Delete(path, true);
            }
            System.IO.Directory.CreateDirectory(path);
        }
    }
}
