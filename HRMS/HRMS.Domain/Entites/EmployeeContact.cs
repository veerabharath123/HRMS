using HRMS.Domain.Common;
using HRMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class EmployeeContact : AuditableWithBaseEntity<int>
    {
        public int EmployeeId { get; set; } 
        public int ContactTypeId { get; set; }
        public string ContactValue { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
