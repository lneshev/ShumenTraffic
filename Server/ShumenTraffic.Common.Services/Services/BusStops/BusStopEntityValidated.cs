using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Filters.BusStops;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.BusStops
{
    public class BusStopEntityValidated : IEntityValidated<BusStop>
    {
        public async Task ValidatedAsync(BusStop entity, BusStop originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            var bsFilter = new BusStopFilter() { NameEqualsInsensitive = entity.Name, ExcludeIds = new List<int>() { entity.Id } };
            var bsExist = await Persistence.ForEntity<BusStop>().ExistAsync(bsFilter);
            if (bsExist)
            {
                throw new EntityNotUniqueException(string.Format(Strings.BusStopWithNameAlreadyExists, entity.Name));
            }
        }
    }
}