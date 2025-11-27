using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.BusStops;
using ShumenTraffic.Common.Core.Enums.Routes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Routes
{
    /// <summary>
    /// DTO for Route.
    /// </summary>
    public class RouteModel : ModelBase<int>
    {
        /// <summary>
        /// Route name or description.
        /// </summary>
        [Required]
        [MaxLength(BusStopConstants.NameMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Direction of the route.
        /// </summary>
        public RouteDirection Direction { get; set; }

        /// <summary>
        /// Direction of the route as text.
        /// </summary>
        public string DirectionText { get; set; }

        /// <summary>
        /// Whether the route is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bus line ID.
        /// </summary>
        public int BusLineId { get; set; }

        /// <summary>
        /// Bus line number.
        /// </summary>
        public string BusLineNumber { get; set; }

        /// <summary>
        /// Collection of stops and waypoints on this route.
        /// </summary>
        public IEnumerable<RouteStopModel> Stops { get; set; } = new List<RouteStopModel>();
    }
}