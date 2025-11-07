using ShumenTraffic.Common.Core.Constants.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Common.Core.Entities
{
    /// <summary>
    /// Represents a transportation company operating bus lines.
    /// </summary>
    public class TransportationCompany : TrackableEntityBase<int>
    {
        /// <summary>
        /// Company name.
        /// </summary>
        [Required]
        [MaxLength(TransportationCompanyConstants.NameMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Company description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if the company is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Collection of bus lines operated by this company (many-to-many).
        /// </summary>
        public ICollection<TransportationCompanyBusLine> TransportationCompanyBusLines { get; set; } = new List<TransportationCompanyBusLine>();
    }
}