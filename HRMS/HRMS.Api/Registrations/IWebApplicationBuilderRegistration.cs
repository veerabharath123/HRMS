namespace HRMS.Api.Registrations
{
    public interface IWebApplicationBuilderRegistration : IRegistration
    {
        void RegisterServices(WebApplicationBuilder builder);
    }
}
