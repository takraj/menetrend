using System;

namespace PortableUtilityLibrary
{
    /// <summary>
    /// From: http://rosettacode.org/wiki/Haversine_formula#C.23
    /// </summary>
    public abstract class Haversine
    {
        /// <summary>
        /// Calculates the distance between two GeoCoordinates. The unit is 1 km.
        /// </summary>
        /// <param name="lat1">Latitude of the first point in degrees.</param>
        /// <param name="lon1">Longitude of the first point in degrees.</param>
        /// <param name="lat2">Latitude of the second point in degrees.</param>
        /// <param name="lon2">Longitude of the second point in degrees.</param>
        /// <returns>Distance in kilometers.</returns>
        public static double GetDistanceBetween(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6372.8; // In kilometers
            var dLat = toRadians(lat2 - lat1);
            var dLon = toRadians(lon2 - lon1);
            lat1 = toRadians(lat1);
            lat2 = toRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

        private static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}