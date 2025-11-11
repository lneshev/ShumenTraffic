using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.DataAccess.DbContexts;
using ShumenTraffic.Common.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Routes
{
    /// <summary>
    /// Service for Route entity operations.
    /// </summary>
    public class RouteService : BaseEntityService<Route>, IRouteService
    {
        public RouteService(AppDbContext context) : base(context)
        {
        }

        protected override DbSet<Route> GetDbSet() => _context.Routes;

        protected override IQueryable<Route> BuildQuery(IQueryable<Route> query)
        {
            return query
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop);
        }

        protected override IQueryable<Route> ApplyActiveFilter(IQueryable<Route> query, bool includeInactive)
        {
            if (!includeInactive)
            {
                query = query.Where(r => r.IsActive);
            }
            return query;
        }

        protected override async Task<Route> FindByIdAsync(IQueryable<Route> query, int id)
        {
            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Route>> GetAllWithDetailsAsync(int? busLineId = null, bool includeInactive = false)
        {
            var query = _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .AsQueryable();

            if (busLineId.HasValue)
            {
                query = query.Where(r => r.BusLineId == busLineId.Value);
            }

            if (!includeInactive)
            {
                query = query.Where(r => r.IsActive);
            }

            return await query
                .OrderBy(r => r.BusLineId)
                .ThenBy(r => r.Direction)
                .ToListAsync();
        }

        public async Task<Route> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Route> CreateAsync(int busLineId, int direction, string name, IEnumerable<RouteStopData> stops)
        {
            var route = new Route
            {
                BusLineId = busLineId,
                Direction = direction,
                Name = name,
                IsActive = true
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            // Add route stops
            foreach (var stopData in stops.OrderBy(s => s.StopOrder))
            {
                var routeStop = new RouteStop
                {
                    RouteId = route.Id,
                    BusStopId = stopData.BusStopId,
                    Latitude = stopData.Latitude,
                    Longitude = stopData.Longitude,
                    StopOrder = stopData.StopOrder,
                    EstimatedMinutesFromStart = stopData.EstimatedMinutesFromStart
                };
                _context.RouteStops.Add(routeStop);
            }

            await _context.SaveChangesAsync();

            return await GetByIdWithDetailsAsync(route.Id);
        }

        public async Task<Route> UpdateAsync(int id, int? direction = null, string name = null, bool? isActive = null)
        {
            var route = await _context.Routes.FindAsync(id);

            if (route == null)
            {
                return null;
            }

            if (direction.HasValue)
            {
                route.Direction = direction.Value;
            }
            if (name != null)
            {
                route.Name = name;
            }
            if (isActive.HasValue)
            {
                route.IsActive = isActive.Value;
            }

            _context.Routes.Update(route);
            await _context.SaveChangesAsync();

            return await GetByIdWithDetailsAsync(id);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var route = await _context.Routes
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return false;
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

