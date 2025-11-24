namespace CompanyName.MyMeetings.API.SignalR
{
    /// <summary>
    /// Interface for managing SignalR connections.
    /// </summary>
    public interface ISignalRConnectionManager
    {
        Task AddConnectionAsync(string userId, string connectionId);

        Task RemoveConnectionAsync(string userId, string connectionId);

        Task UpdateConnectionMetadataAsync(string userId, string connectionId, string module, string additionalData);

        Task<IEnumerable<SignalRConnectionDto>> GetUserConnectionsAsync(string userId);

        Task<IEnumerable<SignalRConnectionDto>> GetAllConnectionsAsync();

        Task<SignalRConnectionDto> GetConnectionAsync(string connectionId);
    }
}
