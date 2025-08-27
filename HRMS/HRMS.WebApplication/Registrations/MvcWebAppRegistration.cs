using Microsoft.AspNetCore.HttpOverrides;

namespace HRMS.WebApplication.Registrations
{
    public class MvcWebAppRegistration : IWebApplicationRegistration
    {
        private readonly string _policyName = "HRMSPolicy";
        public void RegisterPipelineComponents(Microsoft.AspNetCore.Builder.WebApplication app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            })
            .UseHttpsRedirection()
            .UseRequestLocalization()
            //.UseExceptionHandler(_ => { })
            .UseCors(_policyName)
            .UseAuthentication()
            .UseAuthorization();

            app.MapControllers();
        }
    }
}
