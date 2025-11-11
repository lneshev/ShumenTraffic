using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusLines;
using System;

namespace ShumenTraffic.Common.Core.Entities.TransportationCompanies
{
    /// <summary>
    /// Junction table for the many-to-many relationship between TransportationCompany and BusLine.
    /// </summary>
    public class TransportationCompanyBusLine : IEntityBase
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
        public virtual TransportationCompany TransportationCompany { get; set; }

        /// <summary>
        /// The bus line.
        /// </summary>
        public virtual BusLine BusLine { get; set; }

        public bool IsNew()
        {
            throw new NotImplementedException();
        }
    }
}