using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.Zones;
using System;
using System.Collections.Generic;

namespace ShumenTraffic.Common.Core.Entities.BusStops
{
    /// <summary>
    /// Represents a physical bus stop location.
    /// </summary>
    public class BusStop
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the zone.
        /// </summary>
        public int ZoneId { get; set; }

        /// <summary>
        /// Bus stop name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Bus stop description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// GPS latitude coordinate (WGS84).
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// GPS longitude coordinate (WGS84).
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Indicates if the bus stop is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

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
        /// The zone this bus stop belongs to.
        /// </summary>
        public virtual Zone Zone { get; set; }

        /// <summary>
        /// Collection of route stops that include this bus stop.
        /// </summary>
        public virtual ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
    }
}