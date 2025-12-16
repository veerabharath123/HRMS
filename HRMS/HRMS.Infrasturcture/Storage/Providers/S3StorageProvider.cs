using Amazon.S3;
using Amazon.S3.Model;
using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public S3StorageProvider(S3BucketConfigDto config)
        {
            _bucket = config.BucketName;
            _client = new AmazonS3Client(config.AccessKeyId, config.SecretKey, new AmazonS3Config { ServiceURL = config.ServiceUrl, ForcePathStyle = true, AuthenticationRegion = config.Region });
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

        public async Task<byte[]?> FetchAsync(string fileKey, CancellationToken ct = default)
        {
            try
            {
                using var res = await _client.GetObjectAsync(_bucket, fileKey, ct);
                using var ms = new MemoryStream();
                await res.ResponseStream.CopyToAsync(ms, ct);
                ms.Position = 0;
                return ms.ToArray();
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
