using ShumenTraffic.Web.Core.Models;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Interfaces
{
    /// <summary>
    /// Service interface for Transportation Company operations.
    /// </summary>
    public interface ITransportationCompanyModelService : IBaseModelService<TransportationCompanyModel>
    {
        /// <summary>
        /// Create a new transportation company.
        /// </summary>
        /// <param name="dto">Create transportation company DTO</param>
        /// <returns>Created company DTO or null if validation fails</returns>
        Task<(TransportationCompanyModel dto, string error)> CreateAsync(CreateTransportationCompanyDto dto);

        /// <summary>
        /// Update an existing transportation company.
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="dto">Update transportation company DTO</param>
        /// <returns>Updated company DTO or null if not found/validation fails</returns>
        Task<(TransportationCompanyModel dto, string error)> UpdateAsync(int id, UpdateTransportationCompanyDto dto);

        /// <summary>
        /// Check if a transportation company with the given name exists.
        /// </summary>
        /// <param name="name">Name to check</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
    }
}