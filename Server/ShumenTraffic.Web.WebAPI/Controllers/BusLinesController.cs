using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities;
using ShumenTraffic.Common.Core.Filters;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Bus Lines.
    /// </summary>
    [Authorize]
    public class BusLinesController : EntityRestController<BusLine, int, BusLineModel, BusLineFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<BusLineModel>>>> Read([FromQuery] BusLineFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }

        public override Task<ActionResult<BusLineModel>> Post([FromBody] BusLineModel model)
        {
            return base.Post(model);
        }

        public override async Task<ActionResult<ApiResponse<BusLineModel>>> Delete([FromRoute] int id)
        {
            return await base.Delete(id);
        }
    }
}