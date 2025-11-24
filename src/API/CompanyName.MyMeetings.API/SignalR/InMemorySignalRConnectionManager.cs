using System.Collections.Concurrent;

namespace CompanyName.MyMeetings.API.SignalR
{
    /// <summary>
    /// In-memory implementation of SignalR connection manager.
    /// </summary>
    public class InMemorySignalRConnectionManager : ISignalRConnectionManager
    {
        private readonly ConcurrentDictionary<string, SignalRConnectionDto> _connections;
        private readonly ILogger<InMemorySignalRConnectionManager> _logger;

        public InMemorySignalRConnectionManager(ILogger<InMemorySignalRConnectionManager> logger)
        {
            _connections = new ConcurrentDictionary<string, SignalRConnectionDto>();
            _logger = logger;
        }

        public Task AddConnectionAsync(string userId, string connectionId)
        {
            var connection = new SignalRConnectionDto
            {
                ConnectionId = connectionId,
                UserId = userId,
                ConnectedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };

            _connections.TryAdd(connectionId, connection);

            _logger.LogInformation(
                "Connection added: ConnectionId={ConnectionId}, UserId={UserId}",
                connectionId,
                userId);

            return Task.CompletedTask;
        }

        public Task RemoveConnectionAsync(string userId, string connectionId)
        {
            _connections.TryRemove(connectionId, out _);

            _logger.LogInformation(
                "Connection removed: ConnectionId={ConnectionId}, UserId={UserId}",
                connectionId,
                userId);

            return Task.CompletedTask;
        }

        public Task UpdateConnectionMetadataAsync(
            string userId,
            string connectionId,
            string module,
            string additionalData)
        {
            if (_connections.TryGetValue(connectionId, out var connection))
            {
                connection.Module = module;
                connection.AdditionalData = additionalData;
                connection.LastActivityAt = DateTime.UtcNow;

                _logger.LogInformation(
                    "Connection metadata updated: ConnectionId={ConnectionId}, UserId={UserId}, Module={Module}",
                    connectionId,
                    userId,
                    module);
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<SignalRConnectionDto>> GetUserConnectionsAsync(string userId)
        {
            var userConnections = _connections.Values
                .Where(c => c.UserId == userId)
                .ToList();

            return Task.FromResult<IEnumerable<SignalRConnectionDto>>(userConnections);
        }

        public Task<IEnumerable<SignalRConnectionDto>> GetAllConnectionsAsync()
        {
            var allConnections = _connections.Values.ToList();
            return Task.FromResult<IEnumerable<SignalRConnectionDto>>(allConnections);
        }

        public Task<SignalRConnectionDto> GetConnectionAsync(string connectionId)
        {
            _connections.TryGetValue(connectionId, out var connection);
            return Task.FromResult(connection);
        }
    }
}
