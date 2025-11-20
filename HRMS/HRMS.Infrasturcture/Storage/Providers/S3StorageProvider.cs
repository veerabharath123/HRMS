using Amazon.S3;
using Amazon.S3.Model;
using HRMS.Application.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Storage.Providers
{
    public class S3StorageProvider:IFileStorageProvider
    {
        private readonly string _bucket;
        private readonly IAmazonS3 _client;

        public S3StorageProvider(string bucket, string key, string secret, string serviceUrl)
        {
            _bucket = bucket;
            _client = new AmazonS3Client(key, secret, new AmazonS3Config { ServiceURL = serviceUrl, ForcePathStyle = true, AuthenticationRegion = "us-east-005" });
        }

        public async Task<bool> UploadAsync(string fileKey, Stream fileStream, CancellationToken ct = default)
        {
            try
            {
                var req = new PutObjectRequest { BucketName = _bucket, Key = fileKey, InputStream = fileStream };
                var res = await _client.PutObjectAsync(req, ct);
                return res.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch { return false; }
        }

        public async Task<Stream?> FetchAsync(string fileKey, CancellationToken ct = default)
        {
            try
            {
                var res = await _client.GetObjectAsync(_bucket, fileKey, ct);
                var ms = new MemoryStream();
                await res.ResponseStream.CopyToAsync(ms, ct);
                ms.Position = 0;
                return ms;
            }
            catch { return null; }
        }

        public async Task<bool> DeleteAsync(string fileKey, CancellationToken ct = default)
        {
            try
            {
                var res = await _client.DeleteObjectAsync(_bucket, fileKey, ct);
                return res.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch { return true; }
        }
    }
}
