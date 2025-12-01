using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface IChatNotificationServices
    {
        Task SendMessageToUserAsync<TResponse>(string userId, TResponse response);
        Task SendMessageToGroup<TResponse>(string groupName, TResponse response);
        Task AddUserToGroupAsync(string userId, string groupName);
        Task RemoveUserFromGroupAsync(string userId, string groupName);
        Task SendDeliveredStatusToUserAsync<TResponse>(string userId, TResponse response);

    }
}
