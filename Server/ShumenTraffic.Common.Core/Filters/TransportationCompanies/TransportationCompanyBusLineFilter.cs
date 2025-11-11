using LinqKit;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.TransportationCompanies;
using System.Linq;

namespace ShumenTraffic.Common.Core.Filters.TransportationCompanies
{
    public class TransportationCompanyBusLineFilter : FilterSorterBase<TransportationCompanyBusLine>
    {
        public int? TransportationCompanyId { get; set; }

        public override IQueryable<TransportationCompanyBusLine> Filter<TDbContext>(IQueryable<TransportationCompanyBusLine> query, IEntityRepository<TransportationCompanyBusLine, TDbContext> entityRepository)
        {
            query = base.Filter(query, entityRepository);

            var rootCriteria = PredicateBuilder.New<TransportationCompanyBusLine>(x => true);
            var mainCriteria = PredicateBuilder.New<TransportationCompanyBusLine>(x => true);

            if (TransportationCompanyId.HasValue)
            {
                mainCriteria = mainCriteria.And(x => x.TransportationCompanyId == TransportationCompanyId.Value);
            }

            rootCriteria = mainCriteria;

            return query.Where(rootCriteria);
        }
    }
}