using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services
{
    public class TransportationCompanyModelsMappingService : ModelsMappingService<TransportationCompanyModel, TransportationCompany>
    {
        public override Expression<Func<TransportationCompany, IProjectionBase>> Project()
        {
            return x => new TransportationCompanyModel()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            };
        }

        public override async Task<List<EntityModelPair<TransportationCompany, TransportationCompanyModel>>> ToEntities(List<EntityModelPair<TransportationCompany, TransportationCompanyModel>> pairs)
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