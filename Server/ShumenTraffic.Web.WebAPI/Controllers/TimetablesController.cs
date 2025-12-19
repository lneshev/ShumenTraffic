using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models.Timetables;
using ShumenTraffic.Web.Services.Interfaces.Timetables;
using ShumenTraffic.Web.WebAPI.Infrastructure.Constants;
using System;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    [ApiController]
    [Route(RoutingConstants.ApiController)]
    public class TimetablesController : ControllerBase
    {
        private readonly ITimetableModelService timetableModelService;

        public TimetablesController(ITimetableModelService timetableModelService)
        {
            this.timetableModelService = timetableModelService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<TimetableModel>>> Get([FromQuery] int busLineId, [FromQuery] RouteDirection direction, [FromQuery] DateOnly date)
        {
            var data = await timetableModelService.Get(busLineId, direction, date);
            var result = ApiResponse<TimetableModel>.SuccessResponse(data);
            return result;
        }
    }
}