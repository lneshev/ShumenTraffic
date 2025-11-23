using MoravianStar.Dao;
using MoravianStar.Exceptions;
using MoravianStar.Extensions;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Filters.Routes;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Routes
{
    public class RouteEntityValidated : IEntityValidated<Route>
    {
        public async Task ValidatedAsync(Route entity, Route originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            var bsFilter = new RouteFilter()
            {
                NameEqualsInsensitive = entity.Name,
                BusLineId = entity.BusLineId,
                Direction = entity.Direction,
                ExcludeIds = new List<int>() { entity.Id }
            };
            var bsExist = await Persistence.ForEntity<Route>().ExistAsync(bsFilter);
            if (bsExist)
            {
                throw new EntityNotUniqueException(
                    string.Format(
                        Strings.RouteWithNameBusLineAndDirectionAlreadyExists,
                        entity.Name,
                        entity.BusLine.LineNumber,
                        entity.Direction.Translate(typeof(Strings))
                    )
                );
            }
        }
    }
}