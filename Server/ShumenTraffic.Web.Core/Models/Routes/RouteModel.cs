using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.BusStops;
using ShumenTraffic.Common.Core.Enums.Routes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Routes
{
    /// <summary>
    /// DTO for Route Stop (waypoint or actual bus stop on a route).
    /// </summary>
    public class RouteStopDto
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

    /// <summary>
    /// DTO for Route.
    /// </summary>
    public class RouteModel : ModelBase<int>
    {
        /// <summary>
        /// Route name or description.
        /// </summary>
        [Required]
        [MaxLength(BusStopConstants.NameMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Direction of the route.
        /// </summary>
        public RouteDirection Direction { get; set; }

        /// <summary>
        /// Direction of the route as text.
        /// </summary>
        public string DirectionText { get; set; }

        /// <summary>
        /// Whether the route is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bus line ID.
        /// </summary>
        public int BusLineId { get; set; }

        /// <summary>
        /// Bus line number.
        /// </summary>
        public string BusLineNumber { get; set; }

        /// <summary>
        /// Collection of stops and waypoints on this route.
        /// </summary>
        public List<RouteStopDto> Stops { get; set; } = new List<RouteStopDto>();
    }

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