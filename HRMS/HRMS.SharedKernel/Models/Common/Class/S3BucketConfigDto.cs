using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class S3BucketConfigDto
    {
        public string BucketName { get; set; } = string.Empty;
        public string ServiceUrl { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string AccessKeyId { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }
}
