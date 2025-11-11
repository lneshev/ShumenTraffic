using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Common.DataAccess.DbContexts;
using ShumenTraffic.Common.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Zones
{
    /// <summary>
    /// Service for Zone entity operations.
    /// </summary>
    public class ZoneService : BaseEntityService<Zone>, IZoneService
    {
        public ZoneService(AppDbContext context) : base(context)
        {
        }

        protected override DbSet<Zone> GetDbSet() => _context.Zones;

        protected override IQueryable<Zone> BuildQuery(IQueryable<Zone> query)
        {
            return query;
        }

        protected override IQueryable<Zone> ApplyActiveFilter(IQueryable<Zone> query, bool includeInactive)
        {
            if (!includeInactive)
            {
                query = query.Where(z => z.IsActive);
            }
            return query;
        }

        protected override async Task<Zone> FindByIdAsync(IQueryable<Zone> query, int id)
        {
            return await query.FirstOrDefaultAsync(z => z.Id == id);
        }

        public override async Task<IEnumerable<Zone>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Zones.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(z => z.IsActive);
            }

            return await query.OrderBy(z => z.Name).ToListAsync();
        }

        public async Task<Zone> CreateAsync(string name, string description)
        {
            var zone = new Zone
            {
                Name = name,
                Description = description,
                IsActive = true
            };

            _context.Zones.Add(zone);
            await _context.SaveChangesAsync();

            return zone;
        }

        public async Task<Zone> UpdateAsync(int id, string name = null, string description = null, bool? isActive = null)
        {
            var zone = await _context.Zones.FindAsync(id);

            if (zone == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(name))
            {
                zone.Name = name;
            }
            if (description != null)
            {
                zone.Description = description;
            }
            if (isActive.HasValue)
            {
                zone.IsActive = isActive.Value;
            }

            _context.Zones.Update(zone);
            await _context.SaveChangesAsync();

            return zone;
        }
    }
}

