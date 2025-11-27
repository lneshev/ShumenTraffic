using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Filters.Routes;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.BusStops
{
    public class BusStopEntityDeleting : IEntityDeleting<BusStop>
    {
        public async Task DeletingAsync(BusStop entity, IDictionary<string, object> additionalParameters = null)
        {
            var routeStopFilter = new RouteStopFilter() { BusStopId = entity.Id };
            var hasRouteStops = await Persistence.ForEntity<RouteStop>().ExistAsync(routeStopFilter);
            if (hasRouteStops)
            {
                throw new BusinessException(string.Format(Strings.YouAreNotAllowedToDeleteBusStop, entity.Name));
            }
        }
    }
}