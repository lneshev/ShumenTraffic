using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.DataAccess.DbContexts;
using ShumenTraffic.Common.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services
{
    /// <summary>
    /// Service for Bus Line entity operations.
    /// </summary>
    public class BusLineService : BaseEntityService<BusLine>, IBusLineService
    {
        public BusLineService(AppDbContext context) : base(context)
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

        public async Task<IEnumerable<BusLine>> GetAllWithCompaniesAsync(bool includeInactive = false)
        {
            var query = _context.BusLines
                .Include(l => l.TransportationCompanyBusLines)
                .ThenInclude(tc => tc.TransportationCompany)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(l => l.IsActive);
            }

            return await query.OrderBy(l => l.LineNumber).ToListAsync();
        }

        public async Task<BusLine> GetByIdWithCompaniesAsync(int id)
        {
            return await _context.BusLines
                .Include(l => l.TransportationCompanyBusLines)
                .ThenInclude(tc => tc.TransportationCompany)
                .FirstOrDefaultAsync(l => l.Id == id);
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

        public async Task<BusLine> CreateAsync(string lineNumber, string description, IEnumerable<int> transportationCompanyIds)
        {
            var busLine = new BusLine
            {
                LineNumber = lineNumber,
                Description = description,
                IsActive = true
            };

            _context.BusLines.Add(busLine);
            await _context.SaveChangesAsync();

            // Add transportation companies
            var distinctCompanyIds = transportationCompanyIds.Distinct();
            foreach (var companyId in distinctCompanyIds)
            {
                var company = await _context.TransportationCompanies.FindAsync(companyId);
                if (company != null)
                {
                    busLine.TransportationCompanyBusLines.Add(new TransportationCompanyBusLine
                    {
                        TransportationCompanyId = companyId,
                        TransportationCompany = company
                    });
                }
            }

            await _context.SaveChangesAsync();

            return await GetByIdWithCompaniesAsync(busLine.Id);
        }

        public async Task<BusLine> UpdateAsync(int id, string lineNumber = null, string description = null, bool? isActive = null)
        {
            var busLine = await _context.BusLines.FindAsync(id);

            if (busLine == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(lineNumber))
            {
                busLine.LineNumber = lineNumber;
            }
            if (description != null)
            {
                busLine.Description = description;
            }
            if (isActive.HasValue)
            {
                busLine.IsActive = isActive.Value;
            }

            _context.BusLines.Update(busLine);
            await _context.SaveChangesAsync();

            return await GetByIdWithCompaniesAsync(id);
        }
    }
}

