namespace Inventory.Service
{
    public interface IRedisCacheService
    {
        bool TryGetCacheAsync<T>(string key, out T value);
        Task SetCacheAsync<T>(string key, T value);
        Task RemoveCacheAsync(string key);
        Task RemoveCacheAsync(string[] keys);
        Task RemoveCacheTreeAsync(string treeKey);
    }
}
