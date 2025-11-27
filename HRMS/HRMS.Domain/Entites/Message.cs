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
        public int ConversationId { get; private set; }
        public int SenderId { get; private set; }
        public int? ParentMessageId { get; private set; }

        public string? Content { get; private set; }

        // references GeneralReference for message type (e.g., text, image)
        public int MessageTypeId { get; private set; }

        public bool IsEdited { get; private set; }

        // navigation
        public Message? ParentMessage { get; private set; }
        public ICollection<Message>? Replies { get; private set; }
        public ICollection<Attachment>? Attachments { get; private set; }
        public ICollection<MessageStatus>? Statuses { get; private set; }

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
