using CompanyName.MyMeetings.API.Configuration.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyName.MyMeetings.API.SignalR
{
    /// <summary>
    /// Controller for managing SignalR connections.
    /// </summary>
    [Route("api/signalr/[controller]")]
    [ApiController]
    public class ConnectionsController : ControllerBase
    {
        private readonly ISignalRConnectionManager _connectionManager;
        private readonly ILogger<ConnectionsController> _logger;

        public ConnectionsController(
            ISignalRConnectionManager connectionManager,
            ILogger<ConnectionsController> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        /// <summary>
        /// Get all active SignalR connections.
        /// </summary>
        /// <returns>List of all active connections.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<SignalRConnectionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllConnections()
        {
            var connections = await _connectionManager.GetAllConnectionsAsync();
            return Ok(connections);
        }

        /// <summary>
        /// Get SignalR connections for a specific user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>List of user's active connections.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<SignalRConnectionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserConnections(string userId)
        {
            var connections = await _connectionManager.GetUserConnectionsAsync(userId);
            return Ok(connections);
        }

        /// <summary>
        /// Get a specific SignalR connection by connection ID.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns>The connection details or 404 if not found.</returns>
        [HttpGet("{connectionId}")]
        [ProducesResponseType(typeof(SignalRConnectionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConnection(string connectionId)
        {
            var connection = await _connectionManager.GetConnectionAsync(connectionId);

            if (connection == null)
            {
                return NotFound();
            }

            return Ok(connection);
        }

        /// <summary>
        /// Get current user's SignalR connections.
        /// </summary>
        /// <returns>List of current user's active connections.</returns>
        [HttpGet("my-connections")]
        [ProducesResponseType(typeof(IEnumerable<SignalRConnectionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyConnections()
        {
            var userId = UserIdentityHelper.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new List<SignalRConnectionDto>());
            }

            var connections = await _connectionManager.GetUserConnectionsAsync(userId);
            return Ok(connections);
        }
    }
}
