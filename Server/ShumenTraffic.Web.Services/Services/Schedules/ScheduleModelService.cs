using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Enums.Schedules;
using ShumenTraffic.Common.Services.Interfaces;
using ShumenTraffic.Web.Core.Models.Schedules;
using ShumenTraffic.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services.Schedules
{
    /// <summary>
    /// Service for Schedule operations.
    /// </summary>
    public class ScheduleModelService : IScheduleModelService
    {
        private readonly IScheduleService _scheduleService;
        private readonly IRouteService _routeService;

        public ScheduleModelService(IScheduleService scheduleService, IRouteService routeService)
        {
            _scheduleService = scheduleService;
            _routeService = routeService;
        }

        private ScheduleModel MapToModel(Schedule entity)
        {
            return new ScheduleModel
            {
                Id = entity.Id,
                DayType = entity.DayType,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                Priority = entity.Priority,
                BusLineId = entity.BusLineId,
                Courses = entity.ScheduleCourses
                    .OrderBy(sc => sc.DepartureTime)
                    .Select(sc => new ScheduleCourseDto
                    {
                        Id = sc.Id,
                        RouteId = sc.RouteId,
                        BusLineNumber = sc.Route.BusLine.LineNumber,
                        Direction = sc.Route.Direction,
                        DepartureTime = sc.DepartureTime.ToTimeSpan()
                    })
                    .ToList()
            };
        }

        public async Task<IEnumerable<ScheduleModel>> GetAllAsync(DayType? dayType = null, bool includeInactive = false)
        {
            var entities = await _scheduleService.GetAllWithCoursesAsync(dayType, includeInactive);
            return entities.Select(MapToModel);
        }

        public async Task<ScheduleModel> GetByIdAsync(int id)
        {
            var entity = await _scheduleService.GetByIdWithCoursesAsync(id);
            return entity != null ? MapToModel(entity) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _scheduleService.DeleteAsync(id);
        }

        public async Task<(ScheduleModel dto, string error)> CreateAsync(CreateScheduleDto dto)
        {
            // Verify routes exist
            var routeIds = dto.Courses.Select(c => c.RouteId).Distinct().ToList();
            foreach (var routeId in routeIds)
            {
                if (!await _routeService.ExistsAsync(routeId))
                {
                    return (null, "One or more routes do not exist");
                }
            }

            var courses = dto.Courses.Select(c => new ScheduleCourseData
            {
                RouteId = c.RouteId,
                DepartureTime = c.DepartureTime
            });

            var entity = await _scheduleService.CreateAsync(
                dto.DayType,
                dto.StartDate,
                dto.EndDate,
                dto.Priority,
                courses
            );

            return (MapToModel(entity), null);
        }

        public async Task<ScheduleModel> UpdateAsync(int id, UpdateScheduleDto dto)
        {
            var entity = await _scheduleService.UpdateAsync(
                id,
                dto.EndDate,
                dto.IsActive,
                dto.Priority
            );

            return entity != null ? MapToModel(entity) : null;
        }
    }
}