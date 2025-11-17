using ShumenTraffic.Common.Core.Constants.Zones;
using ShumenTraffic.Common.Core.Entities.BusStops;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Common.Core.Entities.Zones
{
    /// <summary>
    /// Represents a geographical zone or neighborhood.
    /// </summary>
    public class Zone : TrackableEntityBase<int>
    {
        /// <summary>
        /// Zone name.
        /// </summary>
        [Required]
        [MaxLength(ZoneConstants.NameMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Zone description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates if the zone is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Collection of bus stops in this zone.
        /// </summary>
        public virtual ICollection<BusStop> BusStops { get; set; } = new List<BusStop>();
    }
}