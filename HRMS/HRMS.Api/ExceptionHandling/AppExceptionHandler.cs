using HRMS.Domain.Exception;
using Microsoft.AspNetCore.Diagnostics;

namespace HRMS.Api.ExceptionHandling
{
    public class AppExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<AppExceptionHandler> _logger;
        public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            (int statusCode, string errorMessage) = exception switch
            {
                DomainNotFoundException => (400, ""),
                DomainBadReqestException => (400, ""),
                DomainTooManyRequestException => (429, ""),
                _ => (500, "")
            };
            _logger.LogError(exception, exception.Message);
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(errorMessage);
            return true;
        }
    }
}
