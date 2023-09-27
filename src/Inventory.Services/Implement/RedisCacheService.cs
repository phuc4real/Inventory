using Inventory.Service;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;

namespace Inventory.Service.Implement
{
    public class RedisCacheService : IRedisCacheService
    {
        #region Ctor & Field

        private readonly IConnectionMultiplexer _conn;

        public RedisCacheService(IConnectionMultiplexer conn)
        {
            _conn = conn;
        }

        #endregion

        #region Private

        private IDatabase redisDb => _conn.GetDatabase();

        #endregion

        #region Method

        public async Task RemoveCacheAsync(string key)
        {
            await redisDb.KeyDeleteAsync(key);
        }

        public async Task RemoveCacheAsync(string[] keys)
        {
            foreach (var key in keys)
            {
                await redisDb.KeyDeleteAsync(key);
            }
        }

        public async Task RemoveCacheTreeAsync(string treeKey)
        {
            var endPoints = _conn.GetEndPoints();
            var server = _conn.GetServer(endPoints[0]);

            foreach (var key in server.Keys(pattern: treeKey + "*"))
            {
                await RemoveCacheAsync(key);
            }
        }

        public async Task SetCacheAsync<T>(string key, T value)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

            await redisDb.StringSetAsync(key, bytes, TimeSpan.FromMinutes(10));
        }

        public bool TryGetCacheAsync<T>(string key, out T value)
        {
            var storedCache = redisDb.StringGetAsync(key).Result;

            value = default!;

            if (storedCache.HasValue)
            {
                value = JsonSerializer.Deserialize<T>(storedCache)!;
                return true;
            }

            return false;
        }

        #endregion
    }
}
