using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Web.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services
{
    public class BusLineModelsMappingService : ModelsMappingService<BusLineModel, BusLine>
    {
        public override Expression<Func<BusLine, IProjectionBase>> Project()
        {
            return x => new BusLineModel()
            {
                Id = x.Id,
                LineNumber = x.LineNumber,
                Description = x.Description,
                IsActive = x.IsActive,
                TransportationCompanyIds = x.TransportationCompanyBusLines.OrderBy(x => x.TransportationCompany.Name).Select(x => x.TransportationCompanyId).ToList(),
                TransportationCompanyNames = x.TransportationCompanyBusLines.OrderBy(x => x.TransportationCompany.Name).Select(x => x.TransportationCompany.Name).ToList()
            };
        }

        public override IQueryable<BusLine> GetIncludes(IQueryable<BusLine> query)
        {
            return query
                .Include(x => x.TransportationCompanyBusLines)
                .ThenInclude(x => x.TransportationCompany);
        }

        public override async Task<List<EntityModelPair<BusLine, BusLineModel>>> ToEntities(List<EntityModelPair<BusLine, BusLineModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Id = pair.Model.Id;
                pair.Entity.LineNumber = pair.Model.LineNumber;
                pair.Entity.Description = pair.Model.Description;
                pair.Entity.IsActive = pair.Model.IsActive;
                await FillEntityTransportCompanies(pair);
            }

            return pairs;
        }

        private async Task FillEntityTransportCompanies(EntityModelPair<BusLine, BusLineModel> pair)
        {
            var distinctModelTransportCompanyIds = pair.Model.TransportationCompanyIds.Distinct();
            var existingTransportCompanyIds = pair.Entity.TransportationCompanyBusLines.Select(x => x.TransportationCompanyId).ToList();

            var transportCompanyIdsToDelete = existingTransportCompanyIds.Except(distinctModelTransportCompanyIds);
            var transportCompanyIdsToInsert = distinctModelTransportCompanyIds.Except(existingTransportCompanyIds);

            if (transportCompanyIdsToDelete.Any() || transportCompanyIdsToInsert.Any())
            {
                foreach (var transportCompanyId in transportCompanyIdsToDelete)
                {
                    var transportCompanyBusLine = pair.Entity.TransportationCompanyBusLines.Single(x => x.TransportationCompanyId == transportCompanyId);
                    pair.Entity.TransportationCompanyBusLines.Remove(transportCompanyBusLine);
                }

                foreach (var transportCompanyId in transportCompanyIdsToInsert)
                {
                    pair.Entity.TransportationCompanyBusLines.Add(new TransportationCompanyBusLine()
                    {
                        TransportationCompanyId = transportCompanyId,
                        TransportationCompany = await Persistence.ForEntity<TransportationCompany, int>().GetAsync(transportCompanyId)
                    });
                }
            }
        }
    }
}