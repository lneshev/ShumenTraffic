using MoravianStar.Dao;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models
{
    /// <summary>
    /// DTO for Transportation Company.
    /// </summary>
    public class TransportationCompanyModel : ModelBase<int>
    {
        /// <summary>
        /// Company name.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Company description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the company is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}