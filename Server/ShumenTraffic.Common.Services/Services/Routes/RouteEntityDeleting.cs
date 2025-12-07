using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Filters.Schedules;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Routes
{
    public class RouteEntityDeleting : IEntityDeleting<Route>
    {
        public async Task DeletingAsync(Route entity, IDictionary<string, object> additionalParameters = null)
        {
            var scheduleCourseFilter = new ScheduleCourseFilter() { RouteId = entity.Id };
            var hasScheduleCourses = await Persistence.ForEntity<ScheduleCourse>().ExistAsync(scheduleCourseFilter);
            if (hasScheduleCourses)
            {
                throw new BusinessException(string.Format(Strings.YouAreNotAllowedToDeleteRoute, entity.Name));
            }

            foreach (var routeStop in entity.RouteStops.ToList())
            {
                await Persistence.ForEntity<RouteStop>().DeleteAsync(routeStop);
            }
        }
    }
}