using System;

namespace ShumenTraffic.Common.Core.Entities
{
    /// <summary>
    /// Represents a course (trip/departure) for a schedule on a specific route.
    /// </summary>
    public class ScheduleCourse
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the schedule.
        /// </summary>
        public int ScheduleId { get; set; }

        /// <summary>
        /// Foreign key to the route.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Departure time for this course from the start of the route.
        /// Actual departure times at each stop are calculated by adding EstimatedMinutesFromStart from RouteStop.
        /// </summary>
        public TimeOnly DepartureTime { get; set; }

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
        /// The schedule this course belongs to.
        /// </summary>
        public virtual Schedule Schedule { get; set; }

        /// <summary>
        /// The route this course uses.
        /// </summary>
        public virtual Route Route { get; set; }
    }
}