using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Common.Interface
{
    public interface IEmailServices
    {
        Task SendMailAsync(EmailMessageDto message);
        Task SendMailAsync(EmailMessageDto request, EmailConfigDto config);

    }
}
