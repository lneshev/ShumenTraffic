using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Common.Core.Filters.Zones;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models.Zones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Zones.
    /// </summary>
    [Authorize]
    public class ZonesWithBusLinesController : EntityRestController<Zone, int, ZoneWithBusLinesModel, ZoneFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<ZoneWithBusLinesModel>>>> Read([FromQuery] ZoneFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }
    }
}