using HRMS.Api.Hubs;
using HRMS.Infrastructure;


namespace HRMS.Api.Registrations
{
    public class HubsRegistration:IWebApplicationBuilderRegistration
    {
        public void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddSignalR();

            var hubTypes = typeof(BaseHub).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseHub)))
                .ToArray();

            builder.Services.AddHubs(hubTypes);
        }
    }
}
