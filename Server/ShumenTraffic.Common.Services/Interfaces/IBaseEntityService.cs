using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces
{
    /// <summary>
    /// Base service interface for common CRUD operations on entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IBaseEntityService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <param name="includeInactive">Include inactive entities</param>
        /// <returns>List of entities</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(bool includeInactive = false);

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Entity or null if not found</returns>
        Task<TEntity> GetByIdAsync(int id);

        /// <summary>
        /// Delete entity by ID.
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Check if entity exists by ID.
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> ExistsAsync(int id);
    }
}

