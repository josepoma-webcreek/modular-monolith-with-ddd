# Cache Verification API - Quick Start

## Overview
This implementation adds a configurable caching system with support for both InMemory and Redis caching, along with a REST API to verify and test cache functionality.

## Quick Start

### 1. Using InMemory Cache (Default)
The application is pre-configured to use InMemory cache. Simply start the application:

```bash
cd src/API/CompanyName.MyMeetings.API
dotnet run
```

### 2. Test the Cache API
```bash
# Check cache type
curl -X GET http://localhost:5000/api/cache/info

# Run comprehensive test
curl -X POST http://localhost:5000/api/cache/test \
  -H "Content-Type: application/json" \
  -d '{"value":"Hello Cache!"}'
```

### 3. Switch to Redis Cache

**Start Redis (using Docker):**
```bash
docker run -d -p 6379:6379 redis:latest
```

**Update Configuration:**
Edit `appsettings.json`:
```json
{
  "CacheConfiguration": {
    "CacheType": "Redis",
    "RedisConnectionString": "localhost:6379"
  }
}
```

**Restart the application** and test again.

## API Endpoints

- `GET /api/cache/info` - Get cache type information
- `POST /api/cache/set` - Set a value in cache
- `GET /api/cache/get/{key}` - Get a value from cache
- `GET /api/cache/exists/{key}` - Check if key exists
- `DELETE /api/cache/delete/{key}` - Remove value from cache
- `POST /api/cache/test` - Run comprehensive cache test

## Documentation
For detailed API documentation, examples, and advanced usage, see [CACHE_API.md](CACHE_API.md).

## Architecture
- **ICacheService**: Abstraction for cache operations
- **InMemoryCacheService**: Implementation using Microsoft.Extensions.Caching.Memory
- **RedisCacheService**: Implementation using Microsoft.Extensions.Caching.StackExchangeRedis
- **CacheController**: REST API for cache verification and testing

## Configuration Options
| Setting | Values | Description |
|---------|--------|-------------|
| `CacheType` | `InMemory` or `Redis` | Type of cache to use |
| `RedisConnectionString` | Connection string | Required only for Redis |

## Use Cases
- **Development**: Use InMemory cache for simplicity
- **Production (Single Instance)**: Use InMemory cache for performance
- **Production (Multiple Instances)**: Use Redis for shared cache across instances
- **Testing**: Use the `/api/cache/test` endpoint to verify cache functionality
