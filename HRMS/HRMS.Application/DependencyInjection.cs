using System.Reflection;
using HRMS.Application.Common.Class;
using HRMS.Domain.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace HRMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //_ = services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));

            DependencyInjectionTypeAttribute? dependencyInjectionTypeAttribute = new(GeneralConstants.DependencyInjectionTypes.Scoped);
            var assembly = Assembly.GetExecutingAssembly();

            var serviceTypes = assembly.GetTypes()
           .Where(t => t.IsClass && t.Name.EndsWith("Services"))
           .ToList();


            foreach (var serviceType in serviceTypes)
            {
                object[] attributes = serviceType.GetCustomAttributes(typeof(DependencyInjectionTypeAttribute), false);
                dependencyInjectionTypeAttribute = attributes.Length > 0 ? (DependencyInjectionTypeAttribute)attributes[0] : new(GeneralConstants.DependencyInjectionTypes.Scoped);

                var interfaceType = serviceType.GetInterfaces().FirstOrDefault();

                switch (dependencyInjectionTypeAttribute.DependencyTypeName)
                {
                    case GeneralConstants.DependencyInjectionTypes.Scoped:
                        AddScoped1(services, interfaceType, serviceType);
                        break;
                    case GeneralConstants.DependencyInjectionTypes.Singleton:
                        AddSingleton1(services, interfaceType, serviceType);
                        break;
                    case GeneralConstants.DependencyInjectionTypes.Transient:
                        AddTransient1(services, interfaceType, serviceType);
                        break;
                    default:
                        AddScoped1(services, interfaceType, serviceType);
                        break;
                }
            }

            return services;
        }

        private static void AddScoped1(this IServiceCollection services, Type? interfaceType, Type serviceType)
        {
            if (interfaceType != null)
                services.AddScoped(interfaceType, serviceType);
            else
                services.AddScoped(serviceType);
        }

        private static void AddSingleton1(this IServiceCollection services, Type? interfaceType, Type serviceType)
        {
            if (interfaceType != null)
                services.AddSingleton(interfaceType, serviceType);
            else
                services.AddSingleton(serviceType);
        }

        private static void AddTransient1(this IServiceCollection services, Type? interfaceType, Type serviceType)
        {
            if (interfaceType != null)
                services.AddTransient(interfaceType, serviceType);
            else
                services.AddTransient(serviceType);
        }
    }
}

