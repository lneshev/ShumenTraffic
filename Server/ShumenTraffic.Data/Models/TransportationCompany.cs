using System;
using System.Collections.Generic;

namespace ShumenTraffic.Data.Models
{
    /// <summary>
    /// Represents a transportation company operating bus lines.
    /// </summary>
    public class TransportationCompany
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Company name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Company description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if the company is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when the record was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Timestamp when the record was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation properties
        /// <summary>
        /// Collection of bus lines operated by this company (many-to-many).
        /// </summary>
        public ICollection<TransportationCompanyBusLine> TransportationCompanyBusLines { get; set; } = new List<TransportationCompanyBusLine>();
    }
}