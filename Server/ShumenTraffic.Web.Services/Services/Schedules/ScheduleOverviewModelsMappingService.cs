using MoravianStar.Dao;
using MoravianStar.Extensions;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Resources;
using ShumenTraffic.Web.Core.Models.Schedules;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services.Schedules
{
    public class ScheduleOverviewModelsMappingService : ModelsMappingService<ScheduleOverviewModel, Schedule>
    {
        public override Expression<Func<Schedule, IProjectionBase>> Project()
        {
            return x => new ScheduleOverviewModel()
            {
                Id = x.Id,
                DayType = x.DayType,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive,
                BusLineId = x.BusLineId,
                BusLineNumber = x.BusLine.LineNumber
            };
        }

        public override async Task<ScheduleOverviewModel> MapAsync(IProjectionBase projection)
        {
            var model = (ScheduleOverviewModel)projection;
            model.DayTypeText = model.DayType.Translate(typeof(Strings));
            return await Task.FromResult(model);
        }

        public override async Task<List<EntityModelPair<Schedule, ScheduleOverviewModel>>> ToEntities(List<EntityModelPair<Schedule, ScheduleOverviewModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Id = pair.Model.Id;
                pair.Entity.DayType = pair.Model.DayType;
                pair.Entity.StartDate = pair.Model.StartDate;
                pair.Entity.EndDate = pair.Model.EndDate;
                pair.Entity.IsActive = pair.Model.IsActive;
                pair.Entity.BusLineId = pair.Model.BusLineId;
                pair.Entity.BusLine = await Persistence.ForEntity<BusLine, int>().GetAsync(pair.Entity.BusLineId);
            }

            return pairs;
        }
    }
}