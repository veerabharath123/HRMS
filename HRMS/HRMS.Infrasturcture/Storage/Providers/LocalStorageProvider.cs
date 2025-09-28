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
            if(File.Exists(filename))
            {
                File.Delete(filename);
                return Task.FromResult(true);
            }

            return Task.FromResult(true);
        }

        public Task<Stream?> FetchAsync(string filename, CancellationToken cancellationToken = default)
        {
            if(File.Exists(filename)) {
                using Stream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                return Task.FromResult<Stream?>(fileStream);
            }
            return Task.FromResult<Stream?>(null);
        }

        public bool FileExists(string path) => File.Exists(path);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public Task<bool> UploadAsync(string filename, Stream fileStream, CancellationToken cancellationToken = default)
        {
            try
            {
                using var localFileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                fileStream.CopyTo(localFileStream);
                return Task.FromResult(true);
            }
            catch(Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                return Task.FromResult(false);
            }
        }
    }
}
