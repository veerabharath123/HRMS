using HRMS.Application.Common.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Communication.Messaging.ConnectionManager
{
    public sealed class PresenceConnectionManager : IPresenceConnectionManager
    {
        // userId -> connectionIds
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

        // userId -> last seen timestamp
        private readonly ConcurrentDictionary<string, DateTimeOffset> _lastSeen = new();

        public bool AddConnection(string userId, string connectionId)
        {
            var connections = _connections.GetOrAdd(userId, _ => new HashSet<string>());

            lock (connections)
            {
                var wasOffline = connections.Count == 0;
                connections.Add(connectionId);

                // user is online now → remove last-seen
                if (wasOffline)
                    _lastSeen.TryRemove(userId, out _);

                return wasOffline;
            }
        }

        public bool RemoveConnection(string userId, string connectionId)
        {
            if (!_connections.TryGetValue(userId, out var connections))
                return false;

            lock (connections)
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    _connections.TryRemove(userId, out _);

                    // 🔹 last tab closed → mark last seen
                    _lastSeen[userId] = DateTimeOffset.UtcNow;
                    return true;
                }

                return false;
            }
        }

        public bool IsOnline(string userId)
            => !string.IsNullOrEmpty(userId) && _connections.ContainsKey(userId);

        public IReadOnlyCollection<string> GetConnections(string userId)
            => _connections.TryGetValue(userId, out var connections)
                ? connections.ToArray()
                : Array.Empty<string>();

        public IReadOnlyCollection<string> GetOnlineUsers()
            => _connections.Keys.ToArray();

        public DateTimeOffset? GetLastSeen(string userId)
            => _lastSeen.TryGetValue(userId, out var ts) ? ts : null;
    }

}
