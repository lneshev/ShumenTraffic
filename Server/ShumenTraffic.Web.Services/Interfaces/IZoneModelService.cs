using ShumenTraffic.Web.Core.Models;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Interfaces
{
    /// <summary>
    /// Service interface for Zone operations.
    /// </summary>
    public interface IZoneModelService : IBaseModelService<ZoneModel>
    {
        /// <summary>
        /// Create a new zone.
        /// </summary>
        /// <param name="dto">Create zone DTO</param>
        /// <returns>Created zone DTO</returns>
        Task<ZoneModel> CreateAsync(CreateZoneDto dto);

        /// <summary>
        /// Update an existing zone.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <param name="dto">Update zone DTO</param>
        /// <returns>Updated zone DTO or null if not found</returns>
        Task<ZoneModel> UpdateAsync(int id, UpdateZoneDto dto);
    }
}