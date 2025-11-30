using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Filters.Schedules;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models.Schedules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Schedules.
    /// </summary>
    [Authorize]
    public class SchedulesOverviewController : EntityRestController<Schedule, int, ScheduleOverviewModel, ScheduleFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<ScheduleOverviewModel>>>> Read([FromQuery] ScheduleFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }

        public override Task<ActionResult<ScheduleOverviewModel>> Post([FromBody] ScheduleOverviewModel model)
        {
            return base.Post(model);
        }
    }
}