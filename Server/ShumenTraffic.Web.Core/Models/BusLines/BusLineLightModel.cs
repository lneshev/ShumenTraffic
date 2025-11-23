using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Constants.BusLines;
using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.BusLines
{
    /// <summary>
    /// DTO for Bus Line.
    /// </summary>
    public class BusLineLightModel : ModelBase<int>
    {
        /// <summary>
        /// Line number (e.g., "1", "2A", "5B").
        /// </summary>
        [Required]
        [MaxLength(BusLineConstants.LineNumberMaxLength)]
        public string LineNumber { get; set; }
    }
}