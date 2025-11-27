using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using MoravianStar.Extensions;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.BusStops;
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
    public class RouteModelsMappingService : ModelsMappingService<RouteModel, Route>
    {
        public override Expression<Func<Route, IProjectionBase>> Project()
        {
            return x => new RouteModel()
            {
                Id = x.Id,
                Name = x.Name,
                Direction = x.Direction,
                IsActive = x.IsActive,
                BusLineId = x.BusLineId,
                BusLineNumber = x.BusLine.LineNumber,
                Stops = x.RouteStops.OrderBy(x => x.StopOrder).Select(y => new RouteStopModel()
                {
                    Id = y.Id,
                    StopOrder = y.StopOrder,
                    Location = y.Location,
                    BusStopId = y.BusStopId,
                    BusStopName = y.BusStopId.HasValue ? y.BusStop.Name : null,
                    BusStopLocation = y.BusStopId.HasValue ? y.BusStop.Location : null,
                    EstimatedMinutesFromStart = y.EstimatedMinutesFromStart,
                })
            };
        }

        public override async Task<RouteModel> MapAsync(IProjectionBase projection)
        {
            var model = (RouteModel)projection;
            model.DirectionText = model.Direction.Translate(typeof(Strings));
            return await Task.FromResult(model);
        }

        public override IQueryable<Route> GetIncludes(IQueryable<Route> query)
        {
            return base.GetIncludes(query)
                .Include(x => x.BusLine)
                .Include(x => x.RouteStops)
                    .ThenInclude(x => x.BusStop);
        }

        public override async Task<List<EntityModelPair<Route, RouteModel>>> ToEntities(List<EntityModelPair<Route, RouteModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Name = pair.Model.Name;
                pair.Entity.Direction = pair.Model.Direction;
                pair.Entity.IsActive = pair.Model.IsActive;
                pair.Entity.BusLineId = pair.Model.BusLineId;
                pair.Entity.BusLine = await Persistence.ForEntity<BusLine, int>().GetAsync(pair.Entity.BusLineId);
                await FillEntityRouteStops(pair);
            }

            return pairs;
        }

        private async Task FillEntityRouteStops(EntityModelPair<Route, RouteModel> pair)
        {
            var distinctModelStopIds = pair.Model.Stops.Where(x => x.Id > 0).Select(x => x.Id).Distinct();
            var existingStopIds = pair.Entity.RouteStops.Select(x => x.Id);

            var stopIdsToDelete = existingStopIds.Except(distinctModelStopIds);
            var stopsToInsert = pair.Model.Stops.Where(x => x.Id == 0);

            // Delete
            foreach (var stopId in stopIdsToDelete.ToList())
            {
                await Persistence.ForEntity<RouteStop, int>().DeleteAsync(stopId);
            }

            // Update
            foreach (var routeStop in pair.Entity.RouteStops)
            {
                var stopModel = pair.Model.Stops.SingleOrDefault(x => x.Id == routeStop.Id);
                if (stopModel != null)
                {
                    routeStop.StopOrder = stopModel.StopOrder;
                    routeStop.Location = stopModel.Location;
                    routeStop.EstimatedMinutesFromStart = stopModel.EstimatedMinutesFromStart;
                    await Persistence.ForEntity<RouteStop>().SaveAsync(routeStop);
                }
            }

            // Insert
            foreach (var stopModel in stopsToInsert)
            {
                var newStop = new RouteStop()
                {
                    RouteId = pair.Entity.Id,
                    Route = pair.Entity,
                    StopOrder = stopModel.StopOrder,
                    Location = stopModel.Location,
                    BusStopId = stopModel.BusStopId,
                    BusStop = stopModel.BusStopId.HasValue ? await Persistence.ForEntity<BusStop, int>().GetAsync(stopModel.BusStopId.Value) : null,
                    EstimatedMinutesFromStart = stopModel.EstimatedMinutesFromStart
                };
                await Persistence.ForEntity<RouteStop>().SaveAsync(newStop);
            }
        }
    }
}