using System;
using System.Collections.Generic;

namespace ShumenTraffic.Common.Core.Entities
{
    /// <summary>
    /// Represents a geographical zone or neighborhood.
    /// </summary>
    public class Zone
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Zone name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Zone description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if the zone is active.
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
        /// Collection of bus stops in this zone.
        /// </summary>
        public ICollection<BusStop> BusStops { get; set; } = new List<BusStop>();
    }
}