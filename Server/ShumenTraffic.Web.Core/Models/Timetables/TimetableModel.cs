using ShumenTraffic.Web.Core.Models.Schedules;
using System.Collections.Generic;

namespace ShumenTraffic.Web.Core.Models.Timetables
{
    public class TimetableModel
    {
        public ScheduleModel Schedule { get; set; }
        public List<TimetableRowModel> TimetableRows { get; set; } = new List<TimetableRowModel>();
    }
}