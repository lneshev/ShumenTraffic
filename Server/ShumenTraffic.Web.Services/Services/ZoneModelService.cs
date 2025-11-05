using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Persistence.DbContexts;
using ShumenTraffic.Web.Core.Models;
using ShumenTraffic.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services
{
    /// <summary>
    /// Service for Zone operations.
    /// </summary>
    public class ZoneModelService : BaseModelService<Zone, ZoneModel>, IZoneModelService
    {
        public ZoneModelService(AppDbContext context) : base(context)
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

        protected override ZoneModel MapToDto(Zone entity)
        {
            return new ZoneModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        public override async Task<IEnumerable<ZoneModel>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Zones.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(z => z.IsActive);
            }

            var zones = await query
                .OrderBy(z => z.Name)
                .Select(z => new ZoneModel
                {
                    Id = z.Id,
                    Name = z.Name,
                    Description = z.Description,
                    IsActive = z.IsActive
                })
                .ToListAsync();

            return zones;
        }

        public override async Task<ZoneModel> GetByIdAsync(int id)
        {
            var zone = await _context.Zones
                .Where(z => z.Id == id)
                .Select(z => new ZoneModel
                {
                    Id = z.Id,
                    Name = z.Name,
                    Description = z.Description,
                    IsActive = z.IsActive
                })
                .FirstOrDefaultAsync();

            return zone;
        }

        public async Task<ZoneModel> CreateAsync(CreateZoneDto dto)
        {
            var zone = new Zone
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };

            _context.Zones.Add(zone);
            await _context.SaveChangesAsync();

            return new ZoneModel
            {
                Id = zone.Id,
                Name = zone.Name,
                Description = zone.Description,
                IsActive = zone.IsActive
            };
        }

        public async Task<ZoneModel> UpdateAsync(int id, UpdateZoneDto dto)
        {
            var zone = await _context.Zones.FindAsync(id);

            if (zone == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Name))
                zone.Name = dto.Name;
            if (dto.Description != null)
                zone.Description = dto.Description;
            if (dto.IsActive.HasValue)
                zone.IsActive = dto.IsActive.Value;

            _context.Zones.Update(zone);
            await _context.SaveChangesAsync();

            return new ZoneModel
            {
                Id = zone.Id,
                Name = zone.Name,
                Description = zone.Description,
                IsActive = zone.IsActive
            };
        }
    }
}