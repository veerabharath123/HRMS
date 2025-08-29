using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
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

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            builder.Services.AddAuthentication("AuthCookie")
            .AddCookie("AuthCookie", options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.LoginPath = "/Home/Login";
                options.AccessDeniedPath = "/Maintenance/NotAllowed";
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("en");
                options.AddSupportedCultures(["en"]);
                options.AddSupportedUICultures(["en"]);

                options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider { CookieName = "UserCulture" });
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.Always;
            });
            builder.Services.AddAntiforgery(options =>
            {
                options.FormFieldName = "AntiforgeryField";
                options.HeaderName = "X-CSRF-TOKEN";
            });
        }

    }
}
