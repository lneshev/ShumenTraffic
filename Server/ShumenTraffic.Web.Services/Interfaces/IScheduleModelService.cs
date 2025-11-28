using ShumenTraffic.Common.Core.Enums.Schedules;
using ShumenTraffic.Web.Core.Models.Schedules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Interfaces
{
    /// <summary>
    /// Service interface for Schedule operations.
    /// </summary>
    public interface IScheduleModelService
    {
        /// <summary>
        /// Get all schedules, optionally filtered by day type.
        /// </summary>
        /// <param name="dayType">Filter by day type (optional)</param>
        /// <param name="includeInactive">Include inactive schedules</param>
        /// <returns>List of schedule DTOs</returns>
        Task<IEnumerable<ScheduleModel>> GetAllAsync(DayType? dayType = null, bool includeInactive = false);

        /// <summary>
        /// Get schedule by ID.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>Schedule DTO or null if not found</returns>
        Task<ScheduleModel> GetByIdAsync(int id);

        /// <summary>
        /// Delete schedule by ID.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Create a new schedule with courses.
        /// </summary>
        /// <param name="dto">Create schedule DTO</param>
        /// <returns>Created schedule DTO or null if validation fails</returns>
        Task<(ScheduleModel dto, string error)> CreateAsync(CreateScheduleDto dto);

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <param name="dto">Update schedule DTO</param>
        /// <returns>Updated schedule DTO or null if not found</returns>
        Task<ScheduleModel> UpdateAsync(int id, UpdateScheduleDto dto);
    }
}