namespace Inventory.Service
{
    public interface IRedisCacheService
    {
        public bool TryGetCacheAsync<T>(string key, out T value);
        public Task SetCacheAsync<T>(string key, T value);
        public Task RemoveCacheAsync(string key);
        public Task RemoveCacheAsync(string[] keys);
        public Task RemoveCacheTreeAsync(string treeKey);
    }
}
