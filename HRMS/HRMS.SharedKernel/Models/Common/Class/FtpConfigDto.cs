using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.SharedKernel.Attributes;

namespace HRMS.SharedKernel.Models.Common.Class
{
    [AppSettingConfig]
    public class FtpConfigDto
    {
        public string FtpBaseUrl { get; set; } = string.Empty;
        public string FtpUsername { get; set; } = string.Empty;
        public string FtpPassword { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = false;
    }
}
