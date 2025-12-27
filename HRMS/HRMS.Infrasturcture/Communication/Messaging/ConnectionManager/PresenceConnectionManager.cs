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
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();
        private readonly ConcurrentDictionary<string, DateTimeOffset> _lastSeen = new();
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _offlineTimers = new();

        private static readonly TimeSpan OfflineDelay = TimeSpan.FromSeconds(10);

        public bool AddConnection(string userId, string connectionId)
        {
            if (_offlineTimers.TryRemove(userId, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }

            var connections = _connections.GetOrAdd(userId, _ => new HashSet<string>());

            lock (connections)
            {
                var wasOffline = connections.Count == 0;
                connections.Add(connectionId);
                return wasOffline;
            }
        }


        public async Task<bool> RemoveConnectionAsync(string userId, string connectionId)
        {
            if (!_connections.TryGetValue(userId, out var connections))
                return false;

            lock (connections)
            {
                connections.Remove(connectionId);

                // Still has active connections → not offline
                if (connections.Count > 0)
                    return false;
            }

            // 🔹 Create / replace offline timer
            var cts = new CancellationTokenSource();
            _offlineTimers.AddOrUpdate(userId, cts, (_, oldCts) =>
            {
                oldCts.Cancel();
                oldCts.Dispose();
                return cts;
            });

            try
            {
                await Task.Delay(OfflineDelay, cts.Token);

                // After delay, ensure user didn't reconnect
                if (_connections.TryRemove(userId, out _))
                {
                    _lastSeen[userId] = DateTimeOffset.UtcNow;
                    return true; // ✅ truly offline
                }

                return false;
            }
            catch (TaskCanceledException)
            {
                // 🔹 Reconnected before delay elapsed
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
