using System;

namespace ShumenTraffic.Common.Core.Entities
{
    /// <summary>
    /// Represents a point on a specific route. Can be either an actual bus stop (where passengers board/alight)
    /// or a waypoint that defines the route path between stops.
    /// </summary>
    public class RouteStop
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the route.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Foreign key to the bus stop. Nullable - NULL indicates this is a waypoint only.
        /// </summary>
        public int? BusStopId { get; set; }

        /// <summary>
        /// GPS latitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// GPS longitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        public decimal Longitude { get; set; }

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
        /// Timestamp when the record was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Timestamp when the record was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation properties
        /// <summary>
        /// The route this stop belongs to.
        /// </summary>
        public Route Route { get; set; }

        /// <summary>
        /// The bus stop at this location.
        /// </summary>
        public BusStop BusStop { get; set; }
    }
}