using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.HttpOverrides;

namespace HRMS.WebApplication.Registrations
{
    public class MvcWebAppRegistration : IWebApplicationRegistration
    {
        private readonly string _policyName = "HRMSPolicy";
        public void RegisterPipelineComponents(Microsoft.AspNetCore.Builder.WebApplication app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }
            app.UseExceptionHandler("/Home/Error");

            app.UseMiddleware<CustomSecurityHeader>();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRequestLocalization();

            app.UseRouting();


            app.UseAuthorization();

            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };

            forwardedHeadersOptions.KnownNetworks.Clear(); // Clear the default networks
            forwardedHeadersOptions.KnownProxies.Clear();  // Clear the default proxies

            app.UseForwardedHeaders(forwardedHeadersOptions);

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "/{controller=Home}/{action=Index}/{id?}");
        }
    }
}
