using Microsoft.EntityFrameworkCore;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Web.Core.Models.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
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
                DaysOfWeek = x.DaysOfWeek,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive,
                Priority = x.Priority,
                Direction = x.Direction,
                BusLineId = x.BusLineId,
                //BusLineNumber = x.BusLine.LineNumber,
                ScheduleCourses = x.ScheduleCourses.OrderBy(x => x.DepartureTime).Select(y => new ScheduleCourseModel()
                {
                    Id = y.Id,
                    RouteId = y.RouteId,
                    DepartureTime = y.DepartureTime
                })
            };
        }

        public override IQueryable<Schedule> GetIncludes(IQueryable<Schedule> query)
        {
            return base.GetIncludes(query)
                .Include(x => x.BusLine)
                .Include(x => x.ScheduleCourses)
                    .ThenInclude(x => x.Route);
        }

        public override async Task<List<EntityModelPair<Schedule, ScheduleModel>>> ToEntities(List<EntityModelPair<Schedule, ScheduleModel>> pairs)
        {
            pairs = await base.ToEntities(pairs);

            foreach (var pair in pairs)
            {
                pair.Entity.Id = pair.Model.Id;
                pair.Entity.DaysOfWeek = pair.Model.DaysOfWeek;
                pair.Entity.StartDate = pair.Model.StartDate;
                pair.Entity.EndDate = pair.Model.EndDate;
                pair.Entity.IsActive = pair.Model.IsActive;
                pair.Entity.Priority = pair.Model.Priority;
                pair.Entity.Direction = pair.Model.Direction;
                pair.Entity.BusLineId = pair.Model.BusLineId;
                pair.Entity.BusLine = await Persistence.ForEntity<BusLine, int>().GetAsync(pair.Entity.BusLineId);
                await FillEntityScheduleCourses(pair);
            }

            return pairs;
        }

        private async Task FillEntityScheduleCourses(EntityModelPair<Schedule, ScheduleModel> pair)
        {
            var distinctModelCourseIds = pair.Model.ScheduleCourses.Where(x => x.Id > 0).Select(x => x.Id).Distinct();
            var existingCourseIds = pair.Entity.ScheduleCourses.Select(x => x.Id);

            var courseIdsToDelete = existingCourseIds.Except(distinctModelCourseIds);
            var coursesToInsert = pair.Model.ScheduleCourses.Where(x => x.Id == 0);

            // Delete
            foreach (var courseId in courseIdsToDelete.ToList())
            {
                await Persistence.ForEntity<ScheduleCourse, int>().DeleteAsync(courseId);
            }

            // Update
            foreach (var scheduleCourse in pair.Entity.ScheduleCourses)
            {
                var stopModel = pair.Model.ScheduleCourses.SingleOrDefault(x => x.Id == scheduleCourse.Id);
                if (stopModel != null)
                {
                    scheduleCourse.DepartureTime = stopModel.DepartureTime;
                    if (scheduleCourse.RouteId != stopModel.RouteId)
                    {
                        scheduleCourse.RouteId = stopModel.RouteId;
                        scheduleCourse.Route = await Persistence.ForEntity<Route, int>().GetAsync(scheduleCourse.RouteId);
                    }
                    await Persistence.ForEntity<ScheduleCourse>().SaveAsync(scheduleCourse);
                }
            }

            // Insert
            foreach (var courseModel in coursesToInsert)
            {
                var newCourse = new ScheduleCourse()
                {
                    ScheduleId = pair.Entity.Id,
                    Schedule = pair.Entity,
                    DepartureTime = courseModel.DepartureTime,
                    RouteId = courseModel.RouteId,
                    Route = await Persistence.ForEntity<Route, int>().GetAsync(courseModel.RouteId)
                };
                await Persistence.ForEntity<ScheduleCourse>().SaveAsync(newCourse);
            }
        }
    }
}