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
        public int ConversationId { get; private set; }
        public int EmployeeId { get; private set; }

        public Conversation? Conversation { get; private set; }
        public Employee? Employee { get; private set; }

        public void AddConversation(int conversationId, int employeeId)
        {
            ConversationId = conversationId;
            EmployeeId = employeeId;
        }

    }
}