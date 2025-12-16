using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Common.Interface
{
    public interface IFileStorageProvider
    {
        Task<bool> UploadAsync(string filename, Stream fileStream, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string filename, CancellationToken cancellationToken = default);
        Task<byte[]?> FetchAsync(string filename, CancellationToken cancellationToken = default);
    }
}
