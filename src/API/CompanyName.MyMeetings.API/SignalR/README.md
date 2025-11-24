# SignalR Hub Implementation

## Overview

This implementation provides a unified SignalR Hub for real-time communication across all modules in the application. Instead of having multiple hubs for different modules, a single `MyMeetingsHub` handles all real-time communications.

## Components

### MyMeetingsHub
- **Location**: `/hubs/myMeetings`
- **Purpose**: Single SignalR hub for all real-time communications
- **Features**:
  - Automatic connection tracking
  - User-based connection management
  - Module-specific metadata support

### ConnectionsController
- **Base Route**: `/api/signalr/connections`
- **Purpose**: REST API for querying and managing SignalR connections
- **Endpoints**:
  - `GET /api/signalr/connections` - Get all active connections
  - `GET /api/signalr/connections/user/{userId}` - Get connections for a specific user
  - `GET /api/signalr/connections/{connectionId}` - Get a specific connection
  - `GET /api/signalr/connections/my-connections` - Get current user's connections

### ISignalRConnectionManager
- **Implementation**: `InMemorySignalRConnectionManager` (singleton)
- **Purpose**: Manages connection metadata and tracking
- **Features**:
  - Thread-safe in-memory storage
  - Connection lifecycle management
  - Metadata updates

## Client Usage

### Connecting to the Hub

```javascript
// Using @microsoft/signalr client library
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://your-api-url/hubs/myMeetings", {
        accessTokenFactory: () => yourAuthToken
    })
    .withAutomaticReconnect()
    .build();

// Start connection
await connection.start();
```

### Registering Connection

```javascript
// Register connection with module information
await connection.invoke("RegisterConnection", "Meetings", JSON.stringify({
    additionalInfo: "some data"
}));

// Listen for registration confirmation
connection.on("ConnectionRegistered", (data) => {
    console.log("Connection registered:", data);
});
```

### Receiving Messages

```javascript
// Subscribe to events from the hub
connection.on("MeetingUpdated", (data) => {
    console.log("Meeting updated:", data);
});

connection.on("NotificationReceived", (data) => {
    console.log("Notification:", data);
});
```

## Server Usage

### Sending Messages from Modules

To send real-time notifications from any module, inject `IHubContext<MyMeetingsHub>`:

```csharp
using Microsoft.AspNetCore.SignalR;
using CompanyName.MyMeetings.API.SignalR;

public class SomeService
{
    private readonly IHubContext<MyMeetingsHub> _hubContext;

    public SomeService(IHubContext<MyMeetingsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUser(string userId, object message)
    {
        // Get user's connections
        var connectionManager = // resolve ISignalRConnectionManager
        var userConnections = await connectionManager.GetUserConnectionsAsync(userId);
        
        // Send to all user's connections
        foreach (var conn in userConnections)
        {
            await _hubContext.Clients.Client(conn.ConnectionId)
                .SendAsync("NotificationReceived", message);
        }
    }

    public async Task BroadcastToAll(object message)
    {
        await _hubContext.Clients.All.SendAsync("GlobalNotification", message);
    }
}
```

## Connection Tracking

The system automatically tracks:
- Connection ID
- User ID (from authentication)
- Module (when registered via `RegisterConnection`)
- Additional metadata
- Connection timestamp
- Last activity timestamp

## Architecture Benefits

1. **Single Hub**: All modules use the same hub, simplifying client-side code
2. **Centralized Management**: One place to manage all real-time connections
3. **Scalability**: Easy to add Redis backplane for multi-server scenarios
4. **Monitoring**: REST API endpoints for connection monitoring
5. **Module Flexibility**: Modules can identify themselves without requiring separate hubs

## Future Enhancements

- Persistent storage for connection history
- Redis backplane for scale-out scenarios
- Message queuing for offline users
- Connection analytics and monitoring dashboards
