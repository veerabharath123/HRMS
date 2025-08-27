using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Common.Interface
{
    public interface IFtpFileServices
    {
        Task<bool> UploadAsync(string remotePath, Stream fileStream, CancellationToken cancellationToken = default);
        Task<bool> UploadAsync(string remotePath, Stream fileStream, FtpConfigDto config, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(string remotePath, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string remotePath, FtpConfigDto config, CancellationToken cancellationToken = default);
        Task<Stream?> FetchAsync(string remotePath, CancellationToken cancellationToken = default);
        Task<Stream?> FetchAsync(string remotePath, FtpConfigDto config, CancellationToken cancellationToken = default);


    }
}
