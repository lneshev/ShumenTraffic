using ShumenTraffic.Common.Core.Entities;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces
{
    /// <summary>
    /// Service interface for Transportation Company entity operations.
    /// </summary>
    public interface ITransportationCompanyService : IBaseEntityService<TransportationCompany>
    {
        /// <summary>
        /// Create a new transportation company.
        /// </summary>
        /// <param name="name">Company name</param>
        /// <param name="description">Description</param>
        /// <returns>Created transportation company</returns>
        Task<TransportationCompany> CreateAsync(string name, string description);

        /// <summary>
        /// Update an existing transportation company.
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="name">Name (optional)</param>
        /// <param name="description">Description (optional)</param>
        /// <param name="isActive">Is active (optional)</param>
        /// <returns>Updated transportation company or null if not found</returns>
        Task<TransportationCompany> UpdateAsync(int id, string name = null, string description = null, bool? isActive = null);

        /// <summary>
        /// Check if a transportation company with the given name exists.
        /// </summary>
        /// <param name="name">Name to check</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}

