using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Data.Context;
using ShumenTraffic.Data.Models;
using ShumenTraffic.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Schedules and Schedule Courses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SchedulesController : BaseController
    {
        private readonly ShumenTrafficDbContext _context;

        public SchedulesController(ShumenTrafficDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all schedules.
        /// </summary>
        /// <param name="dayType">Filter by day type (optional)</param>
        /// <param name="includeInactive">Include inactive schedules</param>
        /// <returns>List of schedules</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string dayType = null, [FromQuery] bool includeInactive = false)
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

            return Ok(schedules, $"Retrieved {schedules.Count} schedules");
        }

        /// <summary>
        /// Get a specific schedule by ID.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>Schedule details with all courses</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
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

            if (schedule == null)
            {
                return NotFound("Schedule not found", $"No schedule found with ID {id}");
            }

            return Ok(schedule, "Schedule retrieved successfully");
        }

        /// <summary>
        /// Create a new schedule with courses.
        /// </summary>
        /// <param name="dto">Create schedule DTO</param>
        /// <returns>Created schedule</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            // Verify routes exist
            var routeIds = dto.Courses.Select(c => c.RouteId).Distinct().ToList();
            var existingRoutes = await _context.Routes.Where(r => routeIds.Contains(r.Id)).CountAsync();
            if (existingRoutes != routeIds.Count)
            {
                return BadRequest("Invalid routes", "One or more routes do not exist");
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

            return Created(nameof(GetById), nameof(SchedulesController), new { id = schedule.Id }, result, "Schedule created successfully");
        }

        /// <summary>
        /// Update an existing schedule.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <param name="dto">Update schedule DTO</param>
        /// <returns>Updated schedule</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateScheduleDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var schedule = await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .ThenInclude(sc => sc.Route)
                .ThenInclude(r => r.BusLine)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return NotFound("Schedule not found", $"No schedule found with ID {id}");
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

            return Ok(result, "Schedule updated successfully");
        }

        /// <summary>
        /// Delete a schedule.
        /// </summary>
        /// <param name="id">Schedule ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.ScheduleCourses)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (schedule == null)
            {
                return NotFound("Schedule not found", $"No schedule found with ID {id}");
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return Ok<object>(null, "Schedule deleted successfully");
        }
    }
}

