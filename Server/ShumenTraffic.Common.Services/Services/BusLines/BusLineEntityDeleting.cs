using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Entities.TransportationCompanies;
using ShumenTraffic.Common.Core.Filters.Routes;
using ShumenTraffic.Common.Core.Filters.Schedules;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.BusLines
{
    public class BusLineEntityDeleting : IEntityDeleting<BusLine>
    {
        public async Task DeletingAsync(BusLine entity, IDictionary<string, object> additionalParameters = null)
        {
            var routeFilter = new RouteFilter() { BusLineId = entity.Id };
            var hasRoutes = await Persistence.ForEntity<Route>().ExistAsync(routeFilter);
            if (hasRoutes)
            {
                throw new BusinessException(string.Format(Strings.YouAreNotAllowedToDeleteBusLineBecauseItHasRoutes, entity.LineNumber));
            }

            var scheduleFilter = new ScheduleFilter() { BusLineId = entity.Id };
            var hasSchedules = await Persistence.ForEntity<Schedule>().ExistAsync(scheduleFilter);
            if (hasSchedules)
            {
                throw new BusinessException(string.Format(Strings.YouAreNotAllowedToDeleteBusLineBecauseItHasSchedules, entity.LineNumber));
            }

            foreach (var transportCompanyBusLine in entity.TransportationCompanyBusLines.ToList())
            {
                await Persistence.ForEntity<TransportationCompanyBusLine>().DeleteAsync(transportCompanyBusLine);
            }
        }
    }
}