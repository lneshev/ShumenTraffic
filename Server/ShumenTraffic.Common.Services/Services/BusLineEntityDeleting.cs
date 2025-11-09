using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services
{
    public class BusLineEntityDeleting : IEntityDeleting<BusLine>
    {
        public async Task DeletingAsync(BusLine entity, IDictionary<string, object> additionalParameters = null)
        {
            foreach (var transportCompanyBusLine in entity.TransportationCompanyBusLines.ToList())
            {
                await MoravianStar.Dao.Persistence.ForEntity<TransportationCompanyBusLine>().DeleteAsync(transportCompanyBusLine);
            }
        }
    }
}