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
        /// Latitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }

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