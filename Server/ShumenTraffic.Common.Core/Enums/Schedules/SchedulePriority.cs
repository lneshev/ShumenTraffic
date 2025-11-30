namespace ShumenTraffic.Common.Core.Enums.Schedules
{
    /// <summary>
    /// Represents the priority level of a schedule.
    /// Used to differentiate between multiple schedules with the same BusLineId, DayType, and date range.
    /// </summary>
    public enum SchedulePriority
    {
        /// <summary>
        /// Normal priority schedule (default).
        /// </summary>
        Normal = 0,

        /// <summary>
        /// High priority schedule.
        /// </summary>
        High = 1
    }
}