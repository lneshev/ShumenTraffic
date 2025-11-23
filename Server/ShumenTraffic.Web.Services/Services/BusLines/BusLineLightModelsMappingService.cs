using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Web.Core.Models.BusLines;
using System;
using System.Linq.Expressions;

namespace ShumenTraffic.Web.Services.Services.BusLines
{
    public class BusLineLightModelsMappingService : ModelsMappingService<BusLineLightModel, BusLine>
    {
        public override Expression<Func<BusLine, IProjectionBase>> Project()
        {
            return x => new BusLineLightModel()
            {
                Id = x.Id,
                LineNumber = x.LineNumber
            };
        }
    }
}