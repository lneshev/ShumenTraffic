using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShumenTraffic.Common.Core.Entities.Schedules;
using ShumenTraffic.Common.Core.Filters.Schedules;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models.Schedules;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Schedules.
    /// </summary>
    [Authorize]
    public class SchedulesController : EntityRestController<Schedule, int, ScheduleModel, ScheduleFilter>
    {
        public override Task<ActionResult<ApiResponse<ScheduleModel>>> Delete([FromRoute] int id)
        {
            return base.Delete(id);
        }
    }
}