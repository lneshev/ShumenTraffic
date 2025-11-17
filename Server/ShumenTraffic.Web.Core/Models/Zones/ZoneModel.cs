using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.Zones;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Zones
{
    /// <summary>
    /// DTO for Zone.
    /// </summary>
    public class ZoneModel : ModelBase<int>
    {
        /// <summary>
        /// Zone name.
        /// </summary>
        [Required]
        [MaxLength(ZoneConstants.NameMaxLength)]
        public string Name { get; set; }

        /// <summary>
        /// Zone description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the zone is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}