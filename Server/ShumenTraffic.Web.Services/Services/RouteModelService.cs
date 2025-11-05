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
    /// Service for Route operations.
    /// </summary>
    public class RouteModelService : BaseModelService<Route, RouteModel>, IRouteModelService
    {
        public RouteModelService(AppDbContext context) : base(context)
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

        protected override RouteModel MapToDto(Route entity)
        {
            return new RouteModel
            {
                Id = entity.Id,
                BusLineId = entity.BusLineId,
                BusLineNumber = entity.BusLine?.LineNumber,
                Direction = entity.Direction,
                Name = entity.Name,
                IsActive = entity.IsActive,
                Stops = entity.RouteStops
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs => new RouteStopDto
                    {
                        Id = rs.Id,
                        BusStopId = rs.BusStopId,
                        BusStopName = rs.BusStop?.Name,
                        Latitude = rs.Latitude,
                        Longitude = rs.Longitude,
                        StopOrder = rs.StopOrder,
                        EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                    })
                    .ToList()
            };
        }

        public async Task<IEnumerable<RouteModel>> GetAllAsync(int? busLineId = null, bool includeInactive = false)
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

            var routes = await query
                .OrderBy(r => r.BusLineId)
                .ThenBy(r => r.Direction)
                .Select(r => new RouteModel
                {
                    Id = r.Id,
                    BusLineId = r.BusLineId,
                    BusLineNumber = r.BusLine.LineNumber,
                    Direction = r.Direction,
                    Name = r.Name,
                    IsActive = r.IsActive,
                    Stops = r.RouteStops
                        .OrderBy(rs => rs.StopOrder)
                        .Select(rs => new RouteStopDto
                        {
                            Id = rs.Id,
                            BusStopId = rs.BusStopId,
                            BusStopName = rs.BusStop.Name,
                            Latitude = rs.Latitude,
                            Longitude = rs.Longitude,
                            StopOrder = rs.StopOrder,
                            EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                        })
                        .ToList()
                })
                .ToListAsync();

            return routes;
        }

        public override async Task<RouteModel> GetByIdAsync(int id)
        {
            var route = await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .Where(r => r.Id == id)
                .Select(r => new RouteModel
                {
                    Id = r.Id,
                    BusLineId = r.BusLineId,
                    BusLineNumber = r.BusLine.LineNumber,
                    Direction = r.Direction,
                    Name = r.Name,
                    IsActive = r.IsActive,
                    Stops = r.RouteStops
                        .OrderBy(rs => rs.StopOrder)
                        .Select(rs => new RouteStopDto
                        {
                            Id = rs.Id,
                            BusStopId = rs.BusStopId,
                            BusStopName = rs.BusStop.Name,
                            Latitude = rs.Latitude,
                            Longitude = rs.Longitude,
                            StopOrder = rs.StopOrder,
                            EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return route;
        }

        public async Task<(RouteModel dto, string error)> CreateAsync(CreateRouteDto dto)
        {
            // Verify bus line exists
            var busLine = await _context.BusLines.FindAsync(dto.BusLineId);
            if (busLine == null)
            {
                return (null, $"Bus line with ID {dto.BusLineId} does not exist");
            }

            // Verify bus stops exist (if provided)
            var busStopIds = dto.Stops.Where(s => s.BusStopId.HasValue).Select(s => s.BusStopId.Value).ToList();
            if (busStopIds.Any())
            {
                var existingStops = await _context.BusStops.Where(b => busStopIds.Contains(b.Id)).CountAsync();
                if (existingStops != busStopIds.Count)
                {
                    return (null, "One or more bus stops do not exist");
                }
            }

            var route = new Route
            {
                BusLineId = dto.BusLineId,
                Direction = dto.Direction,
                Name = dto.Name,
                IsActive = true
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            // Add route stops
            foreach (var stopDto in dto.Stops.OrderBy(s => s.StopOrder))
            {
                var routeStop = new RouteStop
                {
                    RouteId = route.Id,
                    BusStopId = stopDto.BusStopId,
                    Latitude = stopDto.Latitude,
                    Longitude = stopDto.Longitude,
                    StopOrder = stopDto.StopOrder,
                    EstimatedMinutesFromStart = stopDto.EstimatedMinutesFromStart
                };
                _context.RouteStops.Add(routeStop);
            }

            await _context.SaveChangesAsync();

            // Reload route with stops
            var createdRoute = await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .FirstAsync(r => r.Id == route.Id);

            var result = new RouteModel
            {
                Id = createdRoute.Id,
                BusLineId = createdRoute.BusLineId,
                BusLineNumber = createdRoute.BusLine.LineNumber,
                Direction = createdRoute.Direction,
                Name = createdRoute.Name,
                IsActive = createdRoute.IsActive,
                Stops = createdRoute.RouteStops
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs => new RouteStopDto
                    {
                        Id = rs.Id,
                        BusStopId = rs.BusStopId,
                        BusStopName = rs.BusStop?.Name,
                        Latitude = rs.Latitude,
                        Longitude = rs.Longitude,
                        StopOrder = rs.StopOrder,
                        EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                    })
                    .ToList()
            };

            return (result, null);
        }

        public async Task<RouteModel> UpdateAsync(int id, UpdateRouteDto dto)
        {
            var route = await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return null;
            }

            if (dto.Direction.HasValue)
                route.Direction = dto.Direction.Value;
            if (dto.Name != null)
                route.Name = dto.Name;
            if (dto.IsActive.HasValue)
                route.IsActive = dto.IsActive.Value;

            _context.Routes.Update(route);
            await _context.SaveChangesAsync();

            var result = new RouteModel
            {
                Id = route.Id,
                BusLineId = route.BusLineId,
                BusLineNumber = route.BusLine.LineNumber,
                Direction = route.Direction,
                Name = route.Name,
                IsActive = route.IsActive,
                Stops = route.RouteStops
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs => new RouteStopDto
                    {
                        Id = rs.Id,
                        BusStopId = rs.BusStopId,
                        BusStopName = rs.BusStop?.Name,
                        Latitude = rs.Latitude,
                        Longitude = rs.Longitude,
                        StopOrder = rs.StopOrder,
                        EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                    })
                    .ToList()
            };

            return result;
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