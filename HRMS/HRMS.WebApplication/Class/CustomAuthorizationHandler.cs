using HRMS.WebApplication.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRMS.WebApplication.Class
{
    public class CustomAuthorizationHandler: IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Case 1: Not logged in or expired
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                HandleUnauthorized(context);
                return;
            }

            // Case 2: Logged in but lacks permission (if you use role/policy-based auth)
            // Example: check if user has claim or role
            var endpoint = context.HttpContext.GetEndpoint();
            var requiredPolicy = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();

            if (requiredPolicy != null && !user.IsInRole(requiredPolicy?.Roles ?? string.Empty))
            {
                HandleForbidden(context);
            }
        }

        private void HandleUnauthorized(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.IsAjaxOrApiRequest())
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    logout = true,
                    message = "Your session has expired. Please log in again."
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("UnauthorizedPage", "Account", null);
            }
        }

        private void HandleForbidden(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.IsAjaxOrApiRequest())
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    message = "You do not have permission to access this module."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}
