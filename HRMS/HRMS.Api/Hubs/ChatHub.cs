using HRMS.Infrastructure.Sockets.ConnectionManager;

namespace HRMS.Api.Hubs
{
    public class ChatHub: BaseHub
    {
        private readonly HubsConnectionManager _manager;

        public ChatHub(HubsConnectionManager manager)
        {
            _manager = manager;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier ?? Context.ConnectionId;
            _manager.AddConnection(userId, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier ?? Context.ConnectionId;
            _manager.RemoveConnection(userId, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
