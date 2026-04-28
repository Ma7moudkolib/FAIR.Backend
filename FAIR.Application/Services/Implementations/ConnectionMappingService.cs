using FAIR.Application.Services.Interfaces;
using System.Collections.Concurrent;

namespace FAIR.Application.Services.Implementations
{
    public class ConnectionMappingService : IConnectionMappingService
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connectionsByUser = new();
        private readonly ConcurrentDictionary<string, string> _userByConnection = new();

        public void AddOrUpdate(string userId, string connectionId)
        {
            _connectionsByUser.AddOrUpdate(
                userId,
                _ => new HashSet<string> { connectionId },
                (_, existing) =>
                {
                    lock (existing)
                    {
                        existing.Add(connectionId);
                    }

                    return existing;
                });

            _userByConnection[connectionId] = userId;
        }

        public void Remove(string connectionId)
        {
            if (!_userByConnection.TryRemove(connectionId, out var userId))
            {
                return;
            }

            if (!_connectionsByUser.TryGetValue(userId, out var connections))
            {
                return;
            }

            lock (connections)
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _connectionsByUser.TryRemove(userId, out _);
                }
            }
        }

        public string? GetConnectionId(string userId)
        {
            if (!_connectionsByUser.TryGetValue(userId, out var connections))
            {
                return null;
            }

            lock (connections)
            {
                return connections.FirstOrDefault();
            }
        }

        public IReadOnlyCollection<string> GetConnections(string userId)
        {
            if (!_connectionsByUser.TryGetValue(userId, out var connections))
            {
                return [];
            }

            lock (connections)
            {
                return connections.ToList();
            }
        }
    }
}
