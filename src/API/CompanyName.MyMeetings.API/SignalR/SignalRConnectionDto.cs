namespace CompanyName.MyMeetings.API.SignalR
{
    /// <summary>
    /// Data Transfer Object for SignalR connection information.
    /// </summary>
    public class SignalRConnectionDto
    {
        public string ConnectionId { get; set; }

        public string UserId { get; set; }

        public string Module { get; set; }

        public string AdditionalData { get; set; }

        public DateTime ConnectedAt { get; set; }

        public DateTime? LastActivityAt { get; set; }
    }
}
