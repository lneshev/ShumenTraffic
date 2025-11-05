using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models
{
    /// <summary>
    /// DTO for Transportation Company.
    /// </summary>
    public class TransportationCompanyDto
    {
        /// <summary>
        /// Company ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Company name.
        /// </summary>
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Company name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Company description.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the company is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for creating a new Transportation Company.
    /// </summary>
    public class CreateTransportationCompanyDto
    {
        /// <summary>
        /// Company name.
        /// </summary>
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Company name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Company description.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO for updating a Transportation Company.
    /// </summary>
    public class UpdateTransportationCompanyDto
    {
        /// <summary>
        /// Company name.
        /// </summary>
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Company name must be between 1 and 256 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Company description.
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Whether the company is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}

