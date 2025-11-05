using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Web.WebAPI.DTOs;
using ShumenTraffic.Web.WebAPI.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Transportation Companies.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransportationCompaniesController : BaseController
    {
        private readonly ITransportationCompanyService _transportationCompanyService;

        public TransportationCompaniesController(ITransportationCompanyService transportationCompanyService)
        {
            _transportationCompanyService = transportationCompanyService;
        }

        /// <summary>
        /// Get all transportation companies.
        /// </summary>
        /// <param name="includeInactive">Include inactive companies</param>
        /// <returns>List of transportation companies</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var companies = await _transportationCompanyService.GetAllAsync(includeInactive);
            var companiesList = companies.ToList();
            return Ok(companiesList, $"Retrieved {companiesList.Count} transportation companies");
        }

        /// <summary>
        /// Get a specific transportation company by ID.
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>Transportation company details</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _transportationCompanyService.GetByIdAsync(id);

            if (company == null)
            {
                return NotFound("Transportation company not found", $"No company found with ID {id}");
            }

            return Ok(company, "Transportation company retrieved successfully");
        }

        /// <summary>
        /// Create a new transportation company.
        /// </summary>
        /// <param name="dto">Create company DTO</param>
        /// <returns>Created company</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransportationCompanyDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var (result, error) = await _transportationCompanyService.CreateAsync(dto);

            if (error != null)
            {
                return Conflict("Transportation company creation failed", error);
            }

            return Created(nameof(GetById), nameof(TransportationCompaniesController), new { id = result.Id }, result, "Transportation company created successfully");
        }

        /// <summary>
        /// Update an existing transportation company.
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="dto">Update company DTO</param>
        /// <returns>Updated company</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTransportationCompanyDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            var (result, error) = await _transportationCompanyService.UpdateAsync(id, dto);

            if (error != null)
            {
                if (error.Contains("not found"))
                {
                    return NotFound("Transportation company not found", error);
                }
                return Conflict("Transportation company update failed", error);
            }

            return Ok(result, "Transportation company updated successfully");
        }

        /// <summary>
        /// Delete a transportation company.
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _transportationCompanyService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Transportation company not found", $"No company found with ID {id}");
            }

            return Ok<object>(null, "Transportation company deleted successfully");
        }
    }
}

