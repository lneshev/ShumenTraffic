using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoravianStar.Dao;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Filters.Routes;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.Core.Models.Routes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Controller for managing Routes.
    /// </summary>
    [Authorize]
    public class RoutesController : EntityRestController<Route, int, RouteModel, RouteFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<RouteModel>>>> Read([FromQuery] RouteFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }

        public override Task<ActionResult<RouteModel>> Post([FromBody] RouteModel model)
        {
            return base.Post(model);
        }

        public override Task<ActionResult<ApiResponse<RouteModel>>> Delete([FromRoute] int id)
        {
            return base.Delete(id);
        }
    }
}