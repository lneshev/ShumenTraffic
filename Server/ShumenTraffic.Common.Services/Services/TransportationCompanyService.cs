using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.Services.Interfaces;
using ShumenTraffic.Persistence.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services
{
    /// <summary>
    /// Service for Transportation Company entity operations.
    /// </summary>
    public class TransportationCompanyService : BaseEntityService<TransportationCompany>, ITransportationCompanyService
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

        public override async Task<IEnumerable<TransportationCompany>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.TransportationCompanies.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            return await query.OrderBy(c => c.Name).ToListAsync();
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

        public async Task<TransportationCompany> CreateAsync(string name, string description)
        {
            var company = new TransportationCompany
            {
                Name = name,
                Description = description,
                IsActive = true
            };

            _context.TransportationCompanies.Add(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<TransportationCompany> UpdateAsync(int id, string name = null, string description = null, bool? isActive = null)
        {
            var company = await _context.TransportationCompanies.FindAsync(id);

            if (company == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(name))
            {
                company.Name = name;
            }
            if (description != null)
            {
                company.Description = description;
            }
            if (isActive.HasValue)
            {
                company.IsActive = isActive.Value;
            }

            _context.TransportationCompanies.Update(company);
            await _context.SaveChangesAsync();

            return company;
        }
    }
}

