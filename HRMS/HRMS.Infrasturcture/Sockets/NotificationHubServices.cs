using HRMS.Application.Common.Interface;
using Microsoft.AspNetCore.SignalR;

namespace HRMS.Infrastructure.Sockets
{
    public class NotificationHubServices<T> : ISystemNotificationServices where T : Hub
    {
        private readonly IHubContext<T> _hubContext;

        public NotificationHubServices(IHubContext<T> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendNotificationAsync(string userId, string title, string body)
            => _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", title, body);
    }
}
