using System.Net;
using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.Extensions.Options;

namespace HRMS.Infrastructure.Ftp
{
    public class FtpFileServices : IFtpFileServices
    {
        private readonly FtpConfigDto _defaultConfig;

        public FtpFileServices(IOptions<FtpConfigDto> config)
        {
            _defaultConfig = config.Value;
        }

        private static bool TryParseUri(string remotePath, FtpConfigDto config, out Uri? uri)
        {
            uri = null;

            if (config is null || string.IsNullOrWhiteSpace(config.FtpBaseUrl) || string.IsNullOrWhiteSpace(remotePath))
            {
                return false;
            }

            // Make sure host does not contain scheme
            var host = config.FtpBaseUrl
                .Replace("ftp://", "", StringComparison.OrdinalIgnoreCase)
                .Replace("ftps://", "", StringComparison.OrdinalIgnoreCase)
                .TrimEnd('/');

            // Normalize path
            var cleanPath = remotePath.Replace('\\', '/').TrimStart('/');

            var builder = new UriBuilder
            {
                Scheme = config.UseSsl ? Uri.UriSchemeFtps : Uri.UriSchemeFtp,
                Host = host,
                Path = cleanPath
            };

            uri = builder.Uri;
            return true;
        }


        private static FtpWebRequest CreateRequest(string method, string remotePath, FtpConfigDto config)
        {
            if(!TryParseUri(remotePath, config, out Uri? uri) || uri is null)
                throw new ArgumentException("Invalid FTP configuration or remote path.");

            var request = (FtpWebRequest)WebRequest.Create(uri);
            
            request.Method = method;
            request.Credentials = new NetworkCredential(config.FtpUsername, config.FtpPassword);
            request.EnableSsl = config.UseSsl;
            request.UsePassive = true;
            request.UseBinary = true;

            return request;
        }

        public Task<bool> UploadAsync(string remotePath, Stream fileStream, CancellationToken cancellationToken = default)
        {
            return UploadAsync(remotePath, fileStream, _defaultConfig, cancellationToken);
        }

        public async Task<bool> UploadAsync(string remotePath, Stream fileStream, FtpConfigDto config, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequest(WebRequestMethods.Ftp.UploadFile, remotePath, config);
                using var requestStream = await request.GetRequestStreamAsync();
                await fileStream.CopyToAsync(requestStream, cancellationToken);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                return response.StatusCode is
               FtpStatusCode.FileActionOK or
               FtpStatusCode.ClosingData or
               FtpStatusCode.DataAlreadyOpen;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse response)
            {
                return false;
            }
        }

        public Task<bool> DeleteAsync(string remotePath, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(remotePath, _defaultConfig, cancellationToken);
        }

        public async Task<bool> DeleteAsync(string remotePath, FtpConfigDto config, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequest(WebRequestMethods.Ftp.DeleteFile, remotePath, config);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                return response.StatusCode == FtpStatusCode.FileActionOK || response.StatusCode == FtpStatusCode.CommandOK;
            }
            catch(WebException ex) when (ex.Response is FtpWebResponse response && (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailableOrBusy || response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable))
            {
                return true; // File does not exist, consider as deleted
            }
        }

        public Task<Stream?> FetchAsync(string remotePath, CancellationToken cancellationToken = default)
        {
            return FetchAsync(remotePath, _defaultConfig, cancellationToken);
        }

        public async Task<Stream?> FetchAsync(string remotePath, FtpConfigDto config, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequest(WebRequestMethods.Ftp.DownloadFile, remotePath, config);
                var response = (FtpWebResponse)await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    return null;
                var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch
            {
                return null;
            }
        }
    }
}
