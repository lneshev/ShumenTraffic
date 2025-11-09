using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.Core.Filters;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services
{
    public class BusLineEntityValidated : IEntityValidated<BusLine>
    {
        public async Task ValidatedAsync(BusLine entity, BusLine originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            var blFilter = new BusLineFilter() { LineNumberEquals = entity.LineNumber, ExcludeIds = new List<int>() { entity.Id } };
            var blExists = await MoravianStar.Dao.Persistence.ForEntity<BusLine>().ExistAsync(blFilter);
            if (blExists)
            {
                throw new EntityNotUniqueException(string.Format(Strings.BusLineWithLineNumberAlreadyExists, entity.LineNumber));
            }
        }
    }
}