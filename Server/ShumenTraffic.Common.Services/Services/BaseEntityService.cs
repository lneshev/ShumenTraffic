using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.DataAccess.DbContexts;
using ShumenTraffic.Common.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services
{
    /// <summary>
    /// Base service class with common CRUD operations on entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public abstract class BaseEntityService<TEntity> : IBaseEntityService<TEntity>
        where TEntity : class
    {
        protected readonly AppDbContext _context;

        protected BaseEntityService(AppDbContext context)
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
        /// Find entity by ID in the query.
        /// </summary>
        protected abstract Task<TEntity> FindByIdAsync(IQueryable<TEntity> query, int id);

        /// <summary>
        /// Get all entities.
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(bool includeInactive = false)
        {
            var query = GetDbSet().AsQueryable();
            query = BuildQuery(query);
            query = ApplyActiveFilter(query, includeInactive);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get entity by ID.
        /// </summary>
        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            var query = GetDbSet().AsQueryable();
            query = BuildQuery(query);

            return await FindByIdAsync(query, id);
        }

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
        public virtual async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetDbSet().FindAsync(id);
            return entity != null;
        }
    }
}

