using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Web.Core.Models.Zones;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services.Zones
{
    public class ZoneModelMappingService : ModelsMappingService<ZoneModel, Zone>
    {
        public override Expression<Func<Zone, IProjectionBase>> Project()
        {
            return x => new ZoneModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            };
        }

        public override async Task<List<EntityModelPair<Zone, ZoneModel>>> ToEntities(List<EntityModelPair<Zone, ZoneModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Id = pair.Model.Id;
                pair.Entity.Name = pair.Model.Name;
                pair.Entity.Description = pair.Model.Description;
                pair.Entity.IsActive = pair.Model.IsActive;
            }

            return pairs;
        }
    }
}