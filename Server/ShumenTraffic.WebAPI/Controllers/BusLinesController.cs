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
    /// Controller for managing Bus Lines.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BusLinesController : BaseController
    {
        private readonly ShumenTrafficDbContext _context;

        public BusLinesController(ShumenTrafficDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all bus lines.
        /// </summary>
        /// <param name="includeInactive">Include inactive bus lines</param>
        /// <returns>List of bus lines</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var query = _context.BusLines.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(l => l.IsActive);
            }

            var busLines = await query
                .OrderBy(l => l.LineNumber)
                .Select(l => new BusLineDto
                {
                    Id = l.Id,
                    LineNumber = l.LineNumber,
                    Description = l.Description,
                    IsActive = l.IsActive
                })
                .ToListAsync();

            return Ok(busLines, $"Retrieved {busLines.Count} bus lines");
        }

        /// <summary>
        /// Get a specific bus line by ID.
        /// </summary>
        /// <param name="id">Bus line ID</param>
        /// <returns>Bus line details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var busLine = await _context.BusLines
                .Where(l => l.Id == id)
                .Select(l => new BusLineDto
                {
                    Id = l.Id,
                    LineNumber = l.LineNumber,
                    Description = l.Description,
                    IsActive = l.IsActive
                })
                .FirstOrDefaultAsync();

            if (busLine == null)
            {
                return NotFound("Bus line not found", $"No bus line found with ID {id}");
            }

            return Ok(busLine, "Bus line retrieved successfully");
        }

        /// <summary>
        /// Create a new bus line.
        /// </summary>
        /// <param name="dto">Create bus line DTO</param>
        /// <returns>Created bus line</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBusLineDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            // Check if line number already exists
            var existingLine = await _context.BusLines.Where(l => l.LineNumber == dto.LineNumber).AnyAsync();
            if (existingLine)
            {
                return Conflict("Bus line already exists", $"A bus line with number '{dto.LineNumber}' already exists");
            }

            var busLine = new BusLine
            {
                LineNumber = dto.LineNumber,
                Description = dto.Description,
                IsActive = true
            };

            _context.BusLines.Add(busLine);
            await _context.SaveChangesAsync();

            var result = new BusLineDto
            {
                Id = busLine.Id,
                LineNumber = busLine.LineNumber,
                Description = busLine.Description,
                IsActive = busLine.IsActive
            };

            return Created(nameof(GetById), nameof(BusLinesController), new { id = busLine.Id }, result, "Bus line created successfully");
        }

        /// <summary>
        /// Update an existing bus line.
        /// </summary>
        /// <param name="id">Bus line ID</param>
        /// <param name="dto">Update bus line DTO</param>
        /// <returns>Updated bus line</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBusLineDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var busLine = await _context.BusLines.FindAsync(id);

            if (busLine == null)
            {
                return NotFound("Bus line not found", $"No bus line found with ID {id}");
            }

            if (!string.IsNullOrEmpty(dto.LineNumber))
            {
                // Check if new line number already exists
                var existingLine = await _context.BusLines.Where(l => l.LineNumber == dto.LineNumber && l.Id != id).AnyAsync();
                if (existingLine)
                {
                    return Conflict("Bus line already exists", $"A bus line with number '{dto.LineNumber}' already exists");
                }
                busLine.LineNumber = dto.LineNumber;
            }
            if (dto.Description != null)
            {
                busLine.Description = dto.Description;
            }
            if (dto.IsActive.HasValue)
            {
                busLine.IsActive = dto.IsActive.Value;
            }

            _context.BusLines.Update(busLine);
            await _context.SaveChangesAsync();

            var result = new BusLineDto
            {
                Id = busLine.Id,
                LineNumber = busLine.LineNumber,
                Description = busLine.Description,
                IsActive = busLine.IsActive
            };

            return Ok(result, "Bus line updated successfully");
        }

        /// <summary>
        /// Delete a bus line.
        /// </summary>
        /// <param name="id">Bus line ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var busLine = await _context.BusLines.FindAsync(id);

            if (busLine == null)
            {
                return NotFound("Bus line not found", $"No bus line found with ID {id}");
            }

            _context.BusLines.Remove(busLine);
            await _context.SaveChangesAsync();

            return Ok<object>(null, "Bus line deleted successfully");
        }
    }
}

