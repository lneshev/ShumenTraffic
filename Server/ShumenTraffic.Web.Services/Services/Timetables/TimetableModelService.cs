using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Common.Services.Interfaces.Timetables;
using ShumenTraffic.Web.Core.Models.BusStops;
using ShumenTraffic.Web.Core.Models.Schedules;
using ShumenTraffic.Web.Core.Models.Timetables;
using ShumenTraffic.Web.Services.Interfaces.Timetables;
using System;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services.Timetables
{
    public class TimetableModelService : ITimetableModelService
    {
        private readonly ITimetableService timetableService;
        private readonly IModelsMappingService<ScheduleModel, Schedule> scheduleModelsMappingService;
        private readonly IModelsMappingService<BusStopModel, BusStop> busStopModelsMappingService;

        public TimetableModelService(
            ITimetableService timetableService,
            IModelsMappingService<ScheduleModel, Schedule> scheduleModelsMappingService,
            IModelsMappingService<BusStopModel, BusStop> busStopModelsMappingService)
        {
            this.timetableService = timetableService;
            this.scheduleModelsMappingService = scheduleModelsMappingService;
            this.busStopModelsMappingService = busStopModelsMappingService;
        }

        public async Task<TimetableModel> Get(int busLineId, RouteDirection direction, DateOnly date)
        {
            TimetableModel result = null;

            var timetable = await timetableService.Get(busLineId, direction, date);
            if (timetable == null)
            {
                return result;
            }

            result = new TimetableModel()
            {
                Schedule = await scheduleModelsMappingService.MapAsync(timetable.Schedule)
            };
            foreach (var row in timetable.TimetableRows)
            {
                result.TimetableRows.Add(new TimetableRowModel()
                {
                    BusStop = new BusStopModel()
                    {
                        Id = row.BusStop.Id,
                        Name = row.BusStop.Name,
                        IsActive = row.BusStop.IsActive,
                        Location = row.BusStop.Location,
                        ZoneId = row.BusStop.ZoneId
                    },
                    TimesByVariant = row.TimesByVariant
                });
            }

            return result;
        }
    }
}