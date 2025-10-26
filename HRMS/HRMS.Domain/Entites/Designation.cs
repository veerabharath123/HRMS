using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HRMS.Domain.Entites
{
    public class Designation : AuditableWithBaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}