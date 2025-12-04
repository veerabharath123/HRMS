using Amazon.Runtime;
using Azure;
using HRMS.Application.Common.Interface;
using HRMS.Domain.Entites;
using HRMS.Infrastructure.Communication.Messaging.ConnectionManager;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.SignalR;

namespace HRMS.Infrastructure.Communication.Messaging
{
    public class ChatHubServices<THub> : IChatNotificationServices where THub : Hub
    {
        private readonly IHubContext<THub> _hubContext;
        private readonly HubsConnectionManager _manager;
        public ChatHubServices(IHubContext<THub> hubContext, HubsConnectionManager manager)
        {
            _hubContext = hubContext;
            _manager = manager;
        }
        public Task SendMessageToUserAsync<TResponse>(string userId, TResponse response)
            => _hubContext.Clients.User(userId).SendAsync("ReceiveChatMessage", ApiResponseDto.SuccessStatus(response));
        public Task SendDeliveredStatusToUserAsync<TResponse>(string userId, TResponse response)
            => _hubContext.Clients.User(userId).SendAsync("ReceiveDeliveredStatus", ApiResponseDto.SuccessStatus(response));
        public Task SendSeenStatusToUserAsync<TResponse>(string userId, TResponse response)
            => SendStatusToUserAsync("ReceiveSeenStatus", userId, response);
        private Task SendStatusToUserAsync<TResponse>(string statusType,string userId, TResponse response)
            => _hubContext.Clients.User(userId).SendAsync(statusType, ApiResponseDto.SuccessStatus(response));

        public Task SendTypingToUserStatus<TResponse>(string userId, TResponse response)
            => SendStatusToUserAsync("ReceiveTypingStatus", userId, response);

        public async Task SendMessageToGroup<TResponse>(string groupName, TResponse response)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveGroupMessage", response);
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
