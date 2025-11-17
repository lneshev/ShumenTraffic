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
    public class ZonesController : EntityRestController<Zone, int, ZoneModel, ZoneFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<ZoneModel>>>> Read([FromQuery] ZoneFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }

        public override Task<ActionResult<ZoneModel>> Post([FromBody] ZoneModel model)
        {
            return base.Post(model);
        }

        public override Task<ActionResult<ApiResponse<ZoneModel>>> Delete([FromRoute] int id)
        {
            return base.Delete(id);
        }
    }
}