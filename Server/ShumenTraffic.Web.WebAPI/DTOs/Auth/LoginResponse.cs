using System.Collections.Generic;

namespace ShumenTraffic.Web.WebAPI.DTOs.Auth
{
    /// <summary>
    /// Login response DTO.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// List of user roles.
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
}