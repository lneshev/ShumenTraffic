using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Extensions.BusLines;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.BusLines
{
    public class BusLineEntityFilling : IEntityFilling<BusLine>
    {
        public async Task FillingAsync(BusLine entity, BusLine originalEntity, IDictionary<string, object> additionalParameters = null)
        {
            await Task.CompletedTask;

            entity.LineNumberSortKey = entity.GenerateBusLineNumberSortKey();
        }
    }
}