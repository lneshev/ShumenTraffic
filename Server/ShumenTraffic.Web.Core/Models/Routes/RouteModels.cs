using NetTopologySuite.Geometries;
using ShumenTraffic.Common.Core.Attributes;
using ShumenTraffic.Common.Core.Enums.Routes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Routes
{
    /// <summary>
    /// DTO for creating a new Route.
    /// </summary>
    public class CreateRouteDto
    {
        /// <summary>
        /// Bus line ID.
        /// </summary>
        [Required(ErrorMessage = "Bus line ID is required")]
        public int BusLineId { get; set; }

        /// <summary>
        /// Direction of the route (1 or 2).
        /// </summary>
        [Required(ErrorMessage = "Direction is required")]
        [Range(1, 2, ErrorMessage = "Direction must be 1 or 2")]
        public RouteDirection Direction { get; set; }

        /// <summary>
        /// Route name or description.
        /// </summary>
        [StringLength(256, ErrorMessage = "Route name cannot exceed 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Collection of stops and waypoints for this route.
        /// </summary>
        [Required(ErrorMessage = "At least one stop is required")]
        [MinLength(2, ErrorMessage = "Route must have at least 2 stops")]
        public List<CreateRouteStopDto> Stops { get; set; } = new List<CreateRouteStopDto>();
    }

    /// <summary>
    /// DTO for creating a Route Stop.
    /// </summary>
    public class CreateRouteStopDto
    {
        /// <summary>
        /// Bus stop ID (nullable for waypoints).
        /// </summary>
        public int? BusStopId { get; set; }

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

    /// <summary>
    /// DTO for updating a Route.
    /// </summary>
    public class UpdateRouteDto
    {
        /// <summary>
        /// Direction of the route (1 or 2).
        /// </summary>
        [Range(1, 2, ErrorMessage = "Direction must be 1 or 2")]
        public RouteDirection? Direction { get; set; }

        /// <summary>
        /// Route name or description.
        /// </summary>
        [StringLength(256, ErrorMessage = "Route name cannot exceed 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Whether the route is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}