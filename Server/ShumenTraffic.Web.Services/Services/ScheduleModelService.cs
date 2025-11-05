using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Persistence.DbContexts;
using ShumenTraffic.Web.Core.Models;
using ShumenTraffic.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Services
{
    /// <summary>
    /// Service for Schedule operations.
    /// </summary>
    public class ScheduleModelService : BaseModelService<Schedule, ScheduleDto>, IScheduleModelService
    {
        public ScheduleModelService(AppDbContext context) : base(context)
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

        protected override ScheduleDto MapToDto(Schedule entity)
        {
            return new ScheduleDto
            {
                Id = entity.Id,
                DayType = entity.DayType,
                EffectiveDate = entity.EffectiveDate,
                ExpiryDate = entity.ExpiryDate,
                IsActive = entity.IsActive,
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

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync(string dayType = null, bool includeInactive = false)
        {
            var query = _context.Schedules
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dayType))
            {
                query = query.Where(s => s.DayType == dayType);
            }

            if (!includeInactive)
            {
                query = query.Where(s => s.IsActive);
            }

            var schedules = await query
                .OrderBy(s => s.DayType)
                .ThenBy(s => s.EffectiveDate)
                .Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    DayType = s.DayType,
                    EffectiveDate = s.EffectiveDate,
                    ExpiryDate = s.ExpiryDate,
                    IsActive = s.IsActive,
                    Courses = s.ScheduleCourses
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
                })
                .ToListAsync();

            return schedules;
        }

        public override async Task<ScheduleDto> GetByIdAsync(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine)
                .Where(s => s.Id == id)
                .Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    DayType = s.DayType,
                    EffectiveDate = s.EffectiveDate,
                    ExpiryDate = s.ExpiryDate,
                    IsActive = s.IsActive,
                    Courses = s.ScheduleCourses
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
                })
                .FirstOrDefaultAsync();

            return schedule;
        }

        public async Task<(ScheduleDto dto, string error)> CreateAsync(CreateScheduleDto dto)
        {
            // Verify routes exist
            var routeIds = dto.Courses.Select(c => c.RouteId).Distinct().ToList();
            var existingRoutes = await _context.Routes.Where(r => routeIds.Contains(r.Id)).CountAsync();
            if (existingRoutes != routeIds.Count)
            {
                return (null, "One or more routes do not exist");
            }

            var schedule = new Schedule
            {
                DayType = dto.DayType,
                EffectiveDate = dto.EffectiveDate,
                ExpiryDate = dto.ExpiryDate,
                IsActive = true
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            // Add courses
            foreach (var courseDto in dto.Courses)
            {
                var course = new ScheduleCourse
                {
                    ScheduleId = schedule.Id,
                    RouteId = courseDto.RouteId,
                    DepartureTime = TimeOnly.FromTimeSpan(courseDto.DepartureTime)
                };
                _context.ScheduleCourses.Add(course);
            }

            await _context.SaveChangesAsync();

            // Reload schedule with courses
            var createdSchedule = await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine)
                .FirstAsync(s => s.Id == schedule.Id);

            var result = new ScheduleDto
            {
                Id = createdSchedule.Id,
                DayType = createdSchedule.DayType,
                EffectiveDate = createdSchedule.EffectiveDate,
                ExpiryDate = createdSchedule.ExpiryDate,
                IsActive = createdSchedule.IsActive,
                Courses = createdSchedule.ScheduleCourses
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

            return (result, null);
        }

        public async Task<ScheduleDto> UpdateAsync(int id, UpdateScheduleDto dto)
        {
            var schedule = await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return null;
            }

            if (dto.ExpiryDate.HasValue)
                schedule.ExpiryDate = dto.ExpiryDate.Value;
            if (dto.IsActive.HasValue)
                schedule.IsActive = dto.IsActive.Value;

            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();

            var result = new ScheduleDto
            {
                Id = schedule.Id,
                DayType = schedule.DayType,
                EffectiveDate = schedule.EffectiveDate,
                ExpiryDate = schedule.ExpiryDate,
                IsActive = schedule.IsActive,
                Courses = schedule.ScheduleCourses
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

            return result;
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