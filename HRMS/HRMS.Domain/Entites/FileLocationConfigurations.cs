using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class FileLocationConfigurations : AuditableWithBaseEntity<int>
    {
        public string ConfigName { get; set; } = string.Empty;
        public string ConfigJson { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
