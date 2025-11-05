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
    /// Service for Transportation Company operations.
    /// </summary>
    public class TransportationCompanyModelService : ITransportationCompanyModelService
    {
        private readonly ITransportationCompanyService _transportationCompanyService;

        public TransportationCompanyModelService(ITransportationCompanyService transportationCompanyService)
        {
            _transportationCompanyService = transportationCompanyService;
        }

        private TransportationCompanyModel MapToModel(TransportationCompany entity)
        {
            return new TransportationCompanyModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        public async Task<IEnumerable<TransportationCompanyModel>> GetAllAsync(bool includeInactive = false)
        {
            var entities = await _transportationCompanyService.GetAllAsync(includeInactive);
            return entities.Select(MapToModel);
        }

        public async Task<TransportationCompanyModel> GetByIdAsync(int id)
        {
            var entity = await _transportationCompanyService.GetByIdAsync(id);
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _transportationCompanyService.DeleteAsync(id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            return await _transportationCompanyService.NameExistsAsync(name, excludeId);
        }

        public async Task<(TransportationCompanyModel dto, string error)> CreateAsync(CreateTransportationCompanyDto dto)
        {
            // Check if transportation company already exists
            if (await _transportationCompanyService.NameExistsAsync(dto.Name))
            {
                return (null, $"A transportation company with name '{dto.Name}' already exists");
            }

            var entity = await _transportationCompanyService.CreateAsync(dto.Name, dto.Description);

            return (MapToModel(entity), null);
        }

        public async Task<(TransportationCompanyModel dto, string error)> UpdateAsync(int id, UpdateTransportationCompanyDto dto)
        {
            // Check if new name already exists
            if (!string.IsNullOrEmpty(dto.Name) && await _transportationCompanyService.NameExistsAsync(dto.Name, id))
            {
                return (null, $"A transportation company with name '{dto.Name}' already exists");
            }

            var entity = await _transportationCompanyService.UpdateAsync(
                id,
                dto.Name,
                dto.Description,
                dto.IsActive
            );

            if (entity == null)
            {
                return (null, $"No company found with ID {id}");
            }

            return (MapToModel(entity), null);
        }
    }
}