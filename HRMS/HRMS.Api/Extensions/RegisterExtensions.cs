using System.Runtime.CompilerServices;
using HRMS.Api.Registrations;

namespace HRMS.Api.Extensions
{
    public static class RegisterExtensions
    {
        private static IEnumerable<T> GetRegistrations<T>(Type scanningType) where T : IRegistration
        {
            return scanningType.Assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(T)) && !t.IsAbstract && !t.IsInterface)
                .Select(Activator.CreateInstance)
                .Cast<T>();
        }

        public static void RegisterPipelineComponents(this WebApplication app, Type scanningType)
        {
            var registrations = GetRegistrations<IWebApplicationRegistration>(scanningType);
            foreach (var registration in registrations)
            {
                registration.RegisterPipelineComponents(app);
            }
        }
        public static void RegisterServices(this WebApplicationBuilder builder, Type scanningType)
        {
            var registrations = GetRegistrations<IWebApplicationBuilderRegistration>(scanningType);
            foreach (var registration in registrations)
            {
                registration.RegisterServices(builder);
            }
        }
    }

}
