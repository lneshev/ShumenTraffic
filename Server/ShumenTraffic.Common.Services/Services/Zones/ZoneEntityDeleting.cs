using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Common.Core.Filters.BusStops;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Zones
{
    public class ZoneEntityDeleting : IEntityDeleting<Zone>
    {
        public async Task DeletingAsync(Zone entity, IDictionary<string, object> additionalParameters = null)
        {
            var busStopFilter = new BusStopFilter() { ZoneId = entity.Id };
            var hasBusStops = await Persistence.ForEntity<BusStop>().ExistAsync(busStopFilter);
            if (hasBusStops)
            {
                throw new BusinessException(string.Format(Strings.YouAreNotAllowedToDeleteZone, entity.Name));
            }
        }
    }
}