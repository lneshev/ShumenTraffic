using System.ComponentModel.DataAnnotations;

namespace ShumenTraffic.Web.Core.Models.Auth
{
    /// <summary>
    /// Login request DTO.
    /// </summary>
    public class LoginRequestModel
    {
        /// <summary>
        /// Username or email.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters")]
        public string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 4, ErrorMessage = "Password must be at least 4 characters")]
        public string Password { get; set; }
    }
}

