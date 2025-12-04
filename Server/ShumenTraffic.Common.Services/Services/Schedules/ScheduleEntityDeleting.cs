using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Schedules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Schedules
{
    public class ScheduleEntityDeleting : IEntityDeleting<Schedule>
    {
        public async Task DeletingAsync(Schedule entity, IDictionary<string, object> additionalParameters = null)
        {
            foreach (var scheduleCourse in entity.ScheduleCourses.ToList())
            {
                await Persistence.ForEntity<ScheduleCourse>().DeleteAsync(scheduleCourse);
            }
        }
    }
}