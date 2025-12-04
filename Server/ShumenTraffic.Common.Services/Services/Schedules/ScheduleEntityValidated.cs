using MoravianStar.Dao;
using MoravianStar.Exceptions;
using MoravianStar.Extensions;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Filters.Schedules;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Schedules
{
    public class ScheduleEntityValidated : IEntityValidated<Schedule>
    {
        public async Task ValidatedAsync(Schedule entity, Schedule originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            await CheckForUniqueness(entity);
            CheckForRoutes(entity);
        }

        private static async Task CheckForUniqueness(Schedule entity)
        {
            var scheduleFilter = new ScheduleFilter()
            {
                BusLineId = entity.BusLineId,
                Direction = entity.Direction,
                DayType = entity.DayType,
                StartDateLE = entity.EndDate,
                EndDateGEOrNull = entity.StartDate,
                Priority = entity.Priority,
                ExcludeIds = new List<int>() { entity.Id }
            };
            var existingSchedules = (await Persistence.ForEntity<Schedule>().ReadAsync(scheduleFilter, projection: x => new ProjectionBase<int>() { Id = x.Id }, trackable: false)).Items;
            if (existingSchedules.Any())
            {
                throw new EntityNotUniqueException(string.Format(
                    Strings.OneOrMoreSchedulesForBusLineExist,
                    entity.BusLine.LineNumber,
                    entity.Direction.Translate(typeof(Strings)),
                    entity.DayType.Translate(typeof(Strings)),
                    string.Join(", ", existingSchedules.Select(x => x.Id))
                ));
            }
        }

        private static void CheckForRoutes(Schedule entity)
        {
            if (entity.ScheduleCourses.Any(x => x.Route.BusLineId != entity.BusLineId || x.Route.Direction != entity.Direction))
            {
                throw new BusinessException(string.Format(Strings.AllScheduleCoursesInScheduleMustHaveRoutesThatAreForSchedulesBusLineAndDirection, entity.Id));
            }
        }
    }
}