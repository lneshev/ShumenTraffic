using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ShumenTraffic.Common.Core.Filters
{
    public class TransportationCompanyFilter : FilterSorterBase<TransportationCompany>
    {
        public string NameEquals { get; set; }
        public List<int> ExcludeIds { get; set; } = new List<int>();

        public override IQueryable<TransportationCompany> Filter<TDbContext>(IQueryable<TransportationCompany> query, IEntityRepository<TransportationCompany, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<TransportationCompany>(x => true);
            var mainCriteria = PredicateBuilder.New<TransportationCompany>(x => true);

            if (!string.IsNullOrEmpty(NameEquals))
            {
                mainCriteria = mainCriteria.And(x => x.Name == NameEquals);
            }

            if (ExcludeIds != null && ExcludeIds.Count > 0)
            {
                mainCriteria = mainCriteria.And(x => !ExcludeIds.Contains(x.Id));
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }

        public override List<(Expression<Func<TransportationCompany, object>> expression, SortDirection direction)> Sort<TDbContext>(IEnumerable<Sort> sorts, IEntityRepository<TransportationCompany, TDbContext> entityRepository)
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