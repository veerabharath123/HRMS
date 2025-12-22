using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Constants
{
    public static class ChatConstants
    {
        public const string SYSTEM_PROMPT = "You are a helpful HR assistant. Provide accurate and concise information based on the user's queries related to HR policies, procedures, and general inquiries. Always maintain a professional and courteous tone.";
        public const string CREATE_CONVO_FAILED_MSG = "Failed to create Conversation.";
        public const string FETCH_CONVO_FAILED_MSG = "Enable to fetch chat conversation, please try again later.";
        public const string START_NEW_CONVO = "Start a new conversation.";
        public const string FETCH_CHATS_FAILED_MSG = "Failed to fetch chat messages.";
        public const string SEND_MSG_FAILED_MSG = "Failed to send message, please try again later.";
        public const string SEND_MSG_SUCCESS_MSG = "Message sent successfully.";
        public const string NO_UNDELIVERED_MSG = "No undelivered messages.";
        public const string NO_UNREAD_MSG = "No unread messages.";
        public const string MARK_DELIVERED_SUCCESS_MSG = "Messages marked as delivered.";
        public const string MARK_READ_FAILED_MSG = "Failed to mark messages as read.";
        public const string MARK_READ_SUCCESS_MSG = "Messages marked as read.";
        public const string UPDATE_READ_FAILED_MSG = "Failed to update read status.";
        public const string USER_NOT_PART_OF_CONVO = "User not part of conversation.";
        public const string CHAT_USER_UNKNOWN = "Unknown";
        public const string ATT_NOT_FOUND_MSG = "Attachment not found.";


        public const int AMOUNT_OF_MSGS_PER_REQ = 20;


        public struct CONVERSATION_TYPE
        {
            public const string DIRECT = "DIRECT";
            public const string GROUP = "GROUP";            
        }
        public struct ATTACHMENT_TYPE
        {
            public const string TEXT = "TEXT";
            public const string FILE = "FILE";            
        }
    }
}
