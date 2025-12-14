using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Enums.Common;
using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Common.Core.Enums.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShumenTraffic.Common.Core.Filters.Schedules
{
    public class ScheduleFilter : FilterSorterBase<Schedule>
    {
        public int? BusLineId { get; set; }
        public RouteDirection? Direction { get; set; }
        public DaysOfWeek? DaysOfWeek { get; set; }
        public DateOnly? StartDateLE { get; set; }
        public DateOnly? EndDateGEOrNull { get; set; }
        public SchedulePriority? Priority { get; set; }
        public List<int> ExcludeIds { get; set; } = new List<int>();

        public override IQueryable<Schedule> Filter<TDbContext>(IQueryable<Schedule> query, IEntityRepository<Schedule, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<Schedule>(x => true);
            var mainCriteria = PredicateBuilder.New<Schedule>(x => true);

            if (BusLineId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.BusLineId == BusLineId);
            }

            if (Direction.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.Direction == Direction);
            }

            if (DaysOfWeek.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.DaysOfWeek == DaysOfWeek);
            }

            if (StartDateLE.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.StartDate <= StartDateLE);
            }

            if (EndDateGEOrNull.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.EndDate >= EndDateGEOrNull || x.EndDate == null);
            }

            if (Priority.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.Priority == Priority);
            }

            if (ExcludeIds != null && ExcludeIds.Count > 0)
            {
                mainCriteria = mainCriteria.And(x => !ExcludeIds.Contains(x.Id));
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
                else if (sort.Field.Equals("Direction", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.Direction, sort.Dir));
                }
                else if (sort.Field.Equals("DaysOfWeek", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.DaysOfWeek, sort.Dir));
                }
                else if (sort.Field.Equals("StartDate", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.StartDate, sort.Dir));
                }
                else if (sort.Field.Equals("Priority", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.Priority, sort.Dir));
                }
            }

            return result;
        }
    }
}