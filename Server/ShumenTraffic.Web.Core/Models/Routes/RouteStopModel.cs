using MoravianStar.Dao;
using NetTopologySuite.Geometries;
using ShumenTraffic.Common.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Routes
{
    /// <summary>
    /// DTO for Route Stop (waypoint or actual bus stop on a route).
    /// </summary>
    public class RouteStopModel : ModelBase<int>
    {
        /// <summary>
        /// Bus stop ID (nullable for waypoints).
        /// </summary>
        public int? BusStopId { get; set; }

        /// <summary>
        /// Bus stop name (if it's an actual stop).
        /// </summary>
        public string BusStopName { get; set; }

        /// <summary>
        /// Bus stop location (if it's an actual stop).
        /// </summary>
        [PointRange]
        public Point BusStopLocation { get; set; }

        /// <summary>
        /// Route stop's GPS location
        /// </summary>
        [PointRange]
        public Point Location { get; set; }

        /// <summary>
        /// Order of this point in the route (1-based).
        /// </summary>
        [Range(1, int.MaxValue)]
        public int StopOrder { get; set; }

        /// <summary>
        /// Estimated minutes from the start of the route (only for actual bus stops).
        /// </summary>
        [Range(0, int.MaxValue)]
        public int? EstimatedMinutesFromStart { get; set; }
    }
}