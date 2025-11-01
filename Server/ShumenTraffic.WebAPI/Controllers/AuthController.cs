using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.WebAPI.DTOs.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI.Controllers
{
    /// <summary>
    /// Authentication controller for user login and logout.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Login with username and password.
        /// </summary>
        /// <param name="request">Login request with username and password</param>
        /// <returns>Login response with user info and roles</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Validation failed", errors);
            }

            // Find user by username or email
            var user = await _userManager.FindByNameAsync(request.Username)
                    ?? await _userManager.FindByEmailAsync(request.Username);

            if (user == null)
            {
                return BadRequest("Login failed", "Invalid username or password");
            }

            // Check if user is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest("Login failed", "User account is locked out");
            }

            // Attempt sign in
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return BadRequest("Login failed", "User account is locked out");
                }
                if (result.RequiresTwoFactor)
                {
                    return BadRequest("Login failed", "Two-factor authentication is required");
                }
                if (result.IsNotAllowed)
                {
                    return BadRequest("Login failed", "User is not allowed to sign in");
                }

                return BadRequest("Login failed", "Invalid username or password");
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            var response = new LoginResponse
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            return Ok(response, "Login successful");
        }

        /// <summary>
        /// Logout the current user.
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok<object>(null, "Logout successful");
        }

        /// <summary>
        /// Get current user information.
        /// </summary>
        /// <returns>Current user info</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found", "Current user not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var response = new LoginResponse
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            return Ok(response, "User information retrieved successfully");
        }
    }
}

