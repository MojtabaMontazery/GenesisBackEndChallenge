using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndChallenge.Utilities
{
    /// <summary>
    /// The attribute makes sure that the security token is passed in the header.
    /// </summary>
    public class JwtAuthenticationAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// The method makes sure that the security token is passed in the header.
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            string authorization = request.Headers["Authorization"];
            if (authorization == null || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(authorization.Substring("Bearer ".Length).Trim()))
            {
                context.Result = new ObjectResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

    }

    //public class ValidateModelAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        if (!context.ModelState.IsValid)
    //        {
    //            context.Result = new BadRequestObjectResult(context.ModelState);
    //        }
    //    }

    //}
}
