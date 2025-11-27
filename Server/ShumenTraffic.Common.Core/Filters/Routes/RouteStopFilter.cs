using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Routes;
using System.Linq;

namespace ShumenTraffic.Common.Core.Filters.Routes
{
    public class RouteStopFilter : FilterSorterBase<RouteStop>
    {
        public int? BusStopId { get; set; }

        public override IQueryable<RouteStop> Filter<TDbContext>(IQueryable<RouteStop> query, IEntityRepository<RouteStop, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<RouteStop>(x => true);
            var mainCriteria = PredicateBuilder.New<RouteStop>(x => true);

            if (BusStopId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.BusStopId == BusStopId.Value);
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }
    }
}