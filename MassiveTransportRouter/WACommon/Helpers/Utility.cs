using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MTR.DataAccess.Helpers
{
    public class Utility
    {
        public static void SetupCulture()
        {
            var newCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            newCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            newCulture.NumberFormat.CurrencyGroupSeparator = "";

            newCulture.NumberFormat.NumberDecimalSeparator = ".";
            newCulture.NumberFormat.NumberGroupSeparator = "";

            newCulture.NumberFormat.PercentDecimalSeparator = ".";
            newCulture.NumberFormat.PercentGroupSeparator = "";

            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
        }

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
    }
}