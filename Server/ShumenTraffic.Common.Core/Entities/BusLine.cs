using System;
using System.Collections.Generic;

namespace ShumenTraffic.Common.Core.Entities
{
    /// <summary>
    /// Represents a bus line that can be operated by one or more transportation companies.
    /// </summary>
    public class BusLine
    {
        /// <summary>
        /// Primary key identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Line number (e.g., "1", "2A", "5B").
        /// </summary>
        public required string LineNumber { get; set; }

        /// <summary>
        /// Detailed description of the line.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if the line is active.
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
        /// Collection of transportation companies that operate this line (many-to-many).
        /// </summary>
        public ICollection<TransportationCompanyBusLine> TransportationCompanyBusLines { get; set; } = new List<TransportationCompanyBusLine>();

        /// <summary>
        /// Collection of routes for this bus line.
        /// </summary>
        public ICollection<Route> Routes { get; set; } = new List<Route>();
    }
}