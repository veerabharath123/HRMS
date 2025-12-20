using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

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
            
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            builder.Services.AddAuthentication("AuthCookie")
            .AddCookie("AuthCookie", options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("en");
                options.AddSupportedCultures(["en"]);
                options.AddSupportedUICultures(["en"]);

                options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider { CookieName = "UserCulture" });
            });
            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddScoped<ApiRequest>();
            builder.Services.AddHttpClient<ApiRequest>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddScoped<IBreadcrumbCache, SessionBreadcrumbCache>();
            builder.Services.AddScoped<BreadcrumbManager>();
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

            builder.Services.AddSingleton<IProxyConfigProvider, ApiProxyConfigProvider>();

            builder.Services.AddReverseProxy()
                .AddTransforms(transforms =>
                {
                    transforms.AddRequestTransform(async context =>
                    {
                        var httpContext = context.HttpContext;

                        var token = httpContext.Session.GetString("AccessToken");
                        // Inject server JWT before sending to API
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.ProxyRequest.Headers.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        }

                    });
                });  // YARP
        }

    }
}
