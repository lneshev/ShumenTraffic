using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.Services.Interfaces;
using ShumenTraffic.Persistence.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services
{
    /// <summary>
    /// Service for Bus Stop entity operations.
    /// </summary>
    public class BusStopService : BaseEntityService<BusStop>, IBusStopService
    {
        public BusStopService(AppDbContext context) : base(context)
        {
        }

        protected override DbSet<BusStop> GetDbSet() => _context.BusStops;

        protected override IQueryable<BusStop> BuildQuery(IQueryable<BusStop> query)
        {
            return query.Include(b => b.Zone);
        }

        protected override IQueryable<BusStop> ApplyActiveFilter(IQueryable<BusStop> query, bool includeInactive)
        {
            if (!includeInactive)
            {
                query = query.Where(b => b.IsActive);
            }
            return query;
        }

        protected override async Task<BusStop> FindByIdAsync(IQueryable<BusStop> query, int id)
        {
            return await query.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BusStop>> GetAllWithZonesAsync(int? zoneId = null, bool includeInactive = false)
        {
            var query = _context.BusStops.Include(b => b.Zone).AsQueryable();

            if (zoneId.HasValue)
            {
                query = query.Where(b => b.ZoneId == zoneId.Value);
            }

            if (!includeInactive)
            {
                query = query.Where(b => b.IsActive);
            }

            return await query.OrderBy(b => b.Name).ToListAsync();
        }

        public async Task<BusStop> GetByIdWithZoneAsync(int id)
        {
            return await _context.BusStops
                .Include(b => b.Zone)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BusStop> CreateAsync(string name, int zoneId, decimal latitude, decimal longitude)
        {
            var busStop = new BusStop
            {
                Name = name,
                ZoneId = zoneId,
                Latitude = latitude,
                Longitude = longitude,
                IsActive = true
            };

            _context.BusStops.Add(busStop);
            await _context.SaveChangesAsync();

            return await GetByIdWithZoneAsync(busStop.Id);
        }

        public async Task<BusStop> UpdateAsync(int id, string name = null, int? zoneId = null, decimal? latitude = null, decimal? longitude = null, bool? isActive = null)
        {
            var busStop = await _context.BusStops.FindAsync(id);

            if (busStop == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(name))
            {
                busStop.Name = name;
            }
            if (zoneId.HasValue)
            {
                busStop.ZoneId = zoneId.Value;
            }
            if (latitude.HasValue)
            {
                busStop.Latitude = latitude.Value;
            }
            if (longitude.HasValue)
            {
                busStop.Longitude = longitude.Value;
            }
            if (isActive.HasValue)
            {
                busStop.IsActive = isActive.Value;
            }

            _context.BusStops.Update(busStop);
            await _context.SaveChangesAsync();

            return await GetByIdWithZoneAsync(id);
        }
    }
}

