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
    /// Service for Zone operations.
    /// </summary>
    public class ZoneModelService : IZoneModelService
    {
        private readonly IZoneService _zoneService;

        public ZoneModelService(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        private ZoneModel MapToModel(Zone entity)
        {
            return new ZoneModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        public async Task<IEnumerable<ZoneModel>> GetAllAsync(bool includeInactive = false)
        {
            var entities = await _zoneService.GetAllAsync(includeInactive);
            return entities.Select(MapToModel);
        }

        public async Task<ZoneModel> GetByIdAsync(int id)
        {
            var entity = await _zoneService.GetByIdAsync(id);
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _zoneService.DeleteAsync(id);
        }

        public async Task<ZoneModel> CreateAsync(CreateZoneDto dto)
        {
            var entity = await _zoneService.CreateAsync(dto.Name, dto.Description);

            return MapToModel(entity);
        }

        public async Task<ZoneModel> UpdateAsync(int id, UpdateZoneDto dto)
        {
            var entity = await _zoneService.UpdateAsync(
                id,
                dto.Name,
                dto.Description,
                dto.IsActive
            );

            return entity != null ? MapToModel(entity) : null;
        }
    }
}