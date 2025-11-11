using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.TransportationCompanies;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.TransportationCompanies
{
    /// <summary>
    /// Model for Transportation Company.
    /// </summary>
    public class TransportationCompanyModel : ModelBase<int>
    {
        /// <summary>
        /// Company name.
        /// </summary>
        [Required]
        [MaxLength(TransportationCompanyConstants.NameMaxLength)]
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