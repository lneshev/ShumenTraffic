using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.Services.Interfaces;
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
    public class RouteModelService : IRouteModelService
    {
        private readonly IRouteService _routeService;
        private readonly IBusStopService _busStopService;

        public RouteModelService(IRouteService routeService, IBusStopService busStopService)
        {
            _routeService = routeService;
            _busStopService = busStopService;
        }

        private RouteModel MapToModel(Route entity)
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
            var entities = await _routeService.GetAllWithDetailsAsync(busLineId, includeInactive);
            return entities.Select(MapToModel);
        }

        public async Task<RouteModel> GetByIdAsync(int id)
        {
            var entity = await _routeService.GetByIdWithDetailsAsync(id);
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _routeService.DeleteAsync(id);
        }

        public async Task<(RouteModel dto, string error)> CreateAsync(CreateRouteDto dto)
        {
            // Verify bus line exists
            // TODO: Change to use ExistsAsync once available in MoravianStar
            var busLine = await Persistence.ForEntity<BusLine, int>().GetAsync(dto.BusLineId);

            // Verify bus stops exist (if provided)
            var busStopIds = dto.Stops.Where(s => s.BusStopId.HasValue).Select(s => s.BusStopId.Value).ToList();
            foreach (var busStopId in busStopIds)
            {
                if (!await _busStopService.ExistsAsync(busStopId))
                {
                    return (null, "One or more bus stops do not exist");
                }
            }

            var stops = dto.Stops.Select(s => new RouteStopData
            {
                BusStopId = s.BusStopId,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                StopOrder = s.StopOrder,
                EstimatedMinutesFromStart = s.EstimatedMinutesFromStart
            });

            var entity = await _routeService.CreateAsync(
                dto.BusLineId,
                dto.Direction,
                dto.Name,
                stops
            );

            return (MapToModel(entity), null);
        }

        public async Task<RouteModel> UpdateAsync(int id, UpdateRouteDto dto)
        {
            var entity = await _routeService.UpdateAsync(
                id,
                dto.Direction,
                dto.Name,
                dto.IsActive
            );

            return entity != null ? MapToModel(entity) : null;
        }
    }
}