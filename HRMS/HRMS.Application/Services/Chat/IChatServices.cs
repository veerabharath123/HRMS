using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;

namespace HRMS.Application.Services.Chat
{
    public interface IChatServices
    {
        Task<ApiResponseDto> GetChatConversationListAsync();
        Task<ApiResponseDto> StartNewChatWithAsync(int chatWithEmployeeId);
        Task<ApiResponseDto> GetChatConversationDetailsAsync(int conversationId);
        Task<ApiResponseDto> SendMessageAsync(ChatMessageRequestDto request);
    }
}