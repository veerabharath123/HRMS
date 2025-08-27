using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.SharedKernel.Attributes;

namespace HRMS.SharedKernel.Models.Common.Class
{
    [AppSettingConfig]
    public class JwtAuthConfigDto
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string EncryptKey { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
}
