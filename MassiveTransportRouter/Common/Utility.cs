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
            const int R = 6371;

            double latDistance = (lat2 - lat1) / (180 / Math.PI);
            double lonDistance = (lon2 - lon1) / (180 / Math.PI);
            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
                    + Math.Cos(lat1 / (180 / Math.PI)) * Math.Cos(lat2 / (180 / Math.PI))
                    * Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c * 1000;
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
