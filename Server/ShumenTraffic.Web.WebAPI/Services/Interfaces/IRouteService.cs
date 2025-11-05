using ShumenTraffic.Web.WebAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Service interface for Route operations.
    /// </summary>
    public interface IRouteService : IBaseService<RouteDto>
    {
        /// <summary>
        /// Get all routes, optionally filtered by bus line.
        /// </summary>
        /// <param name="busLineId">Filter by bus line ID (optional)</param>
        /// <param name="includeInactive">Include inactive routes</param>
        /// <returns>List of route DTOs</returns>
        Task<IEnumerable<RouteDto>> GetAllAsync(int? busLineId = null, bool includeInactive = false);

        /// <summary>
        /// Create a new route with stops.
        /// </summary>
        /// <param name="dto">Create route DTO</param>
        /// <returns>Created route DTO or null if validation fails</returns>
        Task<(RouteDto dto, string error)> CreateAsync(CreateRouteDto dto);

        /// <summary>
        /// Update an existing route.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <param name="dto">Update route DTO</param>
        /// <returns>Updated route DTO or null if not found</returns>
        Task<RouteDto> UpdateAsync(int id, UpdateRouteDto dto);
    }
}

