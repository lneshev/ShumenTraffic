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
    /// Controller for managing Bus Stops.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BusStopsController : BaseController
    {
        private readonly ShumenTrafficDbContext _context;

        public BusStopsController(ShumenTrafficDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all bus stops.
        /// </summary>
        /// <param name="zoneId">Filter by zone ID (optional)</param>
        /// <param name="includeInactive">Include inactive bus stops</param>
        /// <returns>List of bus stops</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int? zoneId = null, [FromQuery] bool includeInactive = false)
        {
            var query = _context.BusStops.Include(b => b.Zone).AsQueryable();

            if (zoneId.HasValue)
            {
                query = query.Where(b => b.ZoneId == zoneId.Value);
            }

            if (!includeInactive)
            {
                query = query.Where(b => b.IsActive);
            }

            var busStops = await query
                .OrderBy(b => b.Name)
                .Select(b => new BusStopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ZoneId = b.ZoneId,
                    ZoneName = b.Zone.Name,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            return Ok(busStops, $"Retrieved {busStops.Count} bus stops");
        }

        /// <summary>
        /// Get a specific bus stop by ID.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <returns>Bus stop details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var busStop = await _context.BusStops
                .Include(b => b.Zone)
                .Where(b => b.Id == id)
                .Select(b => new BusStopDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ZoneId = b.ZoneId,
                    ZoneName = b.Zone.Name,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    IsActive = b.IsActive
                })
                .FirstOrDefaultAsync();

            if (busStop == null)
            {
                return NotFound("Bus stop not found", $"No bus stop found with ID {id}");
            }

            return Ok(busStop, "Bus stop retrieved successfully");
        }

        /// <summary>
        /// Create a new bus stop.
        /// </summary>
        /// <param name="dto">Create bus stop DTO</param>
        /// <returns>Created bus stop</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBusStopDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            // Verify zone exists
            var zone = await _context.Zones.FindAsync(dto.ZoneId);
            if (zone == null)
            {
                return BadRequest("Invalid zone", $"Zone with ID {dto.ZoneId} does not exist");
            }

            var busStop = new BusStop
            {
                Name = dto.Name,
                ZoneId = dto.ZoneId,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsActive = true
            };

            _context.BusStops.Add(busStop);
            await _context.SaveChangesAsync();

            var result = new BusStopDto
            {
                Id = busStop.Id,
                Name = busStop.Name,
                ZoneId = busStop.ZoneId,
                ZoneName = zone.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude,
                IsActive = busStop.IsActive
            };

            return Created(nameof(GetById), nameof(BusStopsController), new { id = busStop.Id }, result, "Bus stop created successfully");
        }

        /// <summary>
        /// Update an existing bus stop.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <param name="dto">Update bus stop DTO</param>
        /// <returns>Updated bus stop</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBusStopDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var busStop = await _context.BusStops.Include(b => b.Zone).FirstOrDefaultAsync(b => b.Id == id);

            if (busStop == null)
            {
                return NotFound("Bus stop not found", $"No bus stop found with ID {id}");
            }

            if (!string.IsNullOrEmpty(dto.Name))
                busStop.Name = dto.Name;
            if (dto.ZoneId.HasValue)
            {
                var zone = await _context.Zones.FindAsync(dto.ZoneId.Value);
                if (zone == null)
                {
                    return BadRequest("Invalid zone", $"Zone with ID {dto.ZoneId} does not exist");
                }
                busStop.ZoneId = dto.ZoneId.Value;
            }
            if (dto.Latitude.HasValue)
                busStop.Latitude = dto.Latitude.Value;
            if (dto.Longitude.HasValue)
                busStop.Longitude = dto.Longitude.Value;
            if (dto.IsActive.HasValue)
                busStop.IsActive = dto.IsActive.Value;

            _context.BusStops.Update(busStop);
            await _context.SaveChangesAsync();

            var result = new BusStopDto
            {
                Id = busStop.Id,
                Name = busStop.Name,
                ZoneId = busStop.ZoneId,
                ZoneName = busStop.Zone.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude,
                IsActive = busStop.IsActive
            };

            return Ok(result, "Bus stop updated successfully");
        }

        /// <summary>
        /// Delete a bus stop.
        /// </summary>
        /// <param name="id">Bus stop ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var busStop = await _context.BusStops.FindAsync(id);

            if (busStop == null)
            {
                return NotFound("Bus stop not found", $"No bus stop found with ID {id}");
            }

            _context.BusStops.Remove(busStop);
            await _context.SaveChangesAsync();

            return Ok<object>(null, "Bus stop deleted successfully");
        }
    }
}

