namespace HRMS.WebApplication.Registrations
{
    public interface IWebApplicationRegistration : IRegistration
    {
        public void RegisterPipelineComponents(Microsoft.AspNetCore.Builder.WebApplication app);
    }
}
