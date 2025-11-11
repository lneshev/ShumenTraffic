using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.Schedules;
using System;
using System.Collections.Generic;

namespace ShumenTraffic.Common.Core.Entities.Routes
{
    /// <summary>
    /// Represents a specific route for a bus line with direction.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the bus line.
        /// </summary>
        public int BusLineId { get; set; }

        /// <summary>
        /// Direction of the route (1 or 2).
        /// </summary>
        public int Direction { get; set; }

        /// <summary>
        /// Route name or description.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates if the route is active.
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
        /// The bus line this route belongs to.
        /// </summary>
        public virtual BusLine BusLine { get; set; }

        /// <summary>
        /// Collection of stops on this route.
        /// </summary>
        public virtual ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();

        /// <summary>
        /// Collection of courses (trips/departures) that use this route.
        /// </summary>
        public virtual ICollection<ScheduleCourse> ScheduleCourses { get; set; } = new List<ScheduleCourse>();
    }
}