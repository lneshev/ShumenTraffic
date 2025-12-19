using ShumenTraffic.Common.Core.Enums.Common;
using System;

namespace ShumenTraffic.Common.Core.Extensions
{
    public static class DateTimeExtensions
    {
        #region DayOfWeekExtensions
        public static DaysOfWeek ToDaysOfWeek(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => DaysOfWeek.Monday,
                DayOfWeek.Tuesday => DaysOfWeek.Tuesday,
                DayOfWeek.Wednesday => DaysOfWeek.Wednesday,
                DayOfWeek.Thursday => DaysOfWeek.Thursday,
                DayOfWeek.Friday => DaysOfWeek.Friday,
                DayOfWeek.Saturday => DaysOfWeek.Saturday,
                DayOfWeek.Sunday => DaysOfWeek.Sunday,
                _ => throw new NotSupportedException()
            };
        }
        #endregion
    }
}