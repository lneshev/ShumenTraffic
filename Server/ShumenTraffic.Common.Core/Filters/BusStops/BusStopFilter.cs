using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusStops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShumenTraffic.Common.Core.Filters.BusStops
{
    public class BusStopFilter : FilterSorterBase<BusStop>
    {
        public string NameEqualsInsensitive { get; set; }
        public int? ZoneId { get; set; }
        public List<int> ExcludeIds { get; set; } = new List<int>();

        public override IQueryable<BusStop> Filter<TDbContext>(IQueryable<BusStop> query, IEntityRepository<BusStop, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<BusStop>(x => true);
            var mainCriteria = PredicateBuilder.New<BusStop>(x => true);

            if (!string.IsNullOrEmpty(NameEqualsInsensitive))
            {
                mainCriteria = mainCriteria.And(x => x.Name.ToUpper() == NameEqualsInsensitive.ToUpper());
            }

            if (ZoneId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.ZoneId == ZoneId.Value);
            }

            if (ExcludeIds != null && ExcludeIds.Count > 0)
            {
                mainCriteria = mainCriteria.And(x => !ExcludeIds.Contains(x.Id));
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }

        public override List<(Expression<Func<BusStop, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<BusStop, TDbContext> entityRepository)
        {
            var result = base.Sort(sorts, entityRepository);

            foreach (var sort in sorts)
            {
                if (sort.Field.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.Name, sort.Dir));
                }
            }

            return result;
        }
    }
}