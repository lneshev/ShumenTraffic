using ShumenTraffic.Web.WebAPI.DTOs;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for Zone operations.
    /// </summary>
    public interface IZoneService : IBaseService<ZoneDto>
    {
        /// <summary>
        /// Create a new zone.
        /// </summary>
        /// <param name="dto">Create zone DTO</param>
        /// <returns>Created zone DTO</returns>
        Task<ZoneDto> CreateAsync(CreateZoneDto dto);

        /// <summary>
        /// Update an existing zone.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <param name="dto">Update zone DTO</param>
        /// <returns>Updated zone DTO or null if not found</returns>
        Task<ZoneDto> UpdateAsync(int id, UpdateZoneDto dto);
    }
}

