using Inventory.Services.IServices;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;

namespace Inventory.Services.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConnectionMultiplexer _conn;

        public RedisCacheService(IConnectionMultiplexer conn)
        {
            _conn = conn;
        }

        public async Task RemoveCacheAsync(string key)
        {
            var redis = _conn.GetDatabase();
            await redis.KeyDeleteAsync(key);
        }

        public async Task RemoveCacheAsync(string[] keys)
        {
            var redis = _conn.GetDatabase();
            
            foreach (var key in keys)
            {
                await redis.KeyDeleteAsync(key);
            }
        }

        public async Task RemoveCacheTreeAsync(string treeKey)
        {
            var endPoints = _conn.GetEndPoints();
            var server = _conn.GetServer(endPoints[0]);

            foreach (var key in server.Keys(pattern: treeKey + "*")){
                await RemoveCacheAsync(key);
            }
        }

        public async Task SetCacheAsync<T>(string key, T value)
        {
            var redis = _conn.GetDatabase();

            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

            await redis.StringSetAsync(key, bytes);
            await redis.KeyExpireAsync(key, TimeSpan.FromMinutes(5));
        }

        public bool TryGetCacheAsync<T>(string key, out T value)
        {
            var redis = _conn.GetDatabase();

            var storedCache = redis.StringGetAsync(key).Result;

            if (!storedCache.HasValue) 
            {
                value = default!;
                return false;
            }

            value = JsonSerializer.Deserialize<T>(storedCache)!;
            return true;
        }
    }
}
