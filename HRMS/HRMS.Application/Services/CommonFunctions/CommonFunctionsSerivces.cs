using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services.CommonFunctions
{
    public abstract class CommonFunctionsSerivces
    {
        protected IHttpContextAccessor _httpContextAccessor;
        protected string GetCurrentUsername() => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        protected string GetCurrentUserId() => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
    }
}
