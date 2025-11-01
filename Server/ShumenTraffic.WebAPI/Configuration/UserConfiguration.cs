namespace ShumenTraffic.WebAPI.Configuration
{
    /// <summary>
    /// Configuration for a user.
    /// </summary>
    public class UserConfiguration
    {
        /// <summary>
        /// User's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's password.
        /// </summary>
        public string Password { get; set; }
    }
}