
using HRMS.Api.Options;
using Microsoft.OpenApi.Models;

namespace HRMS.Api.Registrations
{
    public class SwaggerRegistration : IWebApplicationBuilderRegistration
    {
        public void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen();
            builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
        }
    }
}
