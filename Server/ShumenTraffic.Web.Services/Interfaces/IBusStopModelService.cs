using ShumenTraffic.Web.Core.Models.BusStops;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Interfaces
{
    /// <summary>
    /// Service interface for Bus Stop operations.
    /// </summary>
    public interface IBusStopModelService
    {
        /// <summary>
        /// Get all bus stops, optionally filtered by zone.
        /// </summary>
        /// <param name="zoneId">Filter by zone ID (optional)</param>
        /// <param name="includeInactive">Include inactive bus stops</param>
        /// <returns>List of bus stop DTOs</returns>
        Task<IEnumerable<BusStopModel>> GetAllAsync(int? zoneId = null, bool includeInactive = false);

        /// <summary>
        /// Get bus stop by ID.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <returns>Bus stop DTO or null if not found</returns>
        Task<BusStopModel> GetByIdAsync(int id);

        /// <summary>
        /// Delete bus stop by ID.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Create a new bus stop.
        /// </summary>
        /// <param name="dto">Create bus stop DTO</param>
        /// <returns>Created bus stop DTO or null if validation fails</returns>
        Task<(BusStopModel dto, string error)> CreateAsync(CreateBusStopDto dto);

        /// <summary>
        /// Update an existing bus stop.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <param name="dto">Update bus stop DTO</param>
        /// <returns>Updated bus stop DTO or null if not found/validation fails</returns>
        Task<(BusStopModel dto, string error)> UpdateAsync(int id, UpdateBusStopDto dto);
    }
}