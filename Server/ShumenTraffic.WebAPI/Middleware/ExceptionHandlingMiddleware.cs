using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShumenTraffic.WebAPI.Common;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions and returning consistent error responses.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exceptions and returns appropriate error responses.
        /// </summary>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                ArgumentNullException => new
                {
                    statusCode = HttpStatusCode.BadRequest,
                    message = "Invalid request",
                    error = exception.Message
                },
                ArgumentException => new
                {
                    statusCode = HttpStatusCode.BadRequest,
                    message = "Invalid argument",
                    error = exception.Message
                },
                InvalidOperationException => new
                {
                    statusCode = HttpStatusCode.BadRequest,
                    message = "Invalid operation",
                    error = exception.Message
                },
                _ => new
                {
                    statusCode = HttpStatusCode.InternalServerError,
                    message = "An internal server error occurred",
                    error = "Please try again later"
                }
            };

            context.Response.StatusCode = (int)response.statusCode;

            var apiResponse = ApiResponse.ErrorResponse(response.message, response.error);
            return context.Response.WriteAsJsonAsync(apiResponse);
        }
    }
}

