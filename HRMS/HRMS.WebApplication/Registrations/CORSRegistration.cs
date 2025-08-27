namespace HRMS.WebApplication.Registrations
{
    public class CORSRegistration : IWebApplicationBuilderRegistration
    {
        private readonly string _policyName = "HRMSPolicy";

        public void RegisterServices(WebApplicationBuilder builder)
        {
            var hosts = builder.Configuration.GetValue<string>("AllowedHosts");

            if (string.IsNullOrEmpty(hosts)) return;

            builder.Services.AddCors(options =>
            {
                var origins = hosts.Split(',');
                options.AddPolicy(_policyName, p =>
                {
                    p.WithOrigins(origins).AllowAnyHeader().WithMethods("POST","GET");
                });
            });
        }
    }
}
