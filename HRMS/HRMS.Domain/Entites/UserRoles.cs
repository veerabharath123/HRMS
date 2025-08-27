using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.Domain.Common;

namespace HRMS.Domain.Entites
{
    public class UserRoles : AuditableWithBaseEntity<int>
    {
        public int UserId { get; private set; }
        public int RoleId { get; private set; }
        public bool IsActive { get; private set; }
    }
}
