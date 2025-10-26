using HRMS.Domain.Common;
using HRMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HRMS.Domain.Entites
{
    public class EmployeeGuardian : AuditableWithBaseEntity<int>
    {
        public int EmployeeId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public int RelationshipId { get; set; }
        public string? Phone { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
    }
}