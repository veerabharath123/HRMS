using HRMS.Application.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Storage.Providers
{
    public class LocalStorageProvider : ILocalStorageProvider
    {
        public LocalStorageProvider(dynamic config)
        {
            // Configuration can be used if needed
        }
        public Task<bool> DeleteAsync(string filename, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<byte[]?> FetchAsync(string filename, CancellationToken cancellationToken = default)
        {
            try
            {
                if (File.Exists(filename))
                {
                    return await File.ReadAllBytesAsync(filename, cancellationToken);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool FileExists(string path) => File.Exists(path);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public async Task<bool> UploadAsync(string filename, Stream fileStream, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var localFileStream = new FileStream(
                    filename,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    options: FileOptions.Asynchronous);

                await fileStream.CopyToAsync(localFileStream, cancellationToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
