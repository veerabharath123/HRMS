using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;

namespace HRMS.Application.Services
{
    public interface IUserServices
    {
        Task<ApiResponseDto<bool>> InsertUserAsync(UserInsertRequestDto request);
        Task<List<string>> GetPermissionsByUserIdAsync(Guid Id);

        Task<ApiResponseDto<LoginResponseDto>> ValidateUserLoginAsync(LoginRequestDto request);
        Task<ApiResponseDto<FileResponseDto>> GetDocument();
    }
}