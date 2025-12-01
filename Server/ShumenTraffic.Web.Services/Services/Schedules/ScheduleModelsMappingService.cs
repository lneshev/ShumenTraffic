using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Web.Core.Models.Schedules;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services.Schedules
{
    public class ScheduleModelsMappingService : ModelsMappingService<ScheduleModel, Schedule>
    {
        public override Expression<Func<Schedule, IProjectionBase>> Project()
        {
            return x => new ScheduleModel()
            {
                Id = x.Id,
                DayType = x.DayType,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive,
                Priority = x.Priority,
                BusLineId = x.BusLineId,
                BusLineNumber = x.BusLine.LineNumber
            };
        }

        public override async Task<List<EntityModelPair<Schedule, ScheduleModel>>> ToEntities(List<EntityModelPair<Schedule, ScheduleModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Id = pair.Model.Id;
                pair.Entity.DayType = pair.Model.DayType;
                pair.Entity.StartDate = pair.Model.StartDate;
                pair.Entity.EndDate = pair.Model.EndDate;
                pair.Entity.IsActive = pair.Model.IsActive;
                pair.Entity.Priority = pair.Model.Priority;
                pair.Entity.BusLineId = pair.Model.BusLineId;
                pair.Entity.BusLine = await Persistence.ForEntity<BusLine, int>().GetAsync(pair.Entity.BusLineId);
            }

            return pairs;
        }
    }
}