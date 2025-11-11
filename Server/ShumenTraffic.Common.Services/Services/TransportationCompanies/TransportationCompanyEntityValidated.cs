using MoravianStar.Dao;
using MoravianStar.Exceptions;
using ShumenTraffic.Common.Core.Entities.TransportationCompanies;
using ShumenTraffic.Common.Core.Filters.TransportationCompanies;
using ShumenTraffic.Common.Core.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.TransportationCompanies
{
    public class TransportationCompanyEntityValidated : IEntityValidated<TransportationCompany>
    {
        public async Task ValidatedAsync(TransportationCompany entity, TransportationCompany originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            var tcFilter = new TransportationCompanyFilter() { NameEquals = entity.Name, ExcludeIds = new List<int>() { entity.Id } };
            var tcExist = await Persistence.ForEntity<TransportationCompany>().ExistAsync(tcFilter);
            if (tcExist)
            {
                throw new EntityNotUniqueException(string.Format(Strings.TransportationCompanyWithNameAlreadyExists, entity.Name));
            }
        }
    }
}