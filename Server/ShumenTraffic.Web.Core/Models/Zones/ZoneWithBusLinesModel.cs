using MoravianStar.Dao;
using ShumenTraffic.Web.Core.Models.BusLines;
using System.Collections.Generic;

namespace ShumenTraffic.Web.Core.Models.Zones
{
    public class ZoneWithBusLinesModel : ModelBase<int>
    {
        public string Name { get; set; }
        public ICollection<BusLineLightModel> BusLines { get; set; }
    }
}