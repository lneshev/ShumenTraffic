using ShumenTraffic.Common.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces
{
    /// <summary>
    /// Service interface for Bus Stop entity operations.
    /// </summary>
    public interface IBusStopService : IBaseEntityService<BusStop>
    {
        /// <summary>
        /// Get all bus stops with their zones.
        /// </summary>
        /// <param name="zoneId">Filter by zone ID (optional)</param>
        /// <param name="includeInactive">Include inactive bus stops</param>
        /// <returns>List of bus stops</returns>
        Task<IEnumerable<BusStop>> GetAllWithZonesAsync(int? zoneId = null, bool includeInactive = false);

        /// <summary>
        /// Get bus stop by ID with zone.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <returns>Bus stop or null if not found</returns>
        Task<BusStop> GetByIdWithZoneAsync(int id);

        /// <summary>
        /// Create a new bus stop.
        /// </summary>
        /// <param name="name">Bus stop name</param>
        /// <param name="zoneId">Zone ID</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>Created bus stop</returns>
        Task<BusStop> CreateAsync(string name, int zoneId, decimal latitude, decimal longitude);

        /// <summary>
        /// Update an existing bus stop.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <param name="name">Name (optional)</param>
        /// <param name="zoneId">Zone ID (optional)</param>
        /// <param name="latitude">Latitude (optional)</param>
        /// <param name="longitude">Longitude (optional)</param>
        /// <param name="isActive">Is active (optional)</param>
        /// <returns>Updated bus stop or null if not found</returns>
        Task<BusStop> UpdateAsync(int id, string name = null, int? zoneId = null, decimal? latitude = null, decimal? longitude = null, bool? isActive = null);
    }
}

