using ShumenTraffic.Common.Core.DTOs.Timetables;
using ShumenTraffic.Common.Core.Enums.Routes;
using System;
using System.Threading.Tasks;

namespace ShumenTraffic.Common.Services.Interfaces.Timetables
{
    public interface ITimetableService
    {
        Task<Timetable> Get(int busLineId, RouteDirection direction, DateOnly date);
    }
}