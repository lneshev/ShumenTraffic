using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Base service interface for common CRUD operations.
    /// </summary>
    /// <typeparam name="TDto">The DTO type</typeparam>
    public interface IBaseService<TDto> where TDto : class
    {
        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <param name="includeInactive">Include inactive entities</param>
        /// <returns>List of DTOs</returns>
        Task<IEnumerable<TDto>> GetAllAsync(bool includeInactive = false);

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>DTO or null if not found</returns>
        Task<TDto> GetByIdAsync(int id);

        /// <summary>
        /// Delete entity by ID.
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id);
    }
}

