using MoravianStar.Dao;
using System;

namespace ShumenTraffic.Web.Core.Models.Schedules
{
    public class ScheduleCourseModel : ModelBase<int>
    {
        public TimeOnly DepartureTime { get; set; }
        public int RouteId { get; set; }
    }
}