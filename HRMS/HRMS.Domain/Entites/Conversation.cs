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
        public string? Name { get; private set; }

        // SQL column is named "Type" - map to a clearer property name while keeping column name
        [Column("Type")]
        public int ConversationTypeId { get; private set; }

        // navigation (optional)
        public ConversationType? ConversationType { get; private set; }

        public ICollection<Message>? Messages { get; private set; }
        public ICollection<ConversationMember>? Members { get; private set; }

        public void Add(string? name, int conversationTypeId)
        {
            Name = name;
            ConversationTypeId = conversationTypeId;
        }

        public void Update(string? name, int conversationTypeId)
        {
            Name = name;
            ConversationTypeId = conversationTypeId;
        }
    }
}
