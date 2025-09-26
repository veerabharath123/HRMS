using HRMS.Application.Common.Interface;
using HRMS.Infrastructure.Sockets.ConnectionManager;
using Microsoft.AspNetCore.SignalR;

namespace HRMS.Infrastructure.Sockets
{
    public class ChatHubServices<THub> : IChatServices where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;
        private readonly HubsConnectionManager _manager;
        public ChatHubServices(IHubContext<THub> hubContext, HubsConnectionManager manager)
        {
            _hubContext = hubContext;
            _manager = manager;
        }
        public Task SendMessageToUserAsync<TResponse>(string userId, TResponse response)
            => _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", response);
        public async Task SendMessageToGroup<TResponse>(string groupName, TResponse response)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", response);
        }

        // Join a group (like a chat room)
        public async Task AddUserToGroupAsync(string userId, string groupName)
        {
            var connections = _manager.GetConnections(userId);
            foreach (var conn in connections)
            {
                await _hubContext.Groups.AddToGroupAsync(conn, groupName);
            }
        }

        // Leave a group
        public async Task RemoveUserFromGroupAsync(string userId, string groupName)
        {
            // Remove live connections from SignalR group
            var connections = _manager.GetConnections(userId);
            foreach (var conn in connections)
            {
                await _hubContext.Groups.RemoveFromGroupAsync(conn, groupName);
            }
        }
    }
}
