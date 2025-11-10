using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.Core.Filters;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services
{
    public class TransportationCompanyEntityDeleting : IEntityDeleting<TransportationCompany>
    {
        public async Task DeletingAsync(TransportationCompany entity, IDictionary<string, object> additionalParameters = null)
        {
            await Task.CompletedTask;

            bool hasBusLines = await MoravianStar.Dao.Persistence.ForEntity<TransportationCompanyBusLine>().ExistAsync(new TransportationCompanyBusLineFilter()
            {
                TransportationCompanyId = entity.Id
            });

            if (hasBusLines)
            {
                throw new BusinessException(string.Format(Strings.YouAreNotAllowedToDeleteTransportationCompany, entity.Name));
            }
        }
    }
}