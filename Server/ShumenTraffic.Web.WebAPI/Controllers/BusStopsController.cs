using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Filters.BusStops;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models.BusStops;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Bus Stops.
    /// </summary>
    [Authorize]
    public class BusStopsController : EntityRestController<BusStop, int, BusStopModel, BusStopFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<BusStopModel>>>> Read([FromQuery] BusStopFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }

        public override Task<ActionResult<BusStopModel>> Post([FromBody] BusStopModel model)
        {
            return base.Post(model);
        }

        public override Task<ActionResult<ApiResponse<BusStopModel>>> Put([FromRoute] int id, [FromBody] BusStopModel model)
        {
            return base.Put(id, model);
        }

        public override Task<ActionResult<ApiResponse<BusStopModel>>> Delete([FromRoute] int id)
        {
            return base.Delete(id);
        }
    }
}