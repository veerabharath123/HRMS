using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HRMS.Domain.Entites
{
    public class Department : AuditableWithBaseEntity<int>, ICacheableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}