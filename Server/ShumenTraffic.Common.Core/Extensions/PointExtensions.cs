using NetTopologySuite.Geometries;

namespace ShumenTraffic.Common.Core.Extensions
{
    public static class PointExtensions
    {
        public static double GetLatitude(this Point p) => p.Y;
        public static void SetLatitude(this Point p, double value) => p.Y = value;

        public static double GetLongitude(this Point p) => p.X;
        public static void SetLongitude(this Point p, double value) => p.X = value;
    }
}