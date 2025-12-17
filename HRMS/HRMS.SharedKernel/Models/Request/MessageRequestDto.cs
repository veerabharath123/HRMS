using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Request
{
    public class MessageRequestDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public DateTime Date { get; set; } 
    }
    public class ChatMessageRequestDto
    {
        [Required]
        public int ConversationId { get; set; }
        [Required]
        public string Content { get; set; } =string.Empty;
        public int? ParentMessageId { get; set; }
        public Guid? FileId { get; set; }
        [Required]
        public string MessageType { get; set; } = "Text";
    }
}