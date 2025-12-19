using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Web.Core.Models.Timetables;
using System;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.Services.Interfaces.Timetables
{
    public interface ITimetableModelService
    {
        Task<TimetableModel> Get(int busLineId, RouteDirection direction, DateOnly date);
    }
}