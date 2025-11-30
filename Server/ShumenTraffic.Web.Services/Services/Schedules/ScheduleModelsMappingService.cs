using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Web.Core.Models.Schedules;
using System;
using System.Linq.Expressions;

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
                BusLineId = x.BusLineId
            };
        }
    }
}