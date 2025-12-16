using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;

namespace HRMS.Application.Services.Chat
{
    public interface IChatServices
    {
        Task<ApiResponseDto> GetChatConversationListAsync();
        Task<ApiResponseDto> GetChatConversationSearchedListAsync(AdvanceTableRequestDto? request = null);
        Task<ApiResponseDto> StartNewChatWithAsync(int chatWithEmployeeId);
        Task<ApiResponseDto> GetChatConversationDetailsAsync(int conversationId);
        Task<ApiResponseDto> SendMessageAsync(ChatMessageRequestDto request);
        Task<ApiResponseDto> MarkMessageAsDeliveredAsync(int messageId);
        Task<ApiResponseDto> MarkMessageAsReadAsync(UpdateSeenRequestDto request);
        Task<ApiResponseDto> SendTypingStatusAsync(TypingRequestDto request);
        Task<ApiResponseDto> GetPreviousMessagesAsync(NextMessagesRequestDto request);
    }
}