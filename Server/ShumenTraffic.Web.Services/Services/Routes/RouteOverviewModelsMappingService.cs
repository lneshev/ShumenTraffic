using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using MoravianStar.Extensions;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Resources;
using ShumenTraffic.Web.Core.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services.Routes
{
    public class RouteOverviewModelsMappingService : ModelsMappingService<RouteOverviewModel, Route>
    {
        public override Expression<Func<Route, IProjectionBase>> Project()
        {
            return x => new RouteOverviewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Direction = x.Direction,
                IsActive = x.IsActive,
                BusLineId = x.BusLineId,
                BusLineNumber = x.BusLine.LineNumber
            };
        }

        public override async Task<RouteOverviewModel> MapAsync(IProjectionBase projection)
        {
            var model = (RouteOverviewModel)projection;
            model.DirectionText = model.Direction.Translate(typeof(Strings));
            return await Task.FromResult(model);
        }

        public override IQueryable<Route> GetIncludes(IQueryable<Route> query)
        {
            return base.GetIncludes(query)
                .Include(x => x.BusLine);
        }

        public override async Task<List<EntityModelPair<Route, RouteOverviewModel>>> ToEntities(List<EntityModelPair<Route, RouteOverviewModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Id = pair.Model.Id;
                pair.Entity.Name = pair.Model.Name;
                pair.Entity.Direction = pair.Model.Direction;
                pair.Entity.IsActive = pair.Model.IsActive;
                pair.Entity.BusLineId = pair.Model.BusLineId;
                pair.Entity.BusLine = await Persistence.ForEntity<BusLine, int>().GetAsync(pair.Entity.BusLineId);
            }

            return pairs;
        }
    }
}