using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Schedules;
using System;
using System.Collections.Generic;

namespace ShumenTraffic.Common.Core.DTOs.Timetables
{
    public class Timetable
    {
        public Schedule Schedule { get; set; }
        public List<TimetableRow> TimetableRows { get; set; } = new List<TimetableRow>();
    }

    public class TimetableRow
    {
        public BusStop BusStop { get; set; }
        public Dictionary<int, TimeOnly?> TimesByVariant { get; set; } = new Dictionary<int, TimeOnly?>();
    }
}