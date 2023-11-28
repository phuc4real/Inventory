using Inventory.Core.Constants;
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
        private readonly TimeSpan expireTime = TimeSpan.FromMinutes(5);

        public RedisCacheService(IConnectionMultiplexer conn)
        {
            _conn = conn;
        }

        #endregion

        #region Private

        private IDatabase RedisDb => _conn.GetDatabase();

        #endregion

        #region Method

        public async Task RemoveCacheAsync(string key)
        {
            await RedisDb.KeyDeleteAsync(key);
        }

        public async Task RemoveCacheAsync(string[] keys)
        {
            foreach (var key in keys)
            {
                await RedisDb.KeyDeleteAsync(key);
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

            await RedisDb.StringSetAsync(key, bytes, expireTime);
        }

        public bool TryGetCacheAsync<T>(string key, out T value)
        {
            var storedCache = RedisDb.StringGetAsync(key).Result;

            value = default!;

            if (storedCache.HasValue)
            {
                value = JsonSerializer.Deserialize<T>(storedCache)!;
                return true;
            }

            return false;
        }


        public async Task RemoveCacheByListIdAsync(IEnumerable<int> idList)
        {
            foreach (var id in idList)
            {
                await RemoveCacheAsync(CacheNameConstant.Item + id);
                await RemoveCacheAsync(CacheNameConstant.ItemCompact + id);
            }

            await RemoveCacheTreeAsync(CacheNameConstant.ItemPagination);
        }

        #endregion
    }
}
