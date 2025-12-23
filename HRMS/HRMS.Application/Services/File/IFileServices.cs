using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;

namespace HRMS.Application.Services.File
{
    public interface IFileServices
    {
        Task<ApiResponseDto> ProcessFileMaintenanceAsync(CancellationToken cancellationToken = default);
        Task<ApiResponseDto> UploadFileAsync(FileRequestDto request);
        Task<string> GetFileByStoredFileIdAsync(int storedFileId);
        Task<FileResponseDto> GetFileBytesByStoredFileIdAsync(int storedFileId, bool thumb = false);
    }
}
