using ShumenTraffic.Web.Core.Models.BusStops;
using System;
using System.Collections.Generic;

namespace ShumenTraffic.Web.Core.Models.Timetables
{
    public class TimetableRowModel
    {
        public BusStopModel BusStop { get; set; }
        public Dictionary<int, TimeOnly?> TimesByVariant { get; set; } = new Dictionary<int, TimeOnly?>();
    }
}