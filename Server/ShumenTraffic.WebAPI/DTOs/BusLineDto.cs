using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.WebAPI.DTOs
{
    /// <summary>
    /// DTO for Bus Line.
    /// </summary>
    public class BusLineDto
    {
        /// <summary>
        /// Bus line ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Line number (e.g., "1", "2A", "5B").
        /// </summary>
        [Required(ErrorMessage = "Line number is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Line number must be between 1 and 50 characters")]
        public string LineNumber { get; set; }

        /// <summary>
        /// Detailed description of the line.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the bus line is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for creating a new Bus Line.
    /// </summary>
    public class CreateBusLineDto
    {
        /// <summary>
        /// Line number (e.g., "1", "2A", "5B").
        /// </summary>
        [Required(ErrorMessage = "Line number is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Line number must be between 1 and 50 characters")]
        public string LineNumber { get; set; }

        /// <summary>
        /// Detailed description of the line.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO for updating a Bus Line.
    /// </summary>
    public class UpdateBusLineDto
    {
        /// <summary>
        /// Line number (e.g., "1", "2A", "5B").
        /// </summary>
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Line number must be between 1 and 50 characters")]
        public string LineNumber { get; set; }

        /// <summary>
        /// Detailed description of the line.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the bus line is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}

