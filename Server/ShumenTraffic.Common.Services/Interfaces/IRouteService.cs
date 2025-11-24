using NetTopologySuite.Geometries;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Enums.Routes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces
{
    /// <summary>
    /// Service interface for Route entity operations.
    /// </summary>
    public interface IRouteService : IBaseEntityService<Route>
    {
        /// <summary>
        /// Get all routes with their bus lines and stops.
        /// </summary>
        /// <param name="busLineId">Filter by bus line ID (optional)</param>
        /// <param name="includeInactive">Include inactive routes</param>
        /// <returns>List of routes</returns>
        Task<IEnumerable<Route>> GetAllWithDetailsAsync(int? busLineId = null, bool includeInactive = false);

        /// <summary>
        /// Get route by ID with bus line and stops.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>Route or null if not found</returns>
        Task<Route> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Create a new route with stops.
        /// </summary>
        /// <param name="busLineId">Bus line ID</param>
        /// <param name="direction">Direction (1 or 2)</param>
        /// <param name="name">Route name</param>
        /// <param name="stops">Route stops</param>
        /// <returns>Created route</returns>
        Task<Route> CreateAsync(int busLineId, RouteDirection direction, string name, IEnumerable<RouteStopData> stops);

        /// <summary>
        /// Update an existing route.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <param name="direction">Direction (optional)</param>
        /// <param name="name">Name (optional)</param>
        /// <param name="isActive">Is active (optional)</param>
        /// <returns>Updated route or null if not found</returns>
        Task<Route> UpdateAsync(int id, RouteDirection? direction = null, string name = null, bool? isActive = null);
    }

    /// <summary>
    /// Data structure for creating route stops.
    /// </summary>
    public class RouteStopData
    {
        public int? BusStopId { get; set; }
        public Point Location { get; set; }
        public int StopOrder { get; set; }
        public int? EstimatedMinutesFromStart { get; set; }
    }
}