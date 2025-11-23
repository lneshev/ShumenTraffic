using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Filters.BusLines;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models.BusLines;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Bus Lines.
    /// </summary>
    [Authorize]
    public class BusLinesLightController : EntityRestController<BusLine, int, BusLineLightModel, BusLineFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<BusLineLightModel>>>> Read([FromQuery] BusLineFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }
    }
}