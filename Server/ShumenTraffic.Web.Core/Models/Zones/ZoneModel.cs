using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Zones
{
    /// <summary>
    /// DTO for Zone.
    /// </summary>
    public class ZoneModel
    {
        /// <summary>
        /// Zone ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Zone name.
        /// </summary>
        [Required(ErrorMessage = "Zone name is required")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Zone name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Zone description.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the zone is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for creating a new Zone.
    /// </summary>
    public class CreateZoneDto
    {
        /// <summary>
        /// Zone name.
        /// </summary>
        [Required(ErrorMessage = "Zone name is required")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Zone name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Zone description.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO for updating a Zone.
    /// </summary>
    public class UpdateZoneDto
    {
        /// <summary>
        /// Zone name.
        /// </summary>
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Zone name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Zone description.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the zone is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}

