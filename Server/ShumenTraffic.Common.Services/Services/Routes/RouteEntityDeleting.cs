using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Routes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Routes
{
    public class RouteEntityDeleting : IEntityDeleting<Route>
    {
        public async Task DeletingAsync(Route entity, IDictionary<string, object> additionalParameters = null)
        {
            foreach (var routeStop in entity.RouteStops.ToList())
            {
                await Persistence.ForEntity<RouteStop>().DeleteAsync(routeStop);
            }
        }
    }
}