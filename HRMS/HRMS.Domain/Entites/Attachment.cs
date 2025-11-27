using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class Attachment : AuditableWithBaseEntity<int>
    {
        public int MessageId { get; private set; }
        public int FileId { get; private set; }

        public void Attach(int messageId, int fileId)
        {
            MessageId = messageId;
            FileId = fileId;
        }
    }
}
