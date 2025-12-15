using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.BusLines;
using ShumenTraffic.Web.Core.Models.TransportationCompanies;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.BusLines
{
    /// <summary>
    /// DTO for Bus Line.
    /// </summary>
    public class BusLineModel : ModelBase<int>
    {
        /// <summary>
        /// Line number (e.g., "1", "2A", "5B").
        /// </summary>
        [Required]
        [MaxLength(BusLineConstants.LineNumberMaxLength)]
        public string LineNumber { get; set; }

        /// <summary>
        /// Detailed description of the line.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the bus line is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the unique identifier for the transportation company.
        /// </summary>
        public IEnumerable<TransportationCompanyLightModel> TransportationCompanies { get; set; } = new List<TransportationCompanyLightModel>();
    }
}