using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;


//HaversineFormula
//https://www.geeksforgeeks.org/program-distance-two-points-earth/
namespace DistanceHelper
{
    /// <summary>
    /// The distance type to return the results in.
    /// </summary>
    public enum DistanceType { Miles, Kilometers };
    /// <summary>
    /// Specifies a Latitude / Longitude point.
    /// </summary>
    public struct Position
    {
        public double Latitude;
        public double Longitude;
    }
    class Haversine
    {
        /// <summary>
        /// Returns the distance in miles or kilometers of any two
        /// latitude / longitude points.
        /// </summary>
        /// <param name=”pos1″></param>
        /// <param name=”pos2″></param>
        /// <param name=”type”></param>
        /// <returns></returns>
        public double Distance(Position pos1, Position pos2, DistanceType type)
        {
            //Metric system     6,357 to 6,378 km
            //English units     3,950 to 3,963 mi
            double R = (type == DistanceType.Miles) ? 3960 : 6371;

            double dLat = this.toRadian(pos2.Latitude - pos1.Latitude);
            double dLon = this.toRadian(pos2.Longitude - pos1.Longitude);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                Math.Cos(this.toRadian(pos1.Latitude)) *
                Math.Cos(this.toRadian(pos2.Latitude)) *
                Math.Pow(Math.Sin(dLon / 2) , 2);

            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }
        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private double toRadian(double val)
        {
            //Degrees × (π / 180) = Radians
            //Radians  × (180 / π) = Degrees
            //360 Degrees = 2π Radians
            //180 Degrees = π Radians
            return (Math.PI / 180) * val;
        }
    }
}