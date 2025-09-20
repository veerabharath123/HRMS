using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HRMS.Api.Hubs
{
    [Authorize]
    public class NotificationHub : BaseHub {

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected: {Context.ConnectionId}, UserIdentifier: {Context.UserIdentifier}");
            return base.OnConnectedAsync();
        }
    }
}
