using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShumenTraffic.WebAPI.Common;
using System.Collections.Generic;
using System.Linq;

namespace ShumenTraffic.WebAPI.Filters
{
    /// <summary>
    /// Action filter for validating model state and returning consistent error responses.
    /// </summary>
    public class ValidationFilter : IActionFilter
    {
        /// <summary>
        /// Executes before the action method.
        /// </summary>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var response = ApiResponse.ErrorResponse("Validation failed", errors);
                context.Result = new BadRequestObjectResult(response);
            }
        }

        /// <summary>
        /// Executes after the action method.
        /// </summary>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No post-action processing needed
        }
    }
}

