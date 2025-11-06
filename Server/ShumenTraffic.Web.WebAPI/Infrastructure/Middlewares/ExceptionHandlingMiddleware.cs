using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MoravianStar.WebAPI.Middlewares;
using ShumenTraffic.Web.Core.DTOs;
using System;

namespace ShumenTraffic.Web.WebAPI.Infrastructure.Middlewares
{
    /// <inheritdoc/>
    public class ExceptionHandlingMiddleware : ExceptionMiddleware
    {
        public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env, ILoggerFactory loggerFactory) : base(next, env, loggerFactory)
        {
        }

        protected override object SetErrorModel(Exception exception)
        {
            return ApiResponse.ErrorResponse(exception.Message);
        }
    }
}