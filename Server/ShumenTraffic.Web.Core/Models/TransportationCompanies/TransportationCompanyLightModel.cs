using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.TransportationCompanies;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.TransportationCompanies
{
    public class TransportationCompanyLightModel : ModelBase<int>
    {
        /// <summary>
        /// Company name.
        /// </summary>
        [Required]
        [MaxLength(TransportationCompanyConstants.NameMaxLength)]
        public string Name { get; set; }
    }
}