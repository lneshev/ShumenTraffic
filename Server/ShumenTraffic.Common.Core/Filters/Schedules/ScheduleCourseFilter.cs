using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Schedules;
using System.Linq;

namespace ShumenTraffic.Common.Core.Filters.Schedules
{
    public class ScheduleCourseFilter : FilterSorterBase<ScheduleCourse>
    {
        public int? RouteId { get; set; }
        public int? ScheduleId { get; set; }

        public override IQueryable<ScheduleCourse> Filter<TDbContext>(IQueryable<ScheduleCourse> query, IEntityRepository<ScheduleCourse, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<ScheduleCourse>(x => true);
            var mainCriteria = PredicateBuilder.New<ScheduleCourse>(x => true);

            if (RouteId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.RouteId == RouteId);
            }

            if (ScheduleId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.ScheduleId == ScheduleId);
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }
    }
}