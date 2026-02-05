using MoravianStar.Dao;
using MoravianStar.Exceptions;
using MoravianStar.Extensions;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Filters.Routes;
using ShumenTraffic.Common.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Routes
{
    public class RouteEntityValidated : IEntityValidated<Route>
    {
        public async Task ValidatedAsync(Route entity, Route originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            await CheckForUniqueness(entity);
            ChecksForRouteStops(entity);
        }

        private static async Task CheckForUniqueness(Route entity)
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

        private static void ChecksForRouteStops(Route entity)
        {
            var orderedRouteStops = entity.RouteStops.OrderBy(x => x.StopOrder).ToList();
            if (orderedRouteStops.Count > 0)
            {
                if (!orderedRouteStops[0].BusStopId.HasValue)
                {
                    throw new BusinessException(Strings.TheRouteShouldStartWithABusStop);
                }
                if (!orderedRouteStops[orderedRouteStops.Count - 1].BusStopId.HasValue)
                {
                    throw new BusinessException(Strings.TheRouteShouldEndWithABusStop);
                }
            }

            int previousRouteStopEstMins = 0;
            for (var i = 1; i < orderedRouteStops.Count; i++)
            {
                var previousRouteStop = orderedRouteStops[i - 1];
                var currentRouteStop = orderedRouteStops[i];

                if (previousRouteStop.StopOrder >= currentRouteStop.StopOrder)
                {
                    throw new BusinessException(Strings.TheRouteCannotBeSavedBecauseTheStopOrderIsNotCorrect);
                }

                previousRouteStopEstMins = Math.Max(previousRouteStopEstMins, previousRouteStop.EstimatedMinutesFromStart ?? 0);
                if (currentRouteStop.EstimatedMinutesFromStart < previousRouteStopEstMins)
                {
                    throw new BusinessException(Strings.TheRouteCannotBeSavedBecauseTheEstimatedMinutesFromStartAreNotCorrect);
                }
            }
        }
    }
}