using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductManagement.Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ProductManagement.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _database = redis.GetDatabase(); 
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                _logger.LogInformation("Getting cache data for key: {Key}", key);

                var cachedValue = await _database.StringGetAsync(key);

                if (!cachedValue.HasValue)
                {
                    _logger.LogInformation("Cache miss for key: {Key}", key);
                    return null; 
                }

                _logger.LogInformation("Cache hit for key: {Key}", key);

                return JsonSerializer.Deserialize<T>(cachedValue!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache data for key: {Key}", key);
                return null; 
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                _logger.LogInformation("Setting cache data for key: {Key}", key);

                var serializedValue = JsonSerializer.Serialize(value);

                await _database.StringSetAsync(key, serializedValue, expiration);

                _logger.LogInformation("Cache data set successfully for key: {Key} with expiration: {Expiration}",
                    key, expiration?.ToString() ?? "Never");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache data for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _logger.LogInformation("Removing cache data for key: {Key}", key);

                await _database.KeyDeleteAsync(key);

                _logger.LogInformation("Cache data removed successfully for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache data for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                _logger.LogInformation("Removing cache data for pattern: {Pattern}", pattern);

                var endpoints = _redis.GetEndPoints();
                var server = _redis.GetServer(endpoints.First());

                var keys = server.Keys(pattern: pattern).ToArray();

                if (keys.Length > 0)
                {
                    await _database.KeyDeleteAsync(keys);
                    _logger.LogInformation("Removed {Count} cache entries for pattern: {Pattern}",
                        keys.Length, pattern);
                }
                else
                {
                    _logger.LogInformation("No cache entries found for pattern: {Pattern}", pattern);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache data for pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if cache key exists: {Key}", key);
                return false;
            }
        }
    }
}
