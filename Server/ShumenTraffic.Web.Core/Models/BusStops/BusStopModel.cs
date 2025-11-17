using MoravianStar.Dao;
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
        /// Latitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// Whether the bus stop is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}