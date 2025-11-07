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
    /// Controller for managing Transportation Companies.
    /// </summary>
    [Authorize]
    public class TransportationCompaniesController : EntityRestController<TransportationCompany, int, TransportationCompanyModel, TransportationCompanyFilter>
    {
        [AllowAnonymous]
        public override Task<ActionResult<ApiResponse<PageResult<TransportationCompanyModel>>>> Read([FromQuery] TransportationCompanyFilter filter, [FromQuery] List<Sort> sorts, [FromQuery] Page page)
        {
            return base.Read(filter, sorts, page);
        }

        public override Task<ActionResult<TransportationCompanyModel>> Post([FromBody] TransportationCompanyModel model)
        {
            return base.Post(model);
        }

        public override Task<ActionResult<ApiResponse<TransportationCompanyModel>>> Delete([FromRoute] int id)
        {
            return base.Delete(id);
        }
    }
}