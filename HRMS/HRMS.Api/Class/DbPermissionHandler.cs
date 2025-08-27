using System.Security.Claims;
using HRMS.Application.Services;
using HRMS.Domain.Authorization;
using HRMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace HRMS.Api.Class
{
    public class DbPermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbPermissionHandler(IMemoryCache cache, IHttpContextAccessor accessor)
        {
            _cache = cache;
            _httpContextAccessor = accessor;

        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim?.Value, out Guid userId)) return;

            var cacheKey = $"permissions_{userId}";

            if (!_cache.TryGetValue(cacheKey, out List<string>? permissions) || permissions is null)
            {
                if (_httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(IUserServices)) is not IUserServices userServices)
                {
                    context.Fail();
                    return;
                }

                permissions = await userServices.GetPermissionsByUserIdAsync(userId);
                _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(15));
            }

            if (permissions.Contains(requirement.PermissionName))
            {
                _httpContextAccessor.HttpContext?.Items.Add(GeneralConstants.USER_PERMISSION_KEY, permissions);
                context.Succeed(requirement);
            }
        }
    }
}
