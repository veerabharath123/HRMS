using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Domain.Records
{
    public static class ChatRecords
    {
        public record PresenceRecord(
            string UserId,
            IReadOnlyCollection<string> NotifyUserIds,
            IReadOnlyCollection<int> ConversationIds
        );
    }
}
