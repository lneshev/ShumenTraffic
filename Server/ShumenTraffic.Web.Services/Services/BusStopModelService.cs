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
    /// Service for Bus Stop operations.
    /// </summary>
    public class BusStopModelService : BaseModelService<BusStop, BusStopDto>, IBusStopModelService
    {
        public BusStopModelService(AppDbContext context) : base(context)
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

        protected override BusStopDto MapToDto(BusStop entity)
        {
            return new BusStopDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ZoneId = entity.ZoneId,
                ZoneName = entity.Zone?.Name,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                IsActive = entity.IsActive
            };
        }

        public async Task<IEnumerable<BusStopDto>> GetAllAsync(int? zoneId = null, bool includeInactive = false)
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

            var busStops = await query
                .OrderBy(b => b.Name)
                .Select(b => new BusStopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ZoneId = b.ZoneId,
                    ZoneName = b.Zone.Name,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            return busStops;
        }

        public override async Task<BusStopDto> GetByIdAsync(int id)
        {
            var busStop = await _context.BusStops
                .Include(b => b.Zone)
                .Where(b => b.Id == id)
                .Select(b => new BusStopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ZoneId = b.ZoneId,
                    ZoneName = b.Zone.Name,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    IsActive = b.IsActive
                })
                .FirstOrDefaultAsync();

            return busStop;
        }

        public async Task<(BusStopDto dto, string error)> CreateAsync(CreateBusStopDto dto)
        {
            // Verify zone exists
            var zone = await _context.Zones.FindAsync(dto.ZoneId);
            if (zone == null)
            {
                return (null, $"Zone with ID {dto.ZoneId} does not exist");
            }

            var busStop = new BusStop
            {
                Name = dto.Name,
                ZoneId = dto.ZoneId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsActive = true
            };

            _context.BusStops.Add(busStop);
            await _context.SaveChangesAsync();

            var result = new BusStopDto
            {
                Id = busStop.Id,
                Name = busStop.Name,
                ZoneId = busStop.ZoneId,
                ZoneName = zone.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude,
                IsActive = busStop.IsActive
            };

            return (result, null);
        }

        public async Task<(BusStopDto dto, string error)> UpdateAsync(int id, UpdateBusStopDto dto)
        {
            var busStop = await _context.BusStops.Include(b => b.Zone).FirstOrDefaultAsync(b => b.Id == id);

            if (busStop == null)
            {
                return (null, $"No bus stop found with ID {id}");
            }

            if (!string.IsNullOrEmpty(dto.Name))
                busStop.Name = dto.Name;
            if (dto.ZoneId.HasValue)
            {
                var zone = await _context.Zones.FindAsync(dto.ZoneId.Value);
                if (zone == null)
                {
                    return (null, $"Zone with ID {dto.ZoneId} does not exist");
                }
                busStop.ZoneId = dto.ZoneId.Value;
            }
            if (dto.Latitude.HasValue)
                busStop.Latitude = dto.Latitude.Value;
            if (dto.Longitude.HasValue)
                busStop.Longitude = dto.Longitude.Value;
            if (dto.IsActive.HasValue)
                busStop.IsActive = dto.IsActive.Value;

            _context.BusStops.Update(busStop);
            await _context.SaveChangesAsync();

            var result = new BusStopDto
            {
                Id = busStop.Id,
                Name = busStop.Name,
                ZoneId = busStop.ZoneId,
                ZoneName = busStop.Zone.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude,
                IsActive = busStop.IsActive
            };

            return (result, null);
        }
    }
}