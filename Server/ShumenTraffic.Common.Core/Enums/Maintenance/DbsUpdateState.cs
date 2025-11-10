namespace ShumenTraffic.Common.Core.Enums.Maintenance
{
    /// <summary>
    /// Enum indicating the update state for all databases
    /// </summary>
    public enum DbsUpdateState
    {
        /// <summary>
        /// Databases update not started or unknown state
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// All databases are created and updated successfully
        /// </summary>
        Success = 1,
        /// <summary>
        /// "App" or "Log" databases could not be updated due to an error. The rest of the databases are not modified at all. No action is needed (like restoring DB backups).
        /// </summary>
        FailNoActionNeeded = 2,
        /// <summary>
        /// "App" and "Log" databases were updated successfully, but some or all of the rest were not. Action is needed (like restoring DB backups), because the environment state is inconsistent.
        /// </summary>
        FailActionNeeded = 3
    }
}