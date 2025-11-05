using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Web.WebAPI.DTOs;
using ShumenTraffic.Web.WebAPI.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Routes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoutesController : BaseController
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
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
            var routes = await _routeService.GetAllAsync(busLineId, includeInactive);
            var routesList = routes.ToList();
            return Ok(routesList, $"Retrieved {routesList.Count} routes");
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
            var route = await _routeService.GetByIdAsync(id);

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

            var (result, error) = await _routeService.CreateAsync(dto);

            if (error != null)
            {
                return BadRequest("Route creation failed", error);
            }

            return Created(nameof(GetById), nameof(RoutesController), new { id = result.Id }, result, "Route created successfully");
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

            var result = await _routeService.UpdateAsync(id, dto);

            if (result == null)
            {
                return NotFound("Route not found", $"No route found with ID {id}");
            }

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
            var deleted = await _routeService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Route not found", $"No route found with ID {id}");
            }

            return Ok<object>(null, "Route deleted successfully");
        }
    }
}

