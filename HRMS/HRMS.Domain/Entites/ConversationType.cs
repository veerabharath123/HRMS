using HRMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Entites
{
    public class ConversationType : AuditableWithBaseEntity<int>
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        public ICollection<Conversation> Conversations { get; set; } = [];

        public void Add(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        public void Update(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
