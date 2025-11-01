using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShumenTraffic.Data.Context;
using ShumenTraffic.Data.Models;
using ShumenTraffic.WebAPI.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Transportation Companies.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransportationCompaniesController : BaseController
    {
        private readonly ShumenTrafficDbContext _context;

        public TransportationCompaniesController(ShumenTrafficDbContext context)
        {
            _context = context;
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
            var query = _context.TransportationCompanies.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            var companies = await query
                .OrderBy(c => c.Name)
                .Select(c => new TransportationCompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return Ok(companies, $"Retrieved {companies.Count} transportation companies");
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
            var company = await _context.TransportationCompanies
                .Where(c => c.Id == id)
                .Select(c => new TransportationCompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                })
                .FirstOrDefaultAsync();

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

            var company = new TransportationCompany
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };

            // Check if transportation company already exists
            var existingCompany = await _context.Set<TransportationCompany>().Where(x => x.Name == dto.Name).AnyAsync();
            if (existingCompany)
            {
                return Conflict("Transportation company already exists", $"A transportation company with name '{dto.Name}' already exists");
            }

            _context.TransportationCompanies.Add(company);
            await _context.SaveChangesAsync();

            var result = new TransportationCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                IsActive = company.IsActive
            };

            return Created(nameof(GetById), nameof(TransportationCompaniesController), new { id = company.Id }, result, "Transportation company created successfully");
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

            var company = await _context.TransportationCompanies.FindAsync(id);

            if (company == null)
            {
                return NotFound("Transportation company not found", $"No company found with ID {id}");
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                // Check if new line number already exists
                var existingCompany = await _context.Set<TransportationCompany>().Where(x => x.Name == dto.Name && x.Id != id).AnyAsync();
                if (existingCompany)
                {
                    return Conflict("Transportation company already exists", $"A transportation company with name '{dto.Name}' already exists");
                }
                company.Name = dto.Name;
            }
            if (dto.Description != null)
            {
                company.Description = dto.Description;
            }
            if (dto.IsActive.HasValue)
            {
                company.IsActive = dto.IsActive.Value;
            }

            _context.TransportationCompanies.Update(company);
            await _context.SaveChangesAsync();

            var result = new TransportationCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                IsActive = company.IsActive
            };

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
            var company = await _context.TransportationCompanies.FindAsync(id);

            if (company == null)
            {
                return NotFound("Transportation company not found", $"No company found with ID {id}");
            }

            _context.TransportationCompanies.Remove(company);
            await _context.SaveChangesAsync();

            return Ok<object>(null, "Transportation company deleted successfully");
        }
    }
}

