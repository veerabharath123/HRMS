using Microsoft.AspNetCore.HttpOverrides;

namespace HRMS.Api.Registrations
{
    public class MvcWebAppRegistration : IWebApplicationRegistration
    {
        private readonly string _policyName = "ExpenseTrackerPolicy";
        public void RegisterPipelineComponents(WebApplication app)
        {
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
        }
    }
}
