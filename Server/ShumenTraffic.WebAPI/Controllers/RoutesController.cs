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
    /// Controller for managing Routes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoutesController : BaseController
    {
        private readonly ShumenTrafficDbContext _context;

        public RoutesController(ShumenTrafficDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all routes.
        /// </summary>
        /// <param name="busLineId">Filter by bus line ID (optional)</param>
        /// <param name="includeInactive">Include inactive routes</param>
        /// <returns>List of routes</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int? busLineId = null, [FromQuery] bool includeInactive = false)
        {
            var query = _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .AsQueryable();

            if (busLineId.HasValue)
            {
                query = query.Where(r => r.BusLineId == busLineId.Value);
            }

            if (!includeInactive)
            {
                query = query.Where(r => r.IsActive);
            }

            var routes = await query
                .OrderBy(r => r.BusLineId)
                .ThenBy(r => r.Direction)
                .Select(r => new RouteDto
                {
                    Id = r.Id,
                    BusLineId = r.BusLineId,
                    BusLineNumber = r.BusLine.LineNumber,
                    Direction = r.Direction,
                    Name = r.Name,
                    IsActive = r.IsActive,
                    Stops = r.RouteStops
                        .OrderBy(rs => rs.StopOrder)
                        .Select(rs => new RouteStopDto
                        {
                            Id = rs.Id,
                            BusStopId = rs.BusStopId,
                            BusStopName = rs.BusStop.Name,
                            Latitude = rs.Latitude,
                            Longitude = rs.Longitude,
                            StopOrder = rs.StopOrder,
                            EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(routes, $"Retrieved {routes.Count} routes");
        }

        /// <summary>
        /// Get a specific route by ID.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>Route details with all stops</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .Where(r => r.Id == id)
                .Select(r => new RouteDto
                {
                    Id = r.Id,
                    BusLineId = r.BusLineId,
                    BusLineNumber = r.BusLine.LineNumber,
                    Direction = r.Direction,
                    Name = r.Name,
                    IsActive = r.IsActive,
                    Stops = r.RouteStops
                        .OrderBy(rs => rs.StopOrder)
                        .Select(rs => new RouteStopDto
                        {
                            Id = rs.Id,
                            BusStopId = rs.BusStopId,
                            BusStopName = rs.BusStop.Name,
                            Latitude = rs.Latitude,
                            Longitude = rs.Longitude,
                            StopOrder = rs.StopOrder,
                            EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (route == null)
            {
                return NotFound("Route not found", $"No route found with ID {id}");
            }

            return Ok(route, "Route retrieved successfully");
        }

        /// <summary>
        /// Create a new route with stops.
        /// </summary>
        /// <param name="dto">Create route DTO</param>
        /// <returns>Created route</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRouteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            // Verify bus line exists
            var busLine = await _context.BusLines.FindAsync(dto.BusLineId);
            if (busLine == null)
            {
                return BadRequest("Invalid bus line", $"Bus line with ID {dto.BusLineId} does not exist");
            }

            // Verify bus stops exist (if provided)
            var busStopIds = dto.Stops.Where(s => s.BusStopId.HasValue).Select(s => s.BusStopId.Value).ToList();
            if (busStopIds.Any())
            {
                var existingStops = await _context.BusStops.Where(b => busStopIds.Contains(b.Id)).CountAsync();
                if (existingStops != busStopIds.Count)
                {
                    return BadRequest("Invalid bus stops", "One or more bus stops do not exist");
                }
            }

            var route = new Route
            {
                BusLineId = dto.BusLineId,
                Direction = dto.Direction,
                Name = dto.Name,
                IsActive = true
            };

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            // Add route stops
            foreach (var stopDto in dto.Stops.OrderBy(s => s.StopOrder))
            {
                var routeStop = new RouteStop
                {
                    RouteId = route.Id,
                    BusStopId = stopDto.BusStopId,
                    Latitude = stopDto.Latitude,
                    Longitude = stopDto.Longitude,
                    StopOrder = stopDto.StopOrder,
                    EstimatedMinutesFromStart = stopDto.EstimatedMinutesFromStart
                };
                _context.RouteStops.Add(routeStop);
            }

            await _context.SaveChangesAsync();

            // Reload route with stops
            var createdRoute = await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .FirstAsync(r => r.Id == route.Id);

            var result = new RouteDto
            {
                Id = createdRoute.Id,
                BusLineId = createdRoute.BusLineId,
                BusLineNumber = createdRoute.BusLine.LineNumber,
                Direction = createdRoute.Direction,
                Name = createdRoute.Name,
                IsActive = createdRoute.IsActive,
                Stops = createdRoute.RouteStops
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs => new RouteStopDto
                    {
                        Id = rs.Id,
                        BusStopId = rs.BusStopId,
                        BusStopName = rs.BusStop?.Name,
                        Latitude = rs.Latitude,
                        Longitude = rs.Longitude,
                        StopOrder = rs.StopOrder,
                        EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                    })
                    .ToList()
            };

            return Created(nameof(GetById), nameof(RoutesController), new { id = route.Id }, result, "Route created successfully");
        }

        /// <summary>
        /// Update an existing route.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <param name="dto">Update route DTO</param>
        /// <returns>Updated route</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRouteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var route = await _context.Routes
                .Include(r => r.BusLine)
                .Include(r => r.RouteStops)
                .ThenInclude(rs => rs.BusStop)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Route not found", $"No route found with ID {id}");
            }

            if (dto.Direction.HasValue)
                route.Direction = dto.Direction.Value;
            if (dto.Name != null)
                route.Name = dto.Name;
            if (dto.IsActive.HasValue)
                route.IsActive = dto.IsActive.Value;

            _context.Routes.Update(route);
            await _context.SaveChangesAsync();

            var result = new RouteDto
            {
                Id = route.Id,
                BusLineId = route.BusLineId,
                BusLineNumber = route.BusLine.LineNumber,
                Direction = route.Direction,
                Name = route.Name,
                IsActive = route.IsActive,
                Stops = route.RouteStops
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs => new RouteStopDto
                    {
                        Id = rs.Id,
                        BusStopId = rs.BusStopId,
                        BusStopName = rs.BusStop?.Name,
                        Latitude = rs.Latitude,
                        Longitude = rs.Longitude,
                        StopOrder = rs.StopOrder,
                        EstimatedMinutesFromStart = rs.EstimatedMinutesFromStart
                    })
                    .ToList()
            };

            return Ok(result, "Route updated successfully");
        }

        /// <summary>
        /// Delete a route.
        /// </summary>
        /// <param name="id">Route ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var route = await _context.Routes
                .Include(r => r.RouteStops)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null)
            {
                return NotFound("Route not found", $"No route found with ID {id}");
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return Ok<object>(null, "Route deleted successfully");
        }
    }
}

