using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Common.Interface
{
    public interface IPresenceConnectionManager
    {
        public bool AddConnection(string userId, string connectionId);
        public Task<bool> RemoveConnectionAsync(string userId, string connectionId);
        public bool IsOnline(string userId);
        public IReadOnlyCollection<string> GetConnections(string userId);
        public IReadOnlyCollection<string> GetOnlineUsers();
        public DateTimeOffset? GetLastSeen(string userId);

    }
}
