using ShumenTraffic.Common.Core.Entities;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces
{
    /// <summary>
    /// Service interface for Zone entity operations.
    /// </summary>
    public interface IZoneService : IBaseEntityService<Zone>
    {
        /// <summary>
        /// Create a new zone.
        /// </summary>
        /// <param name="name">Zone name</param>
        /// <param name="description">Description</param>
        /// <returns>Created zone</returns>
        Task<Zone> CreateAsync(string name, string description);

        /// <summary>
        /// Update an existing zone.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <param name="name">Name (optional)</param>
        /// <param name="description">Description (optional)</param>
        /// <param name="isActive">Is active (optional)</param>
        /// <returns>Updated zone or null if not found</returns>
        Task<Zone> UpdateAsync(int id, string name = null, string description = null, bool? isActive = null);
    }
}

