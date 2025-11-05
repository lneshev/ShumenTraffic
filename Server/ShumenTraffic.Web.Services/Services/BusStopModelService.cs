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
    /// Service for Bus Stop operations.
    /// </summary>
    public class BusStopModelService : IBusStopModelService
    {
        private readonly IBusStopService _busStopService;
        private readonly IZoneService _zoneService;

        public BusStopModelService(IBusStopService busStopService, IZoneService zoneService)
        {
            _busStopService = busStopService;
            _zoneService = zoneService;
        }

        private BusStopModel MapToModel(BusStop entity)
        {
            return new BusStopModel
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

        public async Task<IEnumerable<BusStopModel>> GetAllAsync(int? zoneId = null, bool includeInactive = false)
        {
            var entities = await _busStopService.GetAllWithZonesAsync(zoneId, includeInactive);
            return entities.Select(MapToModel);
        }

        public async Task<BusStopModel> GetByIdAsync(int id)
        {
            var entity = await _busStopService.GetByIdWithZoneAsync(id);
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _busStopService.DeleteAsync(id);
        }

        public async Task<(BusStopModel dto, string error)> CreateAsync(CreateBusStopDto dto)
        {
            // Verify zone exists
            if (!await _zoneService.ExistsAsync(dto.ZoneId))
            {
                return (null, $"Zone with ID {dto.ZoneId} does not exist");
            }

            var entity = await _busStopService.CreateAsync(
                dto.Name,
                dto.ZoneId,
                dto.Latitude,
                dto.Longitude
            );

            return (MapToModel(entity), null);
        }

        public async Task<(BusStopModel dto, string error)> UpdateAsync(int id, UpdateBusStopDto dto)
        {
            // Verify zone exists if provided
            if (dto.ZoneId.HasValue && !await _zoneService.ExistsAsync(dto.ZoneId.Value))
            {
                return (null, $"Zone with ID {dto.ZoneId} does not exist");
            }

            var entity = await _busStopService.UpdateAsync(
                id,
                dto.Name,
                dto.ZoneId,
                dto.Latitude,
                dto.Longitude,
                dto.IsActive
            );

            if (entity == null)
            {
                return (null, $"No bus stop found with ID {id}");
            }

            return (MapToModel(entity), null);
        }
    }
}