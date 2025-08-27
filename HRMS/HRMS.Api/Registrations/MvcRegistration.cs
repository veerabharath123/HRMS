using HRMS.Api.Class;
using HRMS.Api.ExceptionHandling;
using HRMS.Application;
using HRMS.Infrasturcture;
using Microsoft.AspNetCore.Authorization;

namespace HRMS.Api.Registrations
{
    public class MvcRegistration : IWebApplicationBuilderRegistration
    {
        public void RegisterServices(WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.AddServerHeader = false;
            });

            builder.Services
                .AddExceptionHandler<AppExceptionHandler>()
                .AddControllers(options => options.Filters.Add<ValidationModelAttribute>())
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

            builder.Services
                .AddEndpointsApiExplorer()
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
                .AddSingleton<IAuthorizationHandler, DbPermissionHandler>();
        }

    }

}
