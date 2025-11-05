using ShumenTraffic.Web.WebAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for Bus Stop operations.
    /// </summary>
    public interface IBusStopService : IBaseService<BusStopDto>
    {
        /// <summary>
        /// Get all bus stops, optionally filtered by zone.
        /// </summary>
        /// <param name="zoneId">Filter by zone ID (optional)</param>
        /// <param name="includeInactive">Include inactive bus stops</param>
        /// <returns>List of bus stop DTOs</returns>
        Task<IEnumerable<BusStopDto>> GetAllAsync(int? zoneId = null, bool includeInactive = false);

        /// <summary>
        /// Create a new bus stop.
        /// </summary>
        /// <param name="dto">Create bus stop DTO</param>
        /// <returns>Created bus stop DTO or null if validation fails</returns>
        Task<(BusStopDto dto, string error)> CreateAsync(CreateBusStopDto dto);

        /// <summary>
        /// Update an existing bus stop.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <param name="dto">Update bus stop DTO</param>
        /// <returns>Updated bus stop DTO or null if not found/validation fails</returns>
        Task<(BusStopDto dto, string error)> UpdateAsync(int id, UpdateBusStopDto dto);
    }
}

