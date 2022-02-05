using System;
using Microsoft.Extensions.Caching.Memory;

namespace SevenAssignmentLibrary.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cacheOptions = new MemoryCacheEntryOptions();
        }

        public void Set<T>(string key, T obj, int mins)
        {
            _cacheOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(mins);
            _memoryCache.Set(key, obj, _cacheOptions);
        }

        public T Get<T>(string key)
        {
            var inCache = _memoryCache.TryGetValue(key, out T cachedValue);

            return inCache ? cachedValue : default;
        }
    }
}
