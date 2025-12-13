using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class ModuleType: AuditableWithBaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool HasChildren { get; set; } 
        public int ListOrder { get; set; }
        public int? ParentModuleId { get; set; }
    }
}
