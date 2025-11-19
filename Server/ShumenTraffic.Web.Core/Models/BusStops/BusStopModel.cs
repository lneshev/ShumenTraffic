using MoravianStar.Dao;
using NetTopologySuite.Geometries;
using ShumenTraffic.Common.Core.Attributes;
using ShumenTraffic.Common.Core.Constants.BusStops;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.BusStops
{
    /// <summary>
    /// DTO for Bus Stop.
    /// </summary>
    public class BusStopModel : ModelBase<int>
    {
        /// <summary>
        /// Bus stop name.
        /// </summary>
        [Required]
        [MaxLength(BusStopConstants.NameMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Zone ID.
        /// </summary>
        [Required]
        public int ZoneId { get; set; }

        /// <summary>
        /// Zone name.
        /// </summary>
        public string ZoneName { get; set; }

        /// <summary>
        /// Bus stop's GPS location
        /// </summary>
        [Required]
        [PointRange]
        public Point Location { get; set; }

        /// <summary>
        /// Whether the bus stop is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}