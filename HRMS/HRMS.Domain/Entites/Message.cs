using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class Message : AuditableWithBaseEntity<int>
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }

        public int? ParentMessageId { get; set; }

        public string? Content { get; set; }
        public int MessageTypeId { get; set; }     // FK to GeneralReference

        public bool IsEdited { get; set; }

        // Navigation
        public Conversation? Conversation { get; set; }
        public Employee? Sender { get; set; }

        public Message? ParentMessage { get; set; }
        public ICollection<Message> Replies { get; set; } = [];

        public ICollection<MessageStatus> MessageStatuses { get; set; } = [];
        public ICollection<Attachment> Attachments { get; set; } = [];

        public void Add(int conversationId, int senderId, string? content, int messageTypeId, int? parentMessageId = null)
        {
            ConversationId = conversationId;
            SenderId = senderId;
            Content = content;
            MessageTypeId = messageTypeId;
            ParentMessageId = parentMessageId;
            IsEdited = false;
        }

        public void Edit(string? newContent)
        {
            Content = newContent;
            IsEdited = true;
        }
    }
}
