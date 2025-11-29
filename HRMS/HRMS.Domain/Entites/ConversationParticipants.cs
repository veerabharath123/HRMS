using HRMS.Domain.Common;
using HRMS.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class ConversationParticipants:AuditableWithBaseEntity<int>
    {
        public int ConversationId { get; set; }
        public int EmployeeId { get; set; }

        public Conversation? Conversation { get; set; }
        public Employee? Employee { get; set; }

    }
}