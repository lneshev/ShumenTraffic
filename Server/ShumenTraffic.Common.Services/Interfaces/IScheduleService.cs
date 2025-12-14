using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Enums.Common;
using ShumenTraffic.Common.Core.Enums.Schedules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces
{
    /// <summary>
    /// Service interface for Schedule entity operations.
    /// </summary>
    public interface IScheduleService : IBaseEntityService<Schedule>
    {
        /// <summary>
        /// Get all schedules with their courses.
        /// </summary>
        /// <param name="daysOfWeek">Filter by days of week (optional)</param>
        /// <param name="includeInactive">Include inactive schedules</param>
        /// <returns>List of schedules</returns>
        Task<IEnumerable<Schedule>> GetAllWithCoursesAsync(DaysOfWeek? daysOfWeek = null, bool includeInactive = false);

        /// <summary>
        /// Get schedule by ID with courses.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>Schedule or null if not found</returns>
        Task<Schedule> GetByIdWithCoursesAsync(int id);

        /// <summary>
        /// Create a new schedule with courses.
        /// </summary>
        /// <param name="daysOfWeek">Days of week</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date (optional)</param>
        /// <param name="priority">Priority level</param>
        /// <param name="courses">Schedule courses</param>
        /// <returns>Created schedule</returns>
        Task<Schedule> CreateAsync(DaysOfWeek daysOfWeek, DateOnly startDate, DateOnly? endDate, SchedulePriority priority, IEnumerable<ScheduleCourseData> courses);

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <param name="endDate">End date (optional)</param>
        /// <param name="isActive">Is active (optional)</param>
        /// <param name="priority">Priority level (optional)</param>
        /// <returns>Updated schedule or null if not found</returns>
        Task<Schedule> UpdateAsync(int id, DateOnly? endDate = null, bool? isActive = null, SchedulePriority? priority = null);
    }

    /// <summary>
    /// Data structure for creating schedule courses.
    /// </summary>
    public class ScheduleCourseData
    {
        public int RouteId { get; set; }
        public TimeSpan DepartureTime { get; set; }
    }
}