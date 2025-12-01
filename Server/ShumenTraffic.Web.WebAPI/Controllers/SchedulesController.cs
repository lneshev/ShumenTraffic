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
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<ScheduleModel>>> Get([FromRoute] int id)
        {
            return base.Get(id);
        }

        public override Task<ActionResult<ApiResponse<ScheduleModel>>> Put([FromRoute] int id, [FromBody] ScheduleModel model)
        {
            return base.Put(id, model);
        }

        public override Task<ActionResult<ApiResponse<ScheduleModel>>> Delete([FromRoute] int id)
        {
            return base.Delete(id);
        }
    }
}