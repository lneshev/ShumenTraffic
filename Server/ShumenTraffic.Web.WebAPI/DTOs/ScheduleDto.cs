using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.WebAPI.DTOs
{
    /// <summary>
    /// DTO for Schedule Course (trip/departure).
    /// </summary>
    public class ScheduleCourseDto
    {
        /// <summary>
        /// Schedule course ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Route ID.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Bus line number.
        /// </summary>
        public string BusLineNumber { get; set; }

        /// <summary>
        /// Route direction.
        /// </summary>
        public int Direction { get; set; }

        /// <summary>
        /// Departure time from the start of the route.
        /// </summary>
        [Required(ErrorMessage = "Departure time is required")]
        public TimeSpan DepartureTime { get; set; }
    }

    /// <summary>
    /// DTO for Schedule.
    /// </summary>
    public class ScheduleDto
    {
        /// <summary>
        /// Schedule ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Day type: "Weekday", "Saturday", or "Sunday".
        /// </summary>
        [Required(ErrorMessage = "Day type is required")]
        [RegularExpression("^(Weekday|Saturday|Sunday)$", ErrorMessage = "Day type must be 'Weekday', 'Saturday', or 'Sunday'")]
        public string DayType { get; set; }

        /// <summary>
        /// Date when the schedule becomes effective.
        /// </summary>
        [Required(ErrorMessage = "Effective date is required")]
        public DateTimeOffset EffectiveDate { get; set; }

        /// <summary>
        /// Date when the schedule expires (null means ongoing).
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Whether the schedule is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Collection of courses for this schedule.
        /// </summary>
        public List<ScheduleCourseDto> Courses { get; set; } = new List<ScheduleCourseDto>();
    }

    /// <summary>
    /// DTO for creating a new Schedule.
    /// </summary>
    public class CreateScheduleDto
    {
        /// <summary>
        /// Day type: "Weekday", "Saturday", or "Sunday".
        /// </summary>
        [Required(ErrorMessage = "Day type is required")]
        [RegularExpression("^(Weekday|Saturday|Sunday)$", ErrorMessage = "Day type must be 'Weekday', 'Saturday', or 'Sunday'")]
        public string DayType { get; set; }

        /// <summary>
        /// Date when the schedule becomes effective.
        /// </summary>
        [Required(ErrorMessage = "Effective date is required")]
        public DateTimeOffset EffectiveDate { get; set; }

        /// <summary>
        /// Date when the schedule expires (null means ongoing).
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Collection of courses for this schedule.
        /// </summary>
        [Required(ErrorMessage = "At least one course is required")]
        [MinLength(1, ErrorMessage = "Schedule must have at least one course")]
        public List<CreateScheduleCourseDto> Courses { get; set; } = new List<CreateScheduleCourseDto>();
    }

    /// <summary>
    /// DTO for creating a Schedule Course.
    /// </summary>
    public class CreateScheduleCourseDto
    {
        /// <summary>
        /// Route ID.
        /// </summary>
        [Required(ErrorMessage = "Route ID is required")]
        public int RouteId { get; set; }

        /// <summary>
        /// Departure time from the start of the route.
        /// </summary>
        [Required(ErrorMessage = "Departure time is required")]
        public TimeSpan DepartureTime { get; set; }
    }

    /// <summary>
    /// DTO for updating a Schedule.
    /// </summary>
    public class UpdateScheduleDto
    {
        /// <summary>
        /// Date when the schedule expires (null means ongoing).
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Whether the schedule is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for updating a Schedule Course.
    /// </summary>
    public class UpdateScheduleCourseDto
    {
        /// <summary>
        /// Route ID.
        /// </summary>
        public int? RouteId { get; set; }

        /// <summary>
        /// Departure time from the start of the route.
        /// </summary>
        public TimeSpan? DepartureTime { get; set; }
    }
}

