using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Assessment.Services;

public class RedisService
{
    // Lazy connection so app can start without Redis; if connect fails, caching is skipped with a warning.
    private readonly Lazy<IConnectionMultiplexer?> _connectionFactory;
    private readonly ILogger<RedisService> _logger;

    public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
    {
        _logger = logger;
        var redisConnection = configuration.GetValue<string>("Redis:ConnectionString");

        _connectionFactory = new Lazy<IConnectionMultiplexer?>(() =>
        {
            if (string.IsNullOrWhiteSpace(redisConnection))
            {
                _logger.LogWarning("Redis connection string not configured; caching disabled.");
                return null;
            }

            try
            {
                var options = ConfigurationOptions.Parse(redisConnection);
                options.AbortOnConnectFail = false; // do not block startup if Redis is down
                return ConnectionMultiplexer.Connect(options);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable; caching disabled.");
                return null;
            }
        });
    }

    private IConnectionMultiplexer? Connection => _connectionFactory.Value;

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var redis = Connection;
        if (redis == null || !redis.IsConnected) return;

        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, expiration);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var redis = Connection;
        if (redis == null || !redis.IsConnected) return default;

        var db = redis.GetDatabase();
        var json = await db.StringGetAsync(key);

        if (json.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json.ToString());
    }

    public async Task RemoveAsync(string key)
    {
        var redis = Connection;
        if (redis == null || !redis.IsConnected) return;

        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(key);
    }
}
