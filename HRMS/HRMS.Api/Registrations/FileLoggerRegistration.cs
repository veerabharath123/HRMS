using HRMS.Infrastructure;

namespace HRMS.Api.Registrations
{
    public class FileLoggerRegistration:IWebApplicationBuilderRegistration
    {
        public void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Logging.AddFileLogger(builder.Configuration);
        }
    }
}
