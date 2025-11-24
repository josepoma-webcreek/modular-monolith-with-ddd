using Microsoft.AspNetCore.SignalR;

namespace CompanyName.MyMeetings.API.SignalR
{
    /// <summary>
    /// Unified SignalR Hub for all real-time communications across all modules.
    /// </summary>
    public class MyMeetingsHub : Hub
    {
        private readonly ISignalRConnectionManager _connectionManager;
        private readonly ILogger<MyMeetingsHub> _logger;

        public MyMeetingsHub(
            ISignalRConnectionManager connectionManager,
            ILogger<MyMeetingsHub> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.Identity?.Name;
            var connectionId = Context.ConnectionId;

            _logger.LogInformation(
                "Client connected. ConnectionId: {ConnectionId}, UserId: {UserId}",
                connectionId,
                userId);

            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionManager.AddConnectionAsync(userId, connectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.Identity?.Name;
            var connectionId = Context.ConnectionId;

            _logger.LogInformation(
                "Client disconnected. ConnectionId: {ConnectionId}, UserId: {UserId}",
                connectionId,
                userId);

            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionManager.RemoveConnectionAsync(userId, connectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Allows clients to register their connection with additional metadata.
        /// </summary>
        /// <param name="module">The module name.</param>
        /// <param name="additionalData">Optional additional data.</param>
        /// <returns>A task representing the async operation.</returns>
        public async Task RegisterConnection(string module, string additionalData = null)
        {
            var userId = Context.User?.Identity?.Name;
            var connectionId = Context.ConnectionId;

            _logger.LogInformation(
                "Connection registration requested. ConnectionId: {ConnectionId}, UserId: {UserId}, Module: {Module}",
                connectionId,
                userId,
                module);

            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionManager.UpdateConnectionMetadataAsync(
                    userId,
                    connectionId,
                    module,
                    additionalData);

                await Clients.Caller.SendAsync("ConnectionRegistered", new
                {
                    Success = true,
                    ConnectionId = connectionId,
                    Module = module
                });
            }
        }
    }
}
