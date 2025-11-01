using System;
using System.Collections.Generic;

namespace ShumenTraffic.Data.Models
{
    /// <summary>
    /// Represents a schedule for a specific date range and day type.
    /// Contains multiple courses (trips/departures), each specifying which route it uses.
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Day type: "Weekday", "Saturday", or "Sunday".
        /// </summary>
        public required string DayType { get; set; }

        /// <summary>
        /// Date when the schedule becomes effective.
        /// </summary>
        public DateTimeOffset EffectiveDate { get; set; }

        /// <summary>
        /// Date when the schedule expires (null means ongoing).
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Indicates if the schedule is active.
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
        /// Collection of courses (trips/departures) for this schedule.
        /// Each course specifies which route it uses.
        /// </summary>
        public ICollection<ScheduleCourse> ScheduleCourses { get; set; } = new List<ScheduleCourse>();
    }
}