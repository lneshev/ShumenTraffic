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
    /// Service for Bus Line operations.
    /// </summary>
    public class BusLineModelService : BaseModelService<BusLine, BusLineModel>, IBusLineModelService
    {
        public BusLineModelService(AppDbContext context) : base(context)
        {
        }

        protected override DbSet<BusLine> GetDbSet() => _context.BusLines;

        protected override IQueryable<BusLine> BuildQuery(IQueryable<BusLine> query)
        {
            return query;
        }

        protected override IQueryable<BusLine> ApplyActiveFilter(IQueryable<BusLine> query, bool includeInactive)
        {
            if (!includeInactive)
            {
                query = query.Where(l => l.IsActive);
            }
            return query;
        }

        protected override async Task<BusLine> FindByIdAsync(IQueryable<BusLine> query, int id)
        {
            return await query.FirstOrDefaultAsync(l => l.Id == id);
        }

        protected override BusLineModel MapToDto(BusLine entity)
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

        public override async Task<IEnumerable<BusLineModel>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.BusLines.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(l => l.IsActive);
            }

            var busLines = await query
                .OrderBy(l => l.LineNumber)
                .Select(l => new BusLineModel
                {
                    Id = l.Id,
                    LineNumber = l.LineNumber,
                    Description = l.Description,
                    TransportationCompanyIds = l.TransportationCompanyBusLines.Select(x => x.TransportationCompanyId).ToList(),
                    TransportationCompanyNames = l.TransportationCompanyBusLines.Select(x => x.TransportationCompany.Name).ToList(),
                    IsActive = l.IsActive
                })
                .ToListAsync();

            return busLines;
        }

        public override async Task<BusLineModel> GetByIdAsync(int id)
        {
            var busLine = await _context.BusLines
                .Where(l => l.Id == id)
                .Select(l => new BusLineModel
                {
                    Id = l.Id,
                    LineNumber = l.LineNumber,
                    Description = l.Description,
                    TransportationCompanyIds = l.TransportationCompanyBusLines.Select(x => x.TransportationCompanyId).ToList(),
                    TransportationCompanyNames = l.TransportationCompanyBusLines.Select(x => x.TransportationCompany.Name).ToList(),
                    IsActive = l.IsActive
                })
                .FirstOrDefaultAsync();

            return busLine;
        }

        public async Task<bool> LineNumberExistsAsync(string lineNumber, int? excludeId = null)
        {
            var query = _context.BusLines.Where(l => l.LineNumber == lineNumber);

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<(BusLineModel dto, string error)> CreateAsync(CreateBusLineDto dto)
        {
            // Check if line number already exists
            if (await LineNumberExistsAsync(dto.LineNumber))
            {
                return (null, $"A bus line with number '{dto.LineNumber}' already exists");
            }

            if (!dto.TransportationCompanyIds.Any())
            {
                return (null, "At least one transportation company is required");
            }

            var busLine = new BusLine
            {
                LineNumber = dto.LineNumber,
                Description = dto.Description,
                IsActive = true
            };

            await FillEntityTransportCompanies(busLine, dto);

            _context.BusLines.Add(busLine);
            await _context.SaveChangesAsync();

            var result = new BusLineModel
            {
                Id = busLine.Id,
                LineNumber = busLine.LineNumber,
                Description = busLine.Description,
                TransportationCompanyIds = busLine.TransportationCompanyBusLines.Select(x => x.TransportationCompanyId).ToList(),
                TransportationCompanyNames = busLine.TransportationCompanyBusLines.Select(x => x.TransportationCompany.Name).ToList(),
                IsActive = busLine.IsActive
            };

            return (result, null);
        }

        public async Task<(BusLineModel dto, string error)> UpdateAsync(int id, UpdateBusLineDto dto)
        {
            var busLine = await _context.BusLines.FindAsync(id);

            if (busLine == null)
            {
                return (null, $"No bus line found with ID {id}");
            }

            if (!string.IsNullOrEmpty(dto.LineNumber))
            {
                // Check if new line number already exists
                if (await LineNumberExistsAsync(dto.LineNumber, id))
                {
                    return (null, $"A bus line with number '{dto.LineNumber}' already exists");
                }
                busLine.LineNumber = dto.LineNumber;
            }
            if (dto.Description != null)
            {
                busLine.Description = dto.Description;
            }
            if (dto.IsActive.HasValue)
            {
                busLine.IsActive = dto.IsActive.Value;
            }

            _context.BusLines.Update(busLine);
            await _context.SaveChangesAsync();

            var result = new BusLineModel
            {
                Id = busLine.Id,
                LineNumber = busLine.LineNumber,
                Description = busLine.Description,
                TransportationCompanyIds = busLine.TransportationCompanyBusLines.Select(x => x.TransportationCompanyId).ToList(),
                TransportationCompanyNames = busLine.TransportationCompanyBusLines.Select(x => x.TransportationCompany.Name).ToList(),
                IsActive = busLine.IsActive
            };

            return (result, null);
        }

        private async Task FillEntityTransportCompanies(BusLine entity, CreateBusLineDto model)
        {
            var distinctModelTransportCompanyIds = model.TransportationCompanyIds.Distinct();
            var existingTransportCompanyIds = entity.TransportationCompanyBusLines.Select(x => x.TransportationCompanyId).ToList();

            var transportCompanyIdsToDelete = existingTransportCompanyIds.Except(distinctModelTransportCompanyIds);
            var transportCompanyIdsToInsert = distinctModelTransportCompanyIds.Except(existingTransportCompanyIds);

            if (transportCompanyIdsToDelete.Any() || transportCompanyIdsToInsert.Any())
            {
                foreach (var transportCompanyId in transportCompanyIdsToDelete.ToList())
                {
                    var userRoleEntity = entity.TransportationCompanyBusLines.Single(x => x.TransportationCompanyId == transportCompanyId);
                    entity.TransportationCompanyBusLines.Remove(userRoleEntity);
                }

                foreach (var transportCompanyId in transportCompanyIdsToInsert)
                {
                    entity.TransportationCompanyBusLines.Add(new TransportationCompanyBusLine()
                    {
                        TransportationCompany = await _context.TransportationCompanies.FindAsync(transportCompanyId)
                    });
                }
            }
        }
    }
}