using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Web.Core.Models.BusStops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services.BusStops
{
    public class BusStopModelsMappingService : ModelsMappingService<BusStopModel, BusStop>
    {
        public override Expression<Func<BusStop, IProjectionBase>> Project()
        {
            return x => new BusStopModel
            {
                Id = x.Id,
                Name = x.Name,
                ZoneId = x.ZoneId,
                ZoneName = x.Zone.Name,
                Location = x.Location,
                IsActive = x.IsActive
            };
        }

        public override IQueryable<BusStop> GetIncludes(IQueryable<BusStop> query)
        {
            return base.GetIncludes(query)
                .Include(x => x.Zone);
        }

        public override async Task<List<EntityModelPair<BusStop, BusStopModel>>> ToEntities(List<EntityModelPair<BusStop, BusStopModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Id = pair.Model.Id;
                pair.Entity.Name = pair.Model.Name;
                pair.Entity.ZoneId = pair.Model.ZoneId;
                pair.Entity.Zone = await Persistence.ForEntity<Zone, int>().GetAsync(pair.Entity.ZoneId);
                pair.Entity.Location = pair.Model.Location;
                pair.Entity.IsActive = pair.Model.IsActive;
            }

            return pairs;
        }
    }
}