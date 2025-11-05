using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Persistence.DbContexts;
using ShumenTraffic.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services
{
    /// <summary>
    /// Base service class with common CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TDto">The DTO type</typeparam>
    public abstract class BaseModelService<TEntity, TDto> : IBaseModelService<TDto>
        where TEntity : class
        where TDto : class
    {
        protected readonly AppDbContext _context;

        protected BaseModelService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the DbSet for the entity.
        /// </summary>
        protected abstract DbSet<TEntity> GetDbSet();

        /// <summary>
        /// Build query with includes.
        /// </summary>
        protected abstract IQueryable<TEntity> BuildQuery(IQueryable<TEntity> query);

        /// <summary>
        /// Apply active filter to query.
        /// </summary>
        protected abstract IQueryable<TEntity> ApplyActiveFilter(IQueryable<TEntity> query, bool includeInactive);

        /// <summary>
        /// Map entity to DTO.
        /// </summary>
        protected abstract TDto MapToDto(TEntity entity);

        /// <summary>
        /// Get all entities.
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> GetAllAsync(bool includeInactive = false)
        {
            var query = GetDbSet().AsQueryable();
            query = BuildQuery(query);
            query = ApplyActiveFilter(query, includeInactive);

            var entities = await query.ToListAsync();
            return entities.Select(MapToDto);
        }

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            var query = GetDbSet().AsQueryable();
            query = BuildQuery(query);

            var entity = await FindByIdAsync(query, id);
            return entity != null ? MapToDto(entity) : null;
        }

        /// <summary>
        /// Find entity by ID in the query.
        /// </summary>
        protected abstract Task<TEntity> FindByIdAsync(IQueryable<TEntity> query, int id);

        /// <summary>
        /// Delete entity by ID.
        /// </summary>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetDbSet().FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            GetDbSet().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Check if entity exists by ID.
        /// </summary>
        protected async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetDbSet().FindAsync(id);
            return entity != null;
        }
    }
}