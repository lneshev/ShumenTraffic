using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Persistence.DbContexts;
using ShumenTraffic.Web.WebAPI.DTOs;
using ShumenTraffic.Web.WebAPI.Services.Base;
using ShumenTraffic.Web.WebAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Services.Implementations
{
    /// <summary>
    /// Service for Transportation Company operations.
    /// </summary>
    public class TransportationCompanyService : BaseService<TransportationCompany, TransportationCompanyDto>, ITransportationCompanyService
    {
        public TransportationCompanyService(AppDbContext context) : base(context)
        {
        }

        protected override DbSet<TransportationCompany> GetDbSet() => _context.TransportationCompanies;

        protected override IQueryable<TransportationCompany> BuildQuery(IQueryable<TransportationCompany> query)
        {
            return query;
        }

        protected override IQueryable<TransportationCompany> ApplyActiveFilter(IQueryable<TransportationCompany> query, bool includeInactive)
        {
            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }
            return query;
        }

        protected override async Task<TransportationCompany> FindByIdAsync(IQueryable<TransportationCompany> query, int id)
        {
            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        protected override TransportationCompanyDto MapToDto(TransportationCompany entity)
        {
            return new TransportationCompanyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        public override async Task<IEnumerable<TransportationCompanyDto>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.TransportationCompanies.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            var companies = await query
                .OrderBy(c => c.Name)
                .Select(c => new TransportationCompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return companies;
        }

        public override async Task<TransportationCompanyDto> GetByIdAsync(int id)
        {
            var company = await _context.TransportationCompanies
                .Where(c => c.Id == id)
                .Select(c => new TransportationCompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

            return company;
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.TransportationCompanies.Where(c => c.Name == name);
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<(TransportationCompanyDto dto, string error)> CreateAsync(CreateTransportationCompanyDto dto)
        {
            // Check if transportation company already exists
            if (await NameExistsAsync(dto.Name))
            {
                return (null, $"A transportation company with name '{dto.Name}' already exists");
            }

            var company = new TransportationCompany
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };

            _context.TransportationCompanies.Add(company);
            await _context.SaveChangesAsync();

            var result = new TransportationCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                IsActive = company.IsActive
            };

            return (result, null);
        }

        public async Task<(TransportationCompanyDto dto, string error)> UpdateAsync(int id, UpdateTransportationCompanyDto dto)
        {
            var company = await _context.TransportationCompanies.FindAsync(id);

            if (company == null)
            {
                return (null, $"No company found with ID {id}");
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                // Check if new name already exists
                if (await NameExistsAsync(dto.Name, id))
                {
                    return (null, $"A transportation company with name '{dto.Name}' already exists");
                }
                company.Name = dto.Name;
            }
            if (dto.Description != null)
            {
                company.Description = dto.Description;
            }
            if (dto.IsActive.HasValue)
            {
                company.IsActive = dto.IsActive.Value;
            }

            _context.TransportationCompanies.Update(company);
            await _context.SaveChangesAsync();

            var result = new TransportationCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                IsActive = company.IsActive
            };

            return (result, null);
        }
    }
}

