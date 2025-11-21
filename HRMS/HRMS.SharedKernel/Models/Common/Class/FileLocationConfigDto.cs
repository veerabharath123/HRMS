using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.SharedKernel.Models.Common.Class
{
    public class FileLocationConfigDto
    {
        public string ConfigName { get; set; } = string.Empty;
        public string ConfigJson { get; set; } = string.Empty;
        public string ProviderType { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
