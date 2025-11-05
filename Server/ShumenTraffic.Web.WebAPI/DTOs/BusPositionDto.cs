using System;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.WebAPI.DTOs
{
    /// <summary>
    /// DTO for current bus position.
    /// </summary>
    public class BusPositionDto
    {
        /// <summary>
        /// Bus ID (unique identifier for the bus).
        /// </summary>
        [Required(ErrorMessage = "Bus ID is required")]
        public string BusId { get; set; }

        /// <summary>
        /// Route ID.
        /// </summary>
        [Required(ErrorMessage = "Route ID is required")]
        public int RouteId { get; set; }

        /// <summary>
        /// Bus line number.
        /// </summary>
        public string BusLineNumber { get; set; }

        /// <summary>
        /// Route direction.
        /// </summary>
        public int Direction { get; set; }

        /// <summary>
        /// Current latitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Current longitude coordinate (WGS84/EPSG:4326).
        /// </summary>
        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// Current speed in km/h (optional, for real GPS mode).
        /// </summary>
        public decimal? Speed { get; set; }

        /// <summary>
        /// Current heading/bearing in degrees (0-360, optional).
        /// </summary>
        [Range(0, 360, ErrorMessage = "Heading must be between 0 and 360")]
        public decimal? Heading { get; set; }

        /// <summary>
        /// Timestamp of the position update.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Position calculation mode: "calculated" or "gps".
        /// </summary>
        public string Mode { get; set; } = "calculated";

        /// <summary>
        /// Current stop index (0-based) for calculated mode.
        /// </summary>
        public int? CurrentStopIndex { get; set; }

        /// <summary>
        /// Next stop index (0-based) for calculated mode.
        /// </summary>
        public int? NextStopIndex { get; set; }

        /// <summary>
        /// Progress percentage between current and next stop (0-100) for calculated mode.
        /// </summary>
        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100")]
        public decimal? ProgressPercentage { get; set; }

        /// <summary>
        /// Estimated time to next stop in minutes (for calculated mode).
        /// </summary>
        public int? EstimatedMinutesToNextStop { get; set; }
    }

    /// <summary>
    /// DTO for requesting current bus position.
    /// </summary>
    public class BusPositionRequestDto
    {
        /// <summary>
        /// Mode for position calculation: "calculated" (default) or "gps".
        /// </summary>
        public string Mode { get; set; } = "calculated";

        /// <summary>
        /// Current time for calculated mode (defaults to current UTC time).
        /// </summary>
        public TimeSpan? CurrentTime { get; set; }
    }
}

