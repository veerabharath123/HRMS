using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZXing;

namespace HRMS.Api.Class
{
    public class ValidationModelAttribute : ActionFilterAttribute
    {
        private readonly ILogger<ValidationModelAttribute> _logger;
        public ValidationModelAttribute(ILogger<ValidationModelAttribute> logger)
        {
            _logger = logger;
        }
        async public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsList = context.ModelState
                    .Where(x => x.Value is not null && x.Value.Errors.Count > 0)
                    .Select(kvp => $"{kvp.Key} : {string.Join(" | ", kvp.Value!.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();

                await LogValidationErrorAsync(errorsList);

                var response = ApiResponseDto<object?>.CustomStatus(StatusCodes.Status400BadRequest, false, null, "Validation failed");

                context.Result = new BadRequestObjectResult(response);

                return;
            }
            await next();
        }

        async public Task LogValidationErrorAsync(List<string> errorsList)
        {
            var stringErrors = string.Join("; ", errorsList);
            _logger.LogError("Validation error occurred. {Errors}",stringErrors);
        }
    }
}
