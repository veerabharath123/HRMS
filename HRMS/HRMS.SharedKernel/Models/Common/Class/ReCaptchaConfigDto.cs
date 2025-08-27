using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.SharedKernel.Attributes;

namespace HRMS.SharedKernel.Models.Common.Class
{
    [AppSettingConfig]
    public class ReCaptchaConfigDto
    {
        public string SecretKey { get; set; } = string.Empty;
        public string SiteKey { get; set; } = string.Empty;
        public bool UseRecaptchaNet { get; set; }
        public string Version { get; set; } = string.Empty;
    }
}
