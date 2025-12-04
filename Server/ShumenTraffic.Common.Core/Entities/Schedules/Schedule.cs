using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Common.Core.Enums.Schedules;
using ShumenTraffic.Common.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Common.Core.Entities.Schedules
{
    /// <summary>
    /// Represents a schedule for a specific date range and day type.
    /// Contains multiple courses (trips/departures), each specifying which route it uses.
    /// </summary>
    public class Schedule : TrackableEntityBase<int>, IValidatableObject
    {
        /// <summary>
        /// Day type: "Weekday", "Saturday", or "Sunday".
        /// </summary>
        public DayType DayType { get; set; }

        /// <summary>
        /// Date when the schedule starts.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Date when the schedule ends (null means ongoing).
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Indicates if the schedule is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Priority level of the schedule.
        /// </summary>
        public SchedulePriority Priority { get; set; } = SchedulePriority.Normal;

        /// <summary>
        /// The direction of the route this schedule belongs to.
        /// </summary>
        public RouteDirection Direction { get; set; }

        /// <summary>
        /// Foreign key to the bus line.
        /// </summary>
        public int BusLineId { get; set; }

        /// <summary>
        /// The bus line this schedule belongs to.
        /// </summary>
        public virtual BusLine BusLine { get; set; }

        /// <summary>
        /// Collection of courses (trips/departures) for this schedule.
        /// Each course specifies which route it uses.
        /// </summary>
        public virtual ICollection<ScheduleCourse> ScheduleCourses { get; set; } = new List<ScheduleCourse>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult(Strings.StartDateMustBeLessThanOrEqualToEndDate);
            }
        }
    }
}