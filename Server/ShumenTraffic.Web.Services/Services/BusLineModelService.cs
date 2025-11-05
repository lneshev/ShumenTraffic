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
    /// Service for Bus Line operations.
    /// </summary>
    public class BusLineModelService : IBusLineModelService
    {
        private readonly IBusLineService _busLineService;

        public BusLineModelService(IBusLineService busLineService)
        {
            _busLineService = busLineService;
        }

        private BusLineModel MapToModel(BusLine entity)
        {
            return new BusLineModel
            {
                Id = entity.Id,
                LineNumber = entity.LineNumber,
                Description = entity.Description,
                TransportationCompanyIds = entity.TransportationCompanyBusLines.Select(x => x.TransportationCompanyId).ToList(),
                TransportationCompanyNames = entity.TransportationCompanyBusLines.Select(x => x.TransportationCompany.Name).ToList(),
                IsActive = entity.IsActive
            };
        }

        public async Task<IEnumerable<BusLineModel>> GetAllAsync(bool includeInactive = false)
        {
            var entities = await _busLineService.GetAllWithCompaniesAsync(includeInactive);
            return entities.Select(MapToModel);
        }

        public async Task<BusLineModel> GetByIdAsync(int id)
        {
            var entity = await _busLineService.GetByIdWithCompaniesAsync(id);
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _busLineService.DeleteAsync(id);
        }

        public async Task<bool> LineNumberExistsAsync(string lineNumber, int? excludeId = null)
        {
            return await _busLineService.LineNumberExistsAsync(lineNumber, excludeId);
        }

        public async Task<(BusLineModel dto, string error)> CreateAsync(CreateBusLineDto dto)
        {
            // Check if line number already exists
            if (await _busLineService.LineNumberExistsAsync(dto.LineNumber))
            {
                return (null, $"A bus line with number '{dto.LineNumber}' already exists");
            }

            if (!dto.TransportationCompanyIds.Any())
            {
                return (null, "At least one transportation company is required");
            }

            var entity = await _busLineService.CreateAsync(
                dto.LineNumber,
                dto.Description,
                dto.TransportationCompanyIds
            );

            return (MapToModel(entity), null);
        }

        public async Task<(BusLineModel dto, string error)> UpdateAsync(int id, UpdateBusLineDto dto)
        {
            // Check if new line number already exists
            if (!string.IsNullOrEmpty(dto.LineNumber) && await _busLineService.LineNumberExistsAsync(dto.LineNumber, id))
            {
                return (null, $"A bus line with number '{dto.LineNumber}' already exists");
            }

            var entity = await _busLineService.UpdateAsync(
                id,
                dto.LineNumber,
                dto.Description,
                dto.IsActive
            );

            if (entity == null)
            {
                return (null, $"No bus line found with ID {id}");
            }

            return (MapToModel(entity), null);
        }
    }
}