using HRMS.Application.Common.Interface;
using Microsoft.EntityFrameworkCore;
using static HRMS.Domain.Records.ChatRecords;

namespace HRMS.Application.Services.Chat.ChatPresence
{
    public sealed class ChatPresenceServices : IChatPresenceServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatPresenceServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private async Task<PresenceRecord?> GetPresenceForUserAsync(string userId)
        {
            if (!int.TryParse(userId, out int id))
                return null;

            var user = await _unitOfWork.UserRepo
                .TableNoTracking
                .Where(u => u.Id == id && !u.IsDeleted)
                .Select(u => new { u.Id, u.EmployeeId })
                .FirstOrDefaultAsync();

            if (user?.EmployeeId == null)
                return null;

            var conversations = await _unitOfWork.ConversationsRepo
                .TableNoTracking
                .Where(c =>
                    !c.IsDeleted &&
                    c.Participants.Any(p => p.EmployeeId == user.EmployeeId))
                .Select(c => new
                {
                    c.Id,
                    Participants = c.Participants
                        .Where(p => p.EmployeeId != user.EmployeeId)
                        .Select(p => p.EmployeeId)
                })
                .ToListAsync();

            if (conversations.Count == decimal.Zero) return null;

            var conversationIds = conversations.Select(c => c.Id).ToList();

            var participantEmployeeIds = conversations.SelectMany(c => c.Participants).Distinct().ToList();

            if (participantEmployeeIds.Count == decimal.Zero) return null;

            var notifyUsers = await _unitOfWork.UserRepo
                .TableNoTracking
                .Where(u =>
                    u.EmployeeId != null &&
                    participantEmployeeIds.Contains(u.EmployeeId.Value) &&
                    !u.IsDeleted)
                .Select(u => u.Id.ToString())
                .ToListAsync();

            return new PresenceRecord(userId, notifyUsers, conversationIds);
        }

        public Task<PresenceRecord?> UserConnectedAsync(string userId) => GetPresenceForUserAsync(userId);

        public Task<PresenceRecord?> UserDisconnectedAsync(string userId) => GetPresenceForUserAsync(userId);
    }
}
