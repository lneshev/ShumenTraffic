using ShumenTraffic.Web.Core.Models;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Interfaces
{
    /// <summary>
    /// Service interface for Bus Line operations.
    /// </summary>
    public interface IBusLineModelService : IBaseModelService<BusLineModel>
    {
        /// <summary>
        /// Create a new bus line.
        /// </summary>
        /// <param name="dto">Create bus line DTO</param>
        /// <returns>Created bus line DTO or null if validation fails</returns>
        Task<(BusLineModel dto, string error)> CreateAsync(CreateBusLineDto dto);

        /// <summary>
        /// Update an existing bus line.
        /// </summary>
        /// <param name="id">Bus line ID</param>
        /// <param name="dto">Update bus line DTO</param>
        /// <returns>Updated bus line DTO or null if not found/validation fails</returns>
        Task<(BusLineModel dto, string error)> UpdateAsync(int id, UpdateBusLineDto dto);

        /// <summary>
        /// Check if a bus line with the given line number exists.
        /// </summary>
        /// <param name="lineNumber">Line number to check</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> LineNumberExistsAsync(string lineNumber, int? excludeId = null);
    }
}