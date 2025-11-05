using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Web.WebAPI.DTOs;
using ShumenTraffic.Web.WebAPI.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Schedules and Schedule Courses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SchedulesController : BaseController
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
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
            var schedules = await _scheduleService.GetAllAsync(dayType, includeInactive);
            var schedulesList = schedules.ToList();
            return Ok(schedulesList, $"Retrieved {schedulesList.Count} schedules");
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
            var schedule = await _scheduleService.GetByIdAsync(id);

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

            var (result, error) = await _scheduleService.CreateAsync(dto);

            if (error != null)
            {
                return BadRequest("Schedule creation failed", error);
            }

            return Created(nameof(GetById), nameof(SchedulesController), new { id = result.Id }, result, "Schedule created successfully");
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

            var result = await _scheduleService.UpdateAsync(id, dto);

            if (result == null)
            {
                return NotFound("Schedule not found", $"No schedule found with ID {id}");
            }

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
            var deleted = await _scheduleService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Schedule not found", $"No schedule found with ID {id}");
            }

            return Ok<object>(null, "Schedule deleted successfully");
        }
    }
}

