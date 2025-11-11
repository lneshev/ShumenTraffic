using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.BusStops
{
    /// <summary>
    /// DTO for Bus Stop.
    /// </summary>
    public class BusStopModel
    {
        /// <summary>
        /// Bus stop ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Bus stop name.
        /// </summary>
        [Required(ErrorMessage = "Bus stop name is required")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Bus stop name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Zone ID.
        /// </summary>
        [Required(ErrorMessage = "Zone ID is required")]
        public int ZoneId { get; set; }

        /// <summary>
        /// Zone name.
        /// </summary>
        public string ZoneName { get; set; }

        /// <summary>
        /// Latitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// Whether the bus stop is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for creating a new Bus Stop.
    /// </summary>
    public class CreateBusStopDto
    {
        /// <summary>
        /// Bus stop name.
        /// </summary>
        [Required(ErrorMessage = "Bus stop name is required")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Bus stop name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Zone ID.
        /// </summary>
        [Required(ErrorMessage = "Zone ID is required")]
        public int ZoneId { get; set; }

        /// <summary>
        /// Latitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }
    }

    /// <summary>
    /// DTO for updating a Bus Stop.
    /// </summary>
    public class UpdateBusStopDto
    {
        /// <summary>
        /// Bus stop name.
        /// </summary>
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Bus stop name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Zone ID.
        /// </summary>
        public int? ZoneId { get; set; }

        /// <summary>
        /// Latitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Whether the bus stop is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}

