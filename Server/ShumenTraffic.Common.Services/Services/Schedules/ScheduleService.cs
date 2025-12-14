using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Enums.Common;
using ShumenTraffic.Common.Core.Enums.Schedules;
using ShumenTraffic.Common.DataAccess.DbContexts;
using ShumenTraffic.Common.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Services.Schedules
{
    /// <summary>
    /// Service for Schedule entity operations.
    /// </summary>
    public class ScheduleService : BaseEntityService<Schedule>, IScheduleService
    {
        public ScheduleService(AppDbContext context) : base(context)
        {
        }

        protected override DbSet<Schedule> GetDbSet() => _context.Schedules;

        protected override IQueryable<Schedule> BuildQuery(IQueryable<Schedule> query)
        {
            return query
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine);
        }

        protected override IQueryable<Schedule> ApplyActiveFilter(IQueryable<Schedule> query, bool includeInactive)
        {
            if (!includeInactive)
            {
                query = query.Where(s => s.IsActive);
            }
            return query;
        }

        protected override async Task<Schedule> FindByIdAsync(IQueryable<Schedule> query, int id)
        {
            return await query.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Schedule>> GetAllWithCoursesAsync(DaysOfWeek? daysOfWeek = null, bool includeInactive = false)
        {
            var query = _context.Schedules
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine)
                .AsQueryable();

            if (daysOfWeek.HasValue)
            {
                query = query.Where(s => s.DaysOfWeek == daysOfWeek);
            }

            if (!includeInactive)
            {
                query = query.Where(s => s.IsActive);
            }

            return await query
                .OrderBy(s => s.DaysOfWeek)
                .ThenBy(s => s.StartDate)
                .ToListAsync();
        }

        public async Task<Schedule> GetByIdWithCoursesAsync(int id)
        {
            return await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Schedule> CreateAsync(DaysOfWeek daysOfWeek, DateOnly startDate, DateOnly? endDate, SchedulePriority priority, IEnumerable<ScheduleCourseData> courses)
        {
            var schedule = new Schedule
            {
                DaysOfWeek = daysOfWeek,
                StartDate = startDate,
                EndDate = endDate,
                Priority = priority,
                IsActive = true
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Add courses
            foreach (var courseData in courses)
            {
                var course = new ScheduleCourse
                {
                    ScheduleId = schedule.Id,
                    RouteId = courseData.RouteId,
                    DepartureTime = TimeOnly.FromTimeSpan(courseData.DepartureTime)
                };
                _context.ScheduleCourses.Add(course);
            }

            await _context.SaveChangesAsync();

            return await GetByIdWithCoursesAsync(schedule.Id);
        }

        public async Task<Schedule> UpdateAsync(int id, DateOnly? endDate = null, bool? isActive = null, SchedulePriority? priority = null)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
            {
                return null;
            }

            if (endDate.HasValue)
            {
                schedule.EndDate = endDate.Value;
            }
            if (isActive.HasValue)
            {
                schedule.IsActive = isActive.Value;
            }
            if (priority.HasValue)
            {
                schedule.Priority = priority.Value;
            }

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();

            return await GetByIdWithCoursesAsync(id);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return false;
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}