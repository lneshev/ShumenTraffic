using Microsoft.AspNetCore.Mvc;
using MoravianStar.Extensions;
using ShumenTraffic.Web.Core.DTOs;
using System.Collections.Generic;

namespace ShumenTraffic.Web.WebAPI.Controllers
{
    /// <summary>
    /// Base controller for all API controllers.
    /// Provides common functionality and response handling.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Returns a successful response with data.
        /// </summary>
        protected OkObjectResult Ok<T>(T data, string message = "Request successful")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return base.Ok(response);
        }

        /// <summary>
        /// Returns a created response with data.
        /// </summary>
        protected CreatedAtActionResult Created<T>(string actionName, string routeName, object routeValues, T data, string message = "Resource created successfully")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return base.CreatedAtAction(actionName, routeName?.TrimEnd("Controller"), routeValues, response);
        }

        /// <summary>
        /// Returns a no content response.
        /// </summary>
        protected new NoContentResult NoContent()
        {
            return base.NoContent();
        }

        /// <summary>
        /// Returns a bad request response.
        /// </summary>
        protected BadRequestObjectResult BadRequest(string message, string error)
        {
            var response = ApiResponse.ErrorResponse(message, error);
            return base.BadRequest(response);
        }

        /// <summary>
        /// Returns a bad request response with multiple errors.
        /// </summary>
        protected BadRequestObjectResult BadRequest(string message, List<string> errors)
        {
            var response = ApiResponse.ErrorResponse(message, errors);
            return base.BadRequest(response);
        }

        /// <summary>
        /// Returns a not found response.
        /// </summary>
        protected NotFoundObjectResult NotFound(string message, string error = "Resource not found")
        {
            var response = ApiResponse.ErrorResponse(message, error);
            return base.NotFound(response);
        }

        /// <summary>
        /// Returns an unauthorized response.
        /// </summary>
        protected UnauthorizedObjectResult Unauthorized(string message, string error = "Unauthorized access")
        {
            var response = ApiResponse.ErrorResponse(message, error);
            return base.Unauthorized(response);
        }

        /// <summary>
        /// Returns a conflict response.
        /// </summary>
        protected ConflictObjectResult Conflict(string message, string error)
        {
            var response = ApiResponse.ErrorResponse(message, error);
            return base.Conflict(response);
        }

        /// <summary>
        /// Returns an internal server error response.
        /// </summary>
        protected ObjectResult InternalServerError(string message, string error = "An internal server error occurred")
        {
            var response = ApiResponse.ErrorResponse(message, error);
            return base.StatusCode(500, response);
        }
    }
}