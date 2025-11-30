using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShumenTraffic.Common.Core.Filters.Schedules
{
    public class ScheduleFilter : FilterSorterBase<Schedule>
    {
        public int? BusLineId { get; set; }

        public override IQueryable<Schedule> Filter<TDbContext>(IQueryable<Schedule> query, IEntityRepository<Schedule, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<Schedule>(x => true);
            var mainCriteria = PredicateBuilder.New<Schedule>(x => true);

            if (BusLineId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.BusLineId == BusLineId);
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }

        public override List<(Expression<Func<Schedule, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<Schedule, TDbContext> entityRepository)
        {
            var result = base.Sort(sorts, entityRepository);

            foreach (var sort in sorts)
            {
                if (sort.Field.Equals("BusLineNumber", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.BusLine.LineNumber, sort.Dir));
                }
                else if (sort.Field.Equals("DayType", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.DayType, sort.Dir));
                }
                else if (sort.Field.Equals("StartDate", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.StartDate, sort.Dir));
                }
            }

            return result;
        }
    }
}