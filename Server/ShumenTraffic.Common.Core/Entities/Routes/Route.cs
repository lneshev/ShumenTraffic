using ShumenTraffic.Common.Core.Constants.Routes;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Enums.Routes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Common.Core.Entities.Routes
{
    /// <summary>
    /// Represents a specific route for a bus line with direction.
    /// </summary>
    public class Route : TrackableEntityBase<int>
    {
        /// <summary>
        /// Route name or description.
        /// </summary>
        [Required]
        [MaxLength(RouteConstants.NameMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Direction of the route.
        /// </summary>
        public RouteDirection Direction { get; set; }

        /// <summary>
        /// Indicates if the route is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Foreign key to the bus line.
        /// </summary>
        public int BusLineId { get; set; }

        // Navigation properties
        /// <summary>
        /// The bus line this route belongs to.
        /// </summary>
        public virtual BusLine BusLine { get; set; }

        /// <summary>
        /// Collection of stops on this route.
        /// </summary>
        public virtual ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();

        /// <summary>
        /// Collection of courses (trips/departures) that use this route.
        /// </summary>
        public virtual ICollection<ScheduleCourse> ScheduleCourses { get; set; } = new List<ScheduleCourse>();
    }
}