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
    public class ScheduleOverviewModel : ModelBase<int>, IValidatableObject
    {
        /// <summary>
        /// Day type: "Weekday", "Saturday", or "Sunday".
        /// </summary>
        public DayType DayType { get; set; }

        /// <summary>
        /// Day type as text
        /// </summary>
        public string DayTypeText { get; set; }

        /// <summary>
        /// Date when the schedule starts.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Date when the schedule ends (null means ongoing).
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Whether the schedule is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Priority level of the schedule.
        /// </summary>
        public SchedulePriority Priority { get; set; } = SchedulePriority.Normal;

        /// <summary>
        /// Priority level as text.
        /// </summary>
        public string PriorityText { get; set; }

        /// <summary>
        /// Bus line ID.
        /// </summary>
        public int BusLineId { get; set; }

        /// <summary>
        /// Bus line number.
        /// </summary>
        public string BusLineNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult(Strings.StartDateMustBeLessThanOrEqualToEndDate);
            }
        }
    }
}