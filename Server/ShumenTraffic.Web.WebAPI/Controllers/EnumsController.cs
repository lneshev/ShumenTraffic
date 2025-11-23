using Microsoft.AspNetCore.Mvc;
using MoravianStar.Extensions;
using MoravianStar.WebAPI.Helpers;
using ShumenTraffic.Web.Core.DTOs;
using ShumenTraffic.Web.WebAPI.Infrastructure.Constants;
using System.Collections.Generic;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    [ApiController]
    [Route(RoutingConstants.ApiController)]
    public class EnumsController : ControllerBase
    {
        private readonly EnumsControllerHelper helper;

        public EnumsController()
        {
            helper = new EnumsControllerHelper();
        }

        [HttpGet]
        public virtual ActionResult<ApiResponse<List<EnumNameValue>>> Get()
        {
            var data = helper.Get();
            var result = ApiResponse<List<EnumNameValue>>.SuccessResponse(data);
            return result;
        }

        [HttpGet("{enumName}")]
        public virtual ActionResult<ApiResponse<List<EnumTextValue>>> Get([FromRoute] string enumName, [FromQuery] List<int> exactEnumValues, [FromQuery] bool sortByText = false)
        {
            var data = helper.Get(enumName, exactEnumValues, sortByText);
            var result = ApiResponse<List<EnumTextValue>>.SuccessResponse(data);
            return result;
        }
    }
}