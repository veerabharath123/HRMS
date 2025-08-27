namespace HRMS.WebApplication.Registrations
{
    public interface IWebApplicationBuilderRegistration : IRegistration
    {
        void RegisterServices(WebApplicationBuilder builder);
    }
}
