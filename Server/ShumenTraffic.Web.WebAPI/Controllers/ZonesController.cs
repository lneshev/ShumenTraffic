using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Web.Core.Models;
using ShumenTraffic.Web.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Zones.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ZonesController : BaseController
    {
        private readonly IZoneModelService _zoneService;

        public ZonesController(IZoneModelService zoneService)
        {
            _zoneService = zoneService;
        }

        /// <summary>
        /// Get all zones.
        /// </summary>
        /// <param name="includeInactive">Include inactive zones</param>
        /// <returns>List of zones</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var zones = await _zoneService.GetAllAsync(includeInactive);
            var zonesList = zones.ToList();
            return Ok(zonesList, $"Retrieved {zonesList.Count} zones");
        }

        /// <summary>
        /// Get a specific zone by ID.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <returns>Zone details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _zoneService.GetByIdAsync(id);

            if (zone == null)
            {
                return NotFound("Zone not found", $"No zone found with ID {id}");
            }

            return Ok(zone, "Zone retrieved successfully");
        }

        /// <summary>
        /// Create a new zone.
        /// </summary>
        /// <param name="dto">Create zone DTO</param>
        /// <returns>Created zone</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateZoneDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var result = await _zoneService.CreateAsync(dto);

            return Created(nameof(GetById), nameof(ZonesController), new { id = result.Id }, result, "Zone created successfully");
        }

        /// <summary>
        /// Update an existing zone.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <param name="dto">Update zone DTO</param>
        /// <returns>Updated zone</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateZoneDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var result = await _zoneService.UpdateAsync(id, dto);

            if (result == null)
            {
                return NotFound("Zone not found", $"No zone found with ID {id}");
            }

            return Ok(result, "Zone updated successfully");
        }

        /// <summary>
        /// Delete a zone.
        /// </summary>
        /// <param name="id">Zone ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _zoneService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Zone not found", $"No zone found with ID {id}");
            }

            return Ok<object>(null, "Zone deleted successfully");
        }
    }
}