using NetTopologySuite.Geometries;
using ShumenTraffic.Common.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Routes
{
    /// <summary>
    /// DTO for Route Stop (waypoint or actual bus stop on a route).
    /// </summary>
    public class RouteStopModel
    {
        /// <summary>
        /// Route stop ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Bus stop ID (nullable for waypoints).
        /// </summary>
        public int? BusStopId { get; set; }

        /// <summary>
        /// Bus stop name (if it's an actual stop).
        /// </summary>
        public string BusStopName { get; set; }

        /// <summary>
        /// Route stop's GPS location
        /// </summary>
        [Required]
        [PointRange]
        public Point Location { get; set; }

        /// <summary>
        /// Order of this point in the route (1-based).
        /// </summary>
        [Required(ErrorMessage = "Stop order is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Stop order must be greater than 0")]
        public int StopOrder { get; set; }

        /// <summary>
        /// Estimated minutes from the start of the route (only for actual bus stops).
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Estimated minutes must be non-negative")]
        public int? EstimatedMinutesFromStart { get; set; }
    }
}