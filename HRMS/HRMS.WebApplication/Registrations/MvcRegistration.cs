using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Authorization;
using Yarp.ReverseProxy.Configuration;

namespace HRMS.WebApplication.Registrations
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
                //.AddExceptionHandler<AppExceptionHandler>()
                .AddControllers().AddRazorRuntimeCompilation();
            //.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

            builder.Services
                .AddEndpointsApiExplorer();
            //.AddApplication()
            //.AddInfrastructure(builder.Configuration)

            builder.Services.AddScoped<ApiRequest>();
            //builder.Services.AddSingleton<IProxyConfigProvider, ApiProxyConfigProvider>();
        }

    }
}
