using ShumenTraffic.Common.Core.Entities.Schedules;
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
        /// <param name="dayType">Filter by day type (optional)</param>
        /// <param name="includeInactive">Include inactive schedules</param>
        /// <returns>List of schedules</returns>
        Task<IEnumerable<Schedule>> GetAllWithCoursesAsync(string dayType = null, bool includeInactive = false);

        /// <summary>
        /// Get schedule by ID with courses.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>Schedule or null if not found</returns>
        Task<Schedule> GetByIdWithCoursesAsync(int id);

        /// <summary>
        /// Create a new schedule with courses.
        /// </summary>
        /// <param name="dayType">Day type</param>
        /// <param name="effectiveDate">Effective date</param>
        /// <param name="expiryDate">Expiry date (optional)</param>
        /// <param name="courses">Schedule courses</param>
        /// <returns>Created schedule</returns>
        Task<Schedule> CreateAsync(string dayType, DateTimeOffset effectiveDate, DateTimeOffset? expiryDate, IEnumerable<ScheduleCourseData> courses);

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <param name="expiryDate">Expiry date (optional)</param>
        /// <param name="isActive">Is active (optional)</param>
        /// <returns>Updated schedule or null if not found</returns>
        Task<Schedule> UpdateAsync(int id, DateTimeOffset? expiryDate = null, bool? isActive = null);
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

