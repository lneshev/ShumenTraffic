using NetTopologySuite.Geometries;
using ShumenTraffic.Common.Core.Attributes;
using ShumenTraffic.Common.Core.Entities.BusStops;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Common.Core.Entities.Routes
{
    /// <summary>
    /// Represents a point on a specific route. Can be either an actual bus stop (where passengers board/alight)
    /// or a waypoint that defines the route path between stops.
    /// </summary>
    public class RouteStop : TrackableEntityBase<int>
    {
        /// <summary>
        /// Route stop's GPS location
        /// </summary>
        [Required]
        [PointRange]
        public Point Location { get; set; }

        /// <summary>
        /// Order of this point in the route (1-based).
        /// </summary>
        public int StopOrder { get; set; }

        /// <summary>
        /// Estimated minutes from the start of the route. Only populated for actual bus stops (when BusStopId IS NOT NULL).
        /// Nullable for waypoints.
        /// </summary>
        public int? EstimatedMinutesFromStart { get; set; }

        /// <summary>
        /// Foreign key to the route.
        /// </summary>
        public int RouteId { get; set; }

        // Navigation properties
        /// <summary>
        /// The route this stop belongs to.
        /// </summary>
        public virtual Route Route { get; set; }

        /// <summary>
        /// Foreign key to the bus stop. Nullable - NULL indicates this is a waypoint only.
        /// </summary>
        public int? BusStopId { get; set; }

        /// <summary>
        /// The bus stop at this location.
        /// </summary>
        public virtual BusStop BusStop { get; set; }
    }
}