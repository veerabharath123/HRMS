using HRMS.SharedKernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    [AppSettingConfig]
    public class FileLoggerConfigDto
    {
        public string FileBasePath { get; set; } = "log\\";
        public int MaxRetries { get; set; } = 3;
        public int RetentionDays { get; set; } = 7;
        public int RetryDelayMilliseconds { get; set; } = 100;
    }
}
