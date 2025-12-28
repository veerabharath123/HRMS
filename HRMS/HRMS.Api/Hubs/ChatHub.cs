using HRMS.Application.Common.Interface;
using HRMS.Application.Services.Chat.ChatPresence;
using HRMS.Infrastructure.Communication.Messaging.ConnectionManager;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.SignalR;

namespace HRMS.Api.Hubs
{
    public class ChatHub: BaseHub
    {
        private readonly HubsConnectionManager _manager;
        private readonly IPresenceConnectionManager _presenceManager;
        private readonly IChatPresenceServices _chatPresenceServices;

        public ChatHub(HubsConnectionManager manager, IPresenceConnectionManager presenceManager, IChatPresenceServices chatPresenceServices)
        {
            _manager = manager;
            _presenceManager = presenceManager;
            _chatPresenceServices = chatPresenceServices;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier ?? Context.ConnectionId;
            _manager.AddConnection(userId, Context.ConnectionId);

            var becameOnline = _presenceManager.AddConnection(
                userId,
                Context.ConnectionId
            );

            // 2️⃣ Notify only if user truly came online
            if (becameOnline)
            {
                var record = await _chatPresenceServices.UserConnectedAsync(userId);
                if (record != null && record.NotifyUserIds.Count > 0)
                {
                    await Clients.Users(record.NotifyUserIds)
                        .SendAsync("UserOnline", ApiResponseDto.SuccessStatus(new
                        {
                            record.ConversationIds,
                            IsOnline = true,
                            LastSeenUtc = _presenceManager.GetLastSeen(userId)
                        }));
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier ?? Context.ConnectionId;
            _manager.RemoveConnection(userId, Context.ConnectionId);

            var becameOffline = await _presenceManager.RemoveConnectionAsync(
                userId,
                Context.ConnectionId
            );

            // 2️⃣ Notify only if user truly went offline
            if (becameOffline)
            {
                var record = await _chatPresenceServices.UserDisconnectedAsync(userId);
                if (record != null && record.NotifyUserIds.Count > 0)
                {
                    await Clients.Users(record.NotifyUserIds)
                        .SendAsync("UserOffline", ApiResponseDto.SuccessStatus(new
                        {
                            record.ConversationIds,
                            IsOnline = false,
                            LastSeenUtc = _presenceManager.GetLastSeen(userId)
                        }));
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
