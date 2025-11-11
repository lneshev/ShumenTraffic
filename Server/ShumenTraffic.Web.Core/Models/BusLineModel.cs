using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models
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
        public List<int> TransportationCompanyIds { get; set; } = new List<int>();

        /// <summary>
        /// Gets or sets the name of the transportation company.
        /// </summary>
        public List<string> TransportationCompanyNames { get; set; } = new List<string>();
    }
}