using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Response
{
    public class ChatUserResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
    public class ChatConversationListResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<string> Participants { get; set; } = [];
        public DateTime? LastMessageDate { get; set; }
        public int UnreadCount { get; set; }
        public string? LastMessage { get; set; }
        public int? LastMessageId { get; set; }
        public int EmployeeId { get; set; }
        public Guid? ParticipantId { get; set; }
        public bool ParticipantHasPicture { get; set; }
    }
    public class ChatConversationDetailResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public string? PresenceStatus => IsOnline ? "Online" : "Offline";
        public string? LastSeenFormatted { get; set; }
        public List<ChatConversationListResponseDto> Participants { get; set; } = [];
        public List<ChatMessageResponseDto> Messages { get; set; } = [];
    }
    public class ChatMessageResponseDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;

        public string? Content { get; set; }
        public int? ParentMessageId { get; set; }
        public string ParentMessage { get; set; } = string.Empty;
        public string ParentMessageSenderName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public string CreatedDateUtc { get; set; } = string.Empty;

        public bool IsMine { get; set; }   // For UI: left or right alignment

        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public int UnreadCount { get; set; }
        public string MessageType { get; set; } = string.Empty;
        public Guid? FileId { get; set; }
    }
    public class ChatMessagesResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string DeliveredAt { get; set; } = string.Empty;
        public bool Mine { get; set; }
        public bool Seen{ get; set; }
        public bool Delivered{ get; set; }
    }
    public class ChatConversationResponseDto
    {
        public Guid ConversationId { get; set; }
        public ChatUserResponseDto ChatUser { get; set; } = new ChatUserResponseDto();
        public List<ChatMessagesResponseDto> Messages { get; set; } = [];
    }
}
