using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class Conversation : AuditableWithBaseEntity<int>
    {
        public string? Name { get; set; }
        public int Type { get; set; }

        // navigation (optional)
        public ConversationType? TypeNavigation { get; set; }

        public ICollection<ConversationParticipants> Participants { get; set; } = [];
        public ICollection<Message> Messages { get; set; } = [];

    }
}
