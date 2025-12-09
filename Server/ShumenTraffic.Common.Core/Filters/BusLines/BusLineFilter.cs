using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusLines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShumenTraffic.Common.Core.Filters.BusLines
{
    public class BusLineFilter : FilterSorterBase<BusLine>
    {
        public string LineNumberEquals { get; set; }
        public List<int> ExcludeIds { get; set; } = new List<int>();

        public override IQueryable<BusLine> Filter<TDbContext>(IQueryable<BusLine> query, IEntityRepository<BusLine, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<BusLine>(x => true);
            var mainCriteria = PredicateBuilder.New<BusLine>(x => true);

            if (!string.IsNullOrEmpty(LineNumberEquals))
            {
                mainCriteria = mainCriteria.And(x => x.LineNumber == LineNumberEquals);
            }

            if (ExcludeIds != null && ExcludeIds.Count > 0)
            {
                mainCriteria = mainCriteria.And(x => !ExcludeIds.Contains(x.Id));
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }

        public override List<(Expression<Func<BusLine, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<BusLine, TDbContext> entityRepository)
        {
            var result = base.Sort(sorts, entityRepository);

            foreach (var sort in sorts)
            {
                if (sort.Field.Equals("LineNumber", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.LineNumberSortKey, sort.Dir));
                }
            }

            return result;
        }
    }
}