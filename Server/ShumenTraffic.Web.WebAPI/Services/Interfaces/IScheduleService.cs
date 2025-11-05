using ShumenTraffic.Web.WebAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for Schedule operations.
    /// </summary>
    public interface IScheduleService : IBaseService<ScheduleDto>
    {
        /// <summary>
        /// Get all schedules, optionally filtered by day type.
        /// </summary>
        /// <param name="dayType">Filter by day type (optional)</param>
        /// <param name="includeInactive">Include inactive schedules</param>
        /// <returns>List of schedule DTOs</returns>
        Task<IEnumerable<ScheduleDto>> GetAllAsync(string dayType = null, bool includeInactive = false);

        /// <summary>
        /// Create a new schedule with courses.
        /// </summary>
        /// <param name="dto">Create schedule DTO</param>
        /// <returns>Created schedule DTO or null if validation fails</returns>
        Task<(ScheduleDto dto, string error)> CreateAsync(CreateScheduleDto dto);

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <param name="dto">Update schedule DTO</param>
        /// <returns>Updated schedule DTO or null if not found</returns>
        Task<ScheduleDto> UpdateAsync(int id, UpdateScheduleDto dto);
    }
}

