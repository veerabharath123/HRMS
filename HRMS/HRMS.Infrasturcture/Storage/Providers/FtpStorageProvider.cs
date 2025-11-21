using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Common.Class;
using System.Net;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HRMS.Infrastructure.Storage.Providers
{
    public class FtpStorageProvider : IFileStorageProvider
    {
        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _useSsl;
        public FtpStorageProvider(string baseUrl, string username, string password, bool useSsl)
        {
            _baseUrl = baseUrl;
            _username = username;
            _password = password;
            _useSsl = useSsl;
        }
        public FtpStorageProvider(FtpConfigDto config)
        {
            _baseUrl = config.FtpBaseUrl;
            _username = config.FtpUsername;
            _password = config.FtpPassword;
            _useSsl = config.UseSsl;
        }
        private bool TryParseUri(string remotePath, out Uri? uri)
        {
            uri = null;

            if (string.IsNullOrWhiteSpace(_baseUrl) || string.IsNullOrWhiteSpace(remotePath))
                return false;

            // Make sure host does not contain scheme
            var host = _baseUrl
                .Replace("ftp://", "", StringComparison.OrdinalIgnoreCase)
                .Replace("ftps://", "", StringComparison.OrdinalIgnoreCase)
                .TrimEnd('/');

            // Normalize path
            var cleanPath = remotePath.Replace('\\', '/').TrimStart('/');

            var builder = new UriBuilder
            {
                Scheme = _useSsl ? Uri.UriSchemeFtps : Uri.UriSchemeFtp,
                Host = host,
                Path = cleanPath
            };

            uri = builder.Uri;
            return true;
        }
        private FtpWebRequest CreateRequest(string method, string remotePath)
        {
            if (!TryParseUri(remotePath, out Uri? uri) || uri is null)
                throw new ArgumentException("Invalid FTP configuration or remote path.");

            var request = (FtpWebRequest)WebRequest.Create(uri);

            request.Method = method;
            request.Credentials = new NetworkCredential(_username, _password);
            request.EnableSsl = _useSsl;
            request.UsePassive = true;
            request.UseBinary = true;

            return request;
        }
        public async Task<bool> DeleteAsync(string filename, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequest(WebRequestMethods.Ftp.DeleteFile, filename);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                return response.StatusCode == FtpStatusCode.FileActionOK || response.StatusCode == FtpStatusCode.CommandOK;
            }
            catch (WebException ex) when (ex.Response is FtpWebResponse response && (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailableOrBusy || response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable))
            {
                return true; // File does not exist, consider as deleted
            }
        }

        public async Task<byte[]?> FetchAsync(string filename, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequest(WebRequestMethods.Ftp.DownloadFile, filename);
                var response = (FtpWebResponse)await request.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                if (responseStream == null)
                    return null;

                var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UploadAsync(string filename, Stream fileStream, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequest(WebRequestMethods.Ftp.UploadFile, filename);
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
    }
}
