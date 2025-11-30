using ShumenTraffic.Common.Core.Entities.Routes;
using System;

namespace ShumenTraffic.Common.Core.Entities.Schedules
{
    /// <summary>
    /// Represents a course (trip/departure) for a schedule on a specific route.
    /// </summary>
    public class ScheduleCourse : TrackableEntityBase<int>
    {
        /// <summary>
        /// Departure time for this course from the start of the route.
        /// Actual departure times at each stop are calculated by adding EstimatedMinutesFromStart from RouteStop.
        /// </summary>
        public TimeOnly DepartureTime { get; set; }

        /// <summary>
        /// Foreign key to the schedule.
        /// </summary>
        public int ScheduleId { get; set; }

        // Navigation properties
        /// <summary>
        /// The schedule this course belongs to.
        /// </summary>
        public virtual Schedule Schedule { get; set; }

        /// <summary>
        /// Foreign key to the route.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// The route this course uses.
        /// </summary>
        public virtual Route Route { get; set; }
    }
}