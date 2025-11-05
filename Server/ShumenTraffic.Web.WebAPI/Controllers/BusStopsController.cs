using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Web.Core.Models;
using ShumenTraffic.Web.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Bus Stops.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BusStopsController : BaseController
    {
        private readonly IBusStopModelService _busStopService;

        public BusStopsController(IBusStopModelService busStopService)
        {
            _busStopService = busStopService;
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
            var busStops = await _busStopService.GetAllAsync(zoneId, includeInactive);
            var busStopsList = busStops.ToList();
            return Ok(busStopsList, $"Retrieved {busStopsList.Count} bus stops");
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
            var busStop = await _busStopService.GetByIdAsync(id);

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

            var (result, error) = await _busStopService.CreateAsync(dto);

            if (error != null)
            {
                return BadRequest("Bus stop creation failed", error);
            }

            return Created(nameof(GetById), nameof(BusStopsController), new { id = result.Id }, result, "Bus stop created successfully");
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

            var (result, error) = await _busStopService.UpdateAsync(id, dto);

            if (error != null)
            {
                if (error.Contains("not found"))
                {
                    return NotFound("Bus stop not found", error);
                }
                return BadRequest("Bus stop update failed", error);
            }

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
            var deleted = await _busStopService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Bus stop not found", $"No bus stop found with ID {id}");
            }

            return Ok<object>(null, "Bus stop deleted successfully");
        }
    }
}