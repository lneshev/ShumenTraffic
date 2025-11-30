using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Enums.Schedules;
using ShumenTraffic.Common.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Schedules
{
    /// <summary>
    /// DTO for Schedule.
    /// </summary>
    public class ScheduleModel : ModelBase<int>, IValidatableObject
    {
        /// <summary>
        /// Day type: "Weekday", "Saturday", or "Sunday".
        /// </summary>
        public DayType DayType { get; set; }

        /// <summary>
        /// Date when the schedule starts.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Date when the schedule ends (null means ongoing).
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// Whether the schedule is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bus line ID.
        /// </summary>
        public int BusLineId { get; set; }

        /// <summary>
        /// Collection of courses for this schedule.
        /// </summary>
        public List<ScheduleCourseDto> Courses { get; set; } = new List<ScheduleCourseDto>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult(Strings.StartDateMustBeLessThanOrEqualToEndDate);
            }
        }
    }
}