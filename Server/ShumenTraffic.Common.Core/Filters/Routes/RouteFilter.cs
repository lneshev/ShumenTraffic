using LinqKit;
using MoravianStar.Dao;
using MoravianStar.Extensions;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Common.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShumenTraffic.Common.Core.Filters.Routes
{
    public class RouteFilter : FilterSorterBase<Route>
    {
        public string NameEqualsInsensitive { get; set; }
        public int? BusLineId { get; set; }
        public RouteDirection? Direction { get; set; }
        public List<int> ExcludeIds { get; set; } = new List<int>();

        public override IQueryable<Route> Filter<TDbContext>(IQueryable<Route> query, IEntityRepository<Route, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<Route>(x => true);
            var mainCriteria = PredicateBuilder.New<Route>(x => true);

            if (!string.IsNullOrEmpty(NameEqualsInsensitive))
            {
                mainCriteria = mainCriteria.And(x => x.Name.ToUpper() == NameEqualsInsensitive.ToUpper());
            }

            if (BusLineId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.BusLineId == BusLineId.Value);
            }

            if (Direction.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.Direction == Direction.Value);
            }

            if (ExcludeIds != null && ExcludeIds.Count > 0)
            {
                mainCriteria = mainCriteria.And(x => !ExcludeIds.Contains(x.Id));
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }

        public override List<(Expression<Func<Route, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<Route, TDbContext> entityRepository)
        {
            var result = base.Sort(sorts, entityRepository);

            foreach (var sort in sorts)
            {
                if (sort.Field.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.Name, sort.Dir));
                }
                else if (sort.Field.Equals("BusLineNumber", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x => x.BusLine.LineNumber, sort.Dir));
                }
                else if (sort.Field.Equals("DirectionText", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((x =>
                        x.Direction == RouteDirection.One ? RouteDirection.One.Translate(typeof(Strings)) :
                        x.Direction == RouteDirection.Two ? RouteDirection.Two.Translate(typeof(Strings)) :
                        string.Empty, sort.Dir));
                }
            }

            return result;
        }
    }
}