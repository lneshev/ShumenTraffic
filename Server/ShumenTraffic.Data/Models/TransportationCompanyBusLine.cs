using System;

namespace ShumenTraffic.Data.Models
{
    /// <summary>
    /// Junction table for the many-to-many relationship between TransportationCompany and BusLine.
    /// </summary>
    public class TransportationCompanyBusLine
    {
        /// <summary>
        /// Foreign key to the transportation company.
        /// </summary>
        public int TransportationCompanyId { get; set; }

        /// <summary>
        /// Foreign key to the bus line.
        /// </summary>
        public int BusLineId { get; set; }

        // Navigation properties
        /// <summary>
        /// The transportation company.
        /// </summary>
        public TransportationCompany TransportationCompany { get; set; }

        /// <summary>
        /// The bus line.
        /// </summary>
        public BusLine BusLine { get; set; }
    }
}

