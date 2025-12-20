using Asp.Versioning.ApiExplorer;
using HRMS.Api.Hubs;
using Microsoft.AspNetCore.HttpOverrides;

namespace HRMS.Api.Registrations
{
    public class MvcWebAppRegistration : IWebApplicationRegistration
    {
        private readonly string _policyName = "HRMSPolicy";
        public void RegisterPipelineComponents(WebApplication app)
        {
            if(!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            })
                .UseHttpsRedirection()
                .UseRequestLocalization()
                .UseExceptionHandler(_ => { })
                .UseCors(_policyName)
                .UseAuthentication()
                .UseAuthorization();

            app.MapControllers();

            RegisterHubPipelines(app);

        }
        private void RegisterHubPipelines(WebApplication app)
        {
            app.MapHub<NotificationHub>("/hubs/notification")
                .RequireCors(_policyName);

            app.MapHub<ChatHub>("/hubs/chat")
                .RequireCors(_policyName);
        }
    }
}
