using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class ConversationMember : AuditableWithBaseEntity<int>
    {
        public int ConversationId { get; private set; }
        public int EmployeeId { get; private set; }

        public void Add(int conversationId, int employeeId)
        {
            ConversationId = conversationId;
            EmployeeId = employeeId;
        }
    }
}
