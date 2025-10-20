using HRMS.WebApplication.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRMS.WebApplication.Class
{
    public class CustomExceptionHandler : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // You can customize response based on request type
            if (context.HttpContext.Request.IsAjaxOrApiRequest())
            {
                // API or AJAX request
                context.Result = new JsonResult(new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again later.",
                });
                context.HttpContext.Response.StatusCode = 500; // Internal Server Error
            }
            else
            {
                // Normal MVC request (HTML)
                context.Result = new ViewResult
                {
                    ViewName = "Error" // create a Views/Shared/Error.cshtml file
                };
            }

            context.ExceptionHandled = true; // prevent further propagation
        }
    }
}
