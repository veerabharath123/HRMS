using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;

namespace HRMS.Application.Services
{
    public interface IUserServices
    {
        Task<ApiResponseDto> InsertUserAsync(UserInsertRequestDto request);
        Task<List<string>> GetPermissionsByUserIdAsync(int Id);

        Task<ApiResponseDto> ValidateUserLoginAsync(LoginRequestDto request);
        Task<ApiResponseDto> GetDocument();
        Task<ApiResponseDto> UploadImage(FileRequestDto request);
        Task<ApiResponseDto> SignUpUserAsync(UserInsertRequestDto request);
        Task<ApiResponseDto> SendMessageByUser(MessageRequestDto request);

        Task<ApiResponseDto> GetUsersAsync();
    }
}