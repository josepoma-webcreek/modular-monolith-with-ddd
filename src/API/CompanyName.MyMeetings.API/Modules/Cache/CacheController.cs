using CompanyName.MyMeetings.BuildingBlocks.Infrastructure.Caching;
using Microsoft.AspNetCore.Mvc;

namespace CompanyName.MyMeetings.API.Modules.Cache
{
    [Route("api/cache")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get cache type information (InMemory or Redis).
        /// </summary>
        /// <returns>Cache information including type.</returns>
        [HttpGet("info")]
        [ProducesResponseType(typeof(CacheInfoResponse), StatusCodes.Status200OK)]
        public IActionResult GetCacheInfo()
        {
            var cacheType = _cacheService.GetCacheType();
            return Ok(new CacheInfoResponse
            {
                CacheType = cacheType,
                Message = $"Currently using {cacheType} cache"
            });
        }

        /// <summary>
        /// Set a value in cache with optional expiration.
        /// </summary>
        /// <param name="request">Cache set request containing key, value and optional expiration.</param>
        /// <returns>Confirmation message.</returns>
        [HttpPost("set")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetCache([FromBody] SetCacheRequest request)
        {
            TimeSpan? expiration = request.ExpirationSeconds.HasValue
                ? TimeSpan.FromSeconds(request.ExpirationSeconds.Value)
                : null;

            await _cacheService.SetAsync(request.Key, request.Value, expiration);

            return Ok(new { message = "Value cached successfully", key = request.Key });
        }

        /// <summary>
        /// Get a value from cache by key.
        /// </summary>
        /// <param name="key">The cache key to retrieve.</param>
        /// <returns>The cached value if found.</returns>
        [HttpGet("get/{key}")]
        [ProducesResponseType(typeof(GetCacheResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCache(string key)
        {
            var value = await _cacheService.GetAsync<string>(key);

            if (value == null)
            {
                return NotFound(new { message = "Key not found in cache", key });
            }

            return Ok(new GetCacheResponse
            {
                Key = key,
                Value = value,
                CacheType = _cacheService.GetCacheType()
            });
        }

        /// <summary>
        /// Check if a key exists in cache.
        /// </summary>
        /// <param name="key">The cache key to check.</param>
        /// <returns>Existence status of the key.</returns>
        [HttpGet("exists/{key}")]
        [ProducesResponseType(typeof(ExistsCacheResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExistsCache(string key)
        {
            var exists = await _cacheService.ExistsAsync(key);

            return Ok(new ExistsCacheResponse
            {
                Key = key,
                Exists = exists,
                CacheType = _cacheService.GetCacheType()
            });
        }

        /// <summary>
        /// Delete a value from cache by key.
        /// </summary>
        /// <param name="key">The cache key to delete.</param>
        /// <returns>Confirmation message.</returns>
        [HttpDelete("delete/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCache(string key)
        {
            await _cacheService.RemoveAsync(key);

            return Ok(new { message = "Value removed from cache", key });
        }

        /// <summary>
        /// Test cache by setting, getting, and verifying a value.
        /// </summary>
        /// <param name="request">Test request containing optional test value.</param>
        /// <returns>Test results including all operations performed.</returns>
        [HttpPost("test")]
        [ProducesResponseType(typeof(TestCacheResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> TestCache([FromBody] TestCacheRequest request)
        {
            var testKey = $"test_{Guid.NewGuid()}";
            var testValue = request.Value ?? "Test Value";

            // Set the value
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromMinutes(5));

            // Get the value
            var retrievedValue = await _cacheService.GetAsync<string>(testKey);

            // Check existence
            var exists = await _cacheService.ExistsAsync(testKey);

            // Clean up
            await _cacheService.RemoveAsync(testKey);

            // Verify removal
            var existsAfterDelete = await _cacheService.ExistsAsync(testKey);

            return Ok(new TestCacheResponse
            {
                CacheType = _cacheService.GetCacheType(),
                TestKey = testKey,
                OriginalValue = testValue,
                RetrievedValue = retrievedValue,
                ExistedBeforeDelete = exists,
                ExistsAfterDelete = existsAfterDelete,
                TestPassed = retrievedValue == testValue && exists && !existsAfterDelete
            });
        }
    }

    public class CacheInfoResponse
    {
        public string CacheType { get; set; }

        public string Message { get; set; }
    }

    public class SetCacheRequest
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public int? ExpirationSeconds { get; set; }
    }

    public class GetCacheResponse
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string CacheType { get; set; }
    }

    public class ExistsCacheResponse
    {
        public string Key { get; set; }

        public bool Exists { get; set; }

        public string CacheType { get; set; }
    }

    public class TestCacheRequest
    {
        public string Value { get; set; }
    }

    public class TestCacheResponse
    {
        public string CacheType { get; set; }

        public string TestKey { get; set; }

        public string OriginalValue { get; set; }

        public string RetrievedValue { get; set; }

        public bool ExistedBeforeDelete { get; set; }

        public bool ExistsAfterDelete { get; set; }

        public bool TestPassed { get; set; }
    }
}
