using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class MessageStatus : AuditableWithBaseEntity<int>
    {
        public int MessageId { get; private set; }
        public int EmployeeId { get; private set; }

        public DateTime? DeliveredAt { get; private set; }
        public DateTime? ReadAt { get; private set; }

        public Message? Message { get; set; }
        public Employee? Employee { get; set; }

        public void MarkDelivered(DateTime when)
        {
            DeliveredAt = when;
        }

        public void MarkRead(DateTime when)
        {
            ReadAt = when;
        }

        public void Assign(int messageId, int employeeId)
        {
            MessageId = messageId;
            EmployeeId = employeeId;
        }
    }
}

