using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Routes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Routes
{
    public class RouteEntitySaving : IEntitySaving<Route>
    {
        public async Task SavingAsync(Route entity, Route originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            await FixAndCompactRouteStopOrders(entity);
        }

        private static async Task FixAndCompactRouteStopOrders(Route entity)
        {
            var orderedRouteStops = entity.RouteStops.OrderBy(x => x.StopOrder).ToList();
            for (var i = 0; i < orderedRouteStops.Count - 1; i++)
            {
                var currentRouteStop = orderedRouteStops[i];
                var nextRouteStop = orderedRouteStops[i + 1];

                if (i == 0 && currentRouteStop.StopOrder != 1)
                {
                    currentRouteStop.StopOrder = 1;
                    await Persistence.ForEntity<RouteStop>().SaveAsync(currentRouteStop);
                }

                if (currentRouteStop.StopOrder + 1 != nextRouteStop.StopOrder)
                {
                    nextRouteStop.StopOrder = currentRouteStop.StopOrder + 1;
                    await Persistence.ForEntity<RouteStop>().SaveAsync(currentRouteStop);
                }
            }
        }
    }
}