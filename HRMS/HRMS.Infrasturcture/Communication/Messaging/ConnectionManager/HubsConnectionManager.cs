using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Communication.Messaging.ConnectionManager
{
    public class HubsConnectionManager
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

        public void AddConnection(string userId, string connectionId)
        {
            var connections = _connections.GetOrAdd(userId, _ => []);
            lock (connections) { connections.Add(connectionId); }
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                lock (connections) { connections.Remove(connectionId); }
            }
        }

        public IEnumerable<string> GetConnections(string userId)
            => _connections.TryGetValue(userId, out var connections) ? connections : Enumerable.Empty<string>();
    }
}
