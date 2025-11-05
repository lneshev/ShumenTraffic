using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Web.WebAPI.DTOs;
using ShumenTraffic.Web.WebAPI.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Bus Lines.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BusLinesController : BaseController
    {
        private readonly IBusLineService _busLineService;

        public BusLinesController(IBusLineService busLineService)
        {
            _busLineService = busLineService;
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
            var busLines = await _busLineService.GetAllAsync(includeInactive);
            var busLinesList = busLines.ToList();
            return Ok(busLinesList, $"Retrieved {busLinesList.Count} bus lines");
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
            var busLine = await _busLineService.GetByIdAsync(id);

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

            var (result, error) = await _busLineService.CreateAsync(dto);

            if (error != null)
            {
                return Conflict("Bus line creation failed", error);
            }

            return Created(nameof(GetById), nameof(BusLinesController), new { id = result.Id }, result, "Bus line created successfully");
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

            var (result, error) = await _busLineService.UpdateAsync(id, dto);

            if (error != null)
            {
                if (error.Contains("not found"))
                {
                    return NotFound("Bus line not found", error);
                }
                return Conflict("Bus line update failed", error);
            }

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
            var deleted = await _busLineService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Bus line not found", $"No bus line found with ID {id}");
            }

            return Ok<object>(null, "Bus line deleted successfully");
        }
    }
}