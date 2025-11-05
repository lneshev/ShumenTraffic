using ShumenTraffic.Common.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces
{
    /// <summary>
    /// Service interface for Bus Line entity operations.
    /// </summary>
    public interface IBusLineService : IBaseEntityService<BusLine>
    {
        /// <summary>
        /// Get all bus lines with their transportation companies.
        /// </summary>
        /// <param name="includeInactive">Include inactive bus lines</param>
        /// <returns>List of bus lines</returns>
        Task<IEnumerable<BusLine>> GetAllWithCompaniesAsync(bool includeInactive = false);

        /// <summary>
        /// Get bus line by ID with transportation companies.
        /// </summary>
        /// <param name="id">Bus line ID</param>
        /// <returns>Bus line or null if not found</returns>
        Task<BusLine> GetByIdWithCompaniesAsync(int id);

        /// <summary>
        /// Create a new bus line.
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <param name="description">Description</param>
        /// <param name="transportationCompanyIds">Transportation company IDs</param>
        /// <returns>Created bus line</returns>
        Task<BusLine> CreateAsync(string lineNumber, string description, IEnumerable<int> transportationCompanyIds);

        /// <summary>
        /// Update an existing bus line.
        /// </summary>
        /// <param name="id">Bus line ID</param>
        /// <param name="lineNumber">Line number (optional)</param>
        /// <param name="description">Description (optional)</param>
        /// <param name="isActive">Is active (optional)</param>
        /// <returns>Updated bus line or null if not found</returns>
        Task<BusLine> UpdateAsync(int id, string lineNumber = null, string description = null, bool? isActive = null);

        /// <summary>
        /// Check if a bus line with the given line number exists.
        /// </summary>
        /// <param name="lineNumber">Line number to check</param>
        /// <param name="excludeId">ID to exclude from check (for updates)</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> LineNumberExistsAsync(string lineNumber, int? excludeId = null);
    }
}

