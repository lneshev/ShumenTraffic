using ShumenTraffic.Web.Core.Models.Routes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Interfaces
{
    /// <summary>
    /// Service interface for Route operations.
    /// </summary>
    public interface IRouteModelService
    {
        /// <summary>
        /// Get all routes, optionally filtered by bus line.
        /// </summary>
        /// <param name="busLineId">Filter by bus line ID (optional)</param>
        /// <param name="includeInactive">Include inactive routes</param>
        /// <returns>List of route DTOs</returns>
        Task<IEnumerable<RouteModel>> GetAllAsync(int? busLineId = null, bool includeInactive = false);

        /// <summary>
        /// Get route by ID.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>Route DTO or null if not found</returns>
        Task<RouteModel> GetByIdAsync(int id);

        /// <summary>
        /// Delete route by ID.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Create a new route with stops.
        /// </summary>
        /// <param name="dto">Create route DTO</param>
        /// <returns>Created route DTO or null if validation fails</returns>
        Task<(RouteModel dto, string error)> CreateAsync(CreateRouteDto dto);

        /// <summary>
        /// Update an existing route.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <param name="dto">Update route DTO</param>
        /// <returns>Updated route DTO or null if not found</returns>
        Task<RouteModel> UpdateAsync(int id, UpdateRouteDto dto);
    }
}