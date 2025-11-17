using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Common.Core.Filters.Zones;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Zones
{
    public class ZoneEntityValidated : IEntityValidated<Zone>
    {
        public async Task ValidatedAsync(Zone entity, Zone originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            var zFilter = new ZoneFilter() { NameEquals = entity.Name, ExcludeIds = new List<int>() { entity.Id } };
            var zExist = await Persistence.ForEntity<Zone>().ExistAsync(zFilter);
            if (zExist)
            {
                throw new EntityNotUniqueException(string.Format(Strings.ZoneWithNameAlreadyExists, entity.Name));
            }
        }
    }
}