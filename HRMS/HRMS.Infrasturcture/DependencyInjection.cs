using AspNetCore.ReCaptcha;
using HRMS.Application.Common.Interface;
using HRMS.Infrastructure.Ftp;
using HRMS.Infrasturcture.FileLogging;
using HRMS.Infrasturcture.Jwt;
using HRMS.Infrasturcture.Persistence;
using HRMS.Infrasturcture.Persistence.Configuration;
using HRMS.Infrasturcture.Recaptcha;
using HRMS.Infrasturcture.DocumentGenerator;
using HRMS.SharedKernel.Attributes;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HRMS.Infrasturcture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)), ServiceLifetime.Scoped);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddMemoryCache();
            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<IJwtTokenServices, JwtTokenServices>()
                .AddScoped<IFtpFileServices, FtpFileServices>()
                .AddScoped<ICaptchaServices, GoogleRecaptchaServices>()
                .AddScoped<IDocumentGenerator, DocumentGenerator.DocumentGenerator>()
                .AddAppConfigs(configuration)
                .AddRecaptcha(configuration);
        
        }
        private static IServiceCollection AddAppConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            string suffix = "ConfigDto";
            var assembly = typeof(AppSettingConfigAttribute).Assembly;

            var configTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<AppSettingConfigAttribute>() is not null);

            foreach (var type in configTypes)
            {
                var sectionName = type.Name;

                if (sectionName.EndsWith(suffix))
                    sectionName = sectionName[..^suffix.Length];

                // Register with Configure<T>
                var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .First(m => m.Name == "Configure" && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 2);

                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(null, [services, configuration.GetSection(sectionName)]);
            }

            return services;
        }
        private static IServiceCollection AddRecaptcha(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ReCaptcha").Get<ReCaptchaConfigDto>()
                        ?? throw new NullReferenceException("ReCaptcha configuration section is missing or not properly configured.");

            services.AddReCaptcha(options =>
            {
                options.SiteKey = config.SiteKey;
                options.SecretKey = config.SecretKey;
                options.Version = Enum.TryParse(config.Version, true, out ReCaptchaVersion parsedVersion)
                                    ? parsedVersion
                                    : ReCaptchaVersion.V2; ;
                options.UseRecaptchaNet = config.UseRecaptchaNet;
            });

            return services;
        }
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            return builder;
        }
        public static IServiceCollection AddHubs(this IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                var className = type.Name.Replace("Hub", "HubServices");

                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                var serviceType = currentAssembly.GetTypes()
                    .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.Name.Split('`')[0] == className && t.IsGenericType);

                if (serviceType == null) continue;

                var interfaceType = serviceType.GetInterfaces().FirstOrDefault(t => t.Name.EndsWith("Services"));

                if (interfaceType == null) continue;

                services.AddScoped(interfaceType, serviceType.MakeGenericType(type));
            }

            return services;
        }
    }
}
