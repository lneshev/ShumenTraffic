using ShumenTraffic.Common.Core.Constants.BusLines;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Entities.TransportationCompanies;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Common.Core.Entities.BusLines
{
    /// <summary>
    /// Represents a bus line that can be operated by one or more transportation companies.
    /// </summary>
    public class BusLine : TrackableEntityBase<int>
    {
        /// <summary>
        /// Line number (e.g., "1", "2A", "5B").
        /// </summary>
        [Required]
        [MaxLength(BusLineConstants.LineNumberMaxLength)]
        public string LineNumber { get; set; }

        /// <summary>
        /// Line number stored in a way appropriate for number-first sorting
        /// </summary>
        [Required]
        [MaxLength(BusLineConstants.LineNumberSortKeyMinLength)]
        public string LineNumberSortKey { get; set; }

        /// <summary>
        /// Detailed description of the line.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if the line is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Collection of transportation companies that operate this line (many-to-many).
        /// </summary>
        public virtual ICollection<TransportationCompanyBusLine> TransportationCompanyBusLines { get; set; } = new List<TransportationCompanyBusLine>();

        /// <summary>
        /// Collection of routes for this bus line.
        /// </summary>
        public virtual ICollection<Route> Routes { get; set; } = new List<Route>();

        /// <summary>
        /// Collection of schedules for this bus line.
        /// </summary>
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}