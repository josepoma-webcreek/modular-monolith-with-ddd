# Cache Verification API Documentation

## Overview

This API provides endpoints to verify and test cache functionality with support for both InMemory and Redis caching.

## Configuration

The cache type is configured in `appsettings.json`:

```json
{
  "CacheConfiguration": {
    "CacheType": "InMemory",  // Options: "InMemory" or "Redis"
    "RedisConnectionString": "localhost:6379"
  }
}
```

### InMemory Cache
- **CacheType**: `"InMemory"`
- No additional configuration required
- Best for: Development, single-instance deployments

### Redis Cache
- **CacheType**: `"Redis"`
- **RedisConnectionString**: Connection string to your Redis server
- Best for: Production, distributed deployments, shared cache across instances

## API Endpoints

Base URL: `/api/cache`

### 1. Get Cache Information

**Endpoint**: `GET /api/cache/info`

**Description**: Returns information about the currently active cache type.

**Response**:
```json
{
  "cacheType": "InMemory",
  "message": "Currently using InMemory cache"
}
```

**Example**:
```bash
curl -X GET http://localhost:5000/api/cache/info
```

---

### 2. Set Cache Value

**Endpoint**: `POST /api/cache/set`

**Description**: Store a value in the cache with optional expiration.

**Request Body**:
```json
{
  "key": "my-key",
  "value": "my-value",
  "expirationSeconds": 300  // Optional: expires after 5 minutes
}
```

**Response**:
```json
{
  "message": "Value cached successfully",
  "key": "my-key"
}
```

**Example**:
```bash
curl -X POST http://localhost:5000/api/cache/set \
  -H "Content-Type: application/json" \
  -d '{"key":"test-key","value":"Hello Cache!","expirationSeconds":60}'
```

---

### 3. Get Cache Value

**Endpoint**: `GET /api/cache/get/{key}`

**Description**: Retrieve a value from cache by its key.

**Response** (Found):
```json
{
  "key": "my-key",
  "value": "my-value",
  "cacheType": "InMemory"
}
```

**Response** (Not Found):
```json
{
  "message": "Key not found in cache",
  "key": "my-key"
}
```

**Example**:
```bash
curl -X GET http://localhost:5000/api/cache/get/test-key
```

---

### 4. Check Key Existence

**Endpoint**: `GET /api/cache/exists/{key}`

**Description**: Check if a key exists in the cache.

**Response**:
```json
{
  "key": "my-key",
  "exists": true,
  "cacheType": "InMemory"
}
```

**Example**:
```bash
curl -X GET http://localhost:5000/api/cache/exists/test-key
```

---

### 5. Delete Cache Value

**Endpoint**: `DELETE /api/cache/delete/{key}`

**Description**: Remove a value from cache.

**Response**:
```json
{
  "message": "Value removed from cache",
  "key": "my-key"
}
```

**Example**:
```bash
curl -X DELETE http://localhost:5000/api/cache/delete/test-key
```

---

### 6. Test Cache Operations

**Endpoint**: `POST /api/cache/test`

**Description**: Comprehensive test that performs set, get, exists, and delete operations to verify cache functionality.

**Request Body**:
```json
{
  "value": "Test Value"  // Optional: defaults to "Test Value"
}
```

**Response**:
```json
{
  "cacheType": "InMemory",
  "testKey": "test_123e4567-e89b-12d3-a456-426614174000",
  "originalValue": "Test Value",
  "retrievedValue": "Test Value",
  "existedBeforeDelete": true,
  "existsAfterDelete": false,
  "testPassed": true
}
```

**Example**:
```bash
curl -X POST http://localhost:5000/api/cache/test \
  -H "Content-Type: application/json" \
  -d '{"value":"Testing Cache"}'
```

---

## Testing with Different Cache Types

### Test InMemory Cache

1. Update `appsettings.json`:
```json
{
  "CacheConfiguration": {
    "CacheType": "InMemory"
  }
}
```

2. Restart the application
3. Test using the endpoints above

### Test Redis Cache

1. Start a Redis server (using Docker):
```bash
docker run -d -p 6379:6379 redis:latest
```

2. Update `appsettings.json`:
```json
{
  "CacheConfiguration": {
    "CacheType": "Redis",
    "RedisConnectionString": "localhost:6379"
  }
}
```

3. Restart the application
4. Test using the endpoints above

---

## Quick Test Sequence

```bash
# 1. Check cache type
curl -X GET http://localhost:5000/api/cache/info

# 2. Set a value
curl -X POST http://localhost:5000/api/cache/set \
  -H "Content-Type: application/json" \
  -d '{"key":"greeting","value":"Hello World!","expirationSeconds":300}'

# 3. Get the value
curl -X GET http://localhost:5000/api/cache/get/greeting

# 4. Check existence
curl -X GET http://localhost:5000/api/cache/exists/greeting

# 5. Delete the value
curl -X DELETE http://localhost:5000/api/cache/delete/greeting

# 6. Run comprehensive test
curl -X POST http://localhost:5000/api/cache/test \
  -H "Content-Type: application/json" \
  -d '{"value":"Automated Test"}'
```

---

## Swagger UI

When the application is running, you can also access the Swagger UI to test these endpoints interactively:

```
http://localhost:5000/swagger
```

Navigate to the "Cache" section to see all available endpoints with their documentation and test them directly from the browser.

---

## Architecture Notes

- **ICacheService Interface**: Provides abstraction over different cache implementations
- **InMemoryCacheService**: Uses `Microsoft.Extensions.Caching.Memory`
- **RedisCacheService**: Uses `Microsoft.Extensions.Caching.StackExchangeRedis`
- Cache service is registered as a Singleton in the DI container
- Configuration-based selection between InMemory and Redis at startup
- All operations are async to support distributed caching scenarios
