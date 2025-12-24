using HRMS.Domain.Records;

namespace HRMS.Application.Services.Chat.ChatPresence
{
    public interface IChatPresenceServices
    {
        Task<ChatRecords.PresenceRecord?> UserConnectedAsync(string userId);
        Task<ChatRecords.PresenceRecord?> UserDisconnectedAsync(string userId);
    }
}