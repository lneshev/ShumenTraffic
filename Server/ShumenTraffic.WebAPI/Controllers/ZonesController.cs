using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Data.Context;
using ShumenTraffic.Data.Models;
using ShumenTraffic.WebAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Zones.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ZonesController : BaseController
    {
        private readonly ShumenTrafficDbContext _context;

        public ZonesController(ShumenTrafficDbContext context)
        {
            _context = context;
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
            var query = _context.Zones.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(z => z.IsActive);
            }

            var zones = await query
                .OrderBy(z => z.Name)
                .Select(z => new ZoneDto
                {
                    Id = z.Id,
                    Name = z.Name,
                    Description = z.Description,
                    IsActive = z.IsActive
                })
                .ToListAsync();

            return Ok(zones, $"Retrieved {zones.Count} zones");
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
            var zone = await _context.Zones
                .Where(z => z.Id == id)
                .Select(z => new ZoneDto
                {
                    Id = z.Id,
                    Name = z.Name,
                    Description = z.Description,
                    IsActive = z.IsActive
                })
                .FirstOrDefaultAsync();

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

            var zone = new Zone
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };

            _context.Zones.Add(zone);
            await _context.SaveChangesAsync();

            var result = new ZoneDto
            {
                Id = zone.Id,
                Name = zone.Name,
                Description = zone.Description,
                IsActive = zone.IsActive
            };

            return Created(nameof(GetById), nameof(ZonesController), new { id = zone.Id }, result, "Zone created successfully");
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

            var zone = await _context.Zones.FindAsync(id);

            if (zone == null)
            {
                return NotFound("Zone not found", $"No zone found with ID {id}");
            }

            if (!string.IsNullOrEmpty(dto.Name))
                zone.Name = dto.Name;
            if (dto.Description != null)
                zone.Description = dto.Description;
            if (dto.IsActive.HasValue)
                zone.IsActive = dto.IsActive.Value;

            _context.Zones.Update(zone);
            await _context.SaveChangesAsync();

            var result = new ZoneDto
            {
                Id = zone.Id,
                Name = zone.Name,
                Description = zone.Description,
                IsActive = zone.IsActive
            };

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
            var zone = await _context.Zones.FindAsync(id);

            if (zone == null)
            {
                return NotFound("Zone not found", $"No zone found with ID {id}");
            }

            _context.Zones.Remove(zone);
            await _context.SaveChangesAsync();

            return Ok<object>(null, "Zone deleted successfully");
        }
    }
}

