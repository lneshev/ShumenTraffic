namespace ShumenTraffic.Common.Core.Enums.Maintenance
{
    /// <summary>
    /// Enum indicating the update state for a single database
    /// </summary>
    public enum DbUpdateState
    {
        /// <summary>
        /// Database update not started or unknown state
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Database update success
        /// </summary>
        Success = 1,
        /// <summary>
        /// Database update fail
        /// </summary>
        Fail = 2
    }
}