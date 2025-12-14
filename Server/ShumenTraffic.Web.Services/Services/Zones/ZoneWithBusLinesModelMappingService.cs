using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Web.Core.Models.BusLines;
using ShumenTraffic.Web.Core.Models.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShumenTraffic.Web.Services.Services.Zones
{
    public class ZoneWithBusLinesModelMappingService : ModelsMappingService<ZoneWithBusLinesModel, Zone>
    {
        public override Expression<Func<Zone, IProjectionBase>> Project()
        {
            return x => new ZoneWithBusLinesModel()
            {
                Id = x.Id,
                Name = x.Name,
                BusLines = x.BusStops
                    .SelectMany(bs => bs.RouteStops)
                    .Select(rs => rs.Route.BusLine)
                    .Distinct()
                    .Select(bl => new BusLineLightModel { Id = bl.Id, LineNumber = bl.LineNumber })
                    .ToList()
            };
        }
    }
}