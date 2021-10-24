using Microsoft.Extensions.Caching.Memory;
using SSock.Core.Abstract.Infrastructure.Session;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SSock.Core.Infrastructure.Session
{
    internal class Cache
        : ICache
    {
        private MemoryCache _cache;

        private ConcurrentDictionary<object, SemaphoreSlim> _locks;

        public Cache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _locks = new ConcurrentDictionary<object, SemaphoreSlim>();
        }
        
        public async Task<T> GetOrCreateAsync<T>(object key, T value)
        {
            T cacheEntry;

            if (!_cache.TryGetValue(key, out cacheEntry))
            {
                var locker = _locks.GetOrAdd(key, v => new SemaphoreSlim(1, 1));

                await locker.WaitAsync();
                try
                {
                    if (!_cache.TryGetValue(key, out cacheEntry))
                    {
                        cacheEntry = value;
                        _cache.Set(key, cacheEntry);
                    }
                }
                finally
                {
                    locker.Release();
                }
            }

            return cacheEntry;
        }
    }
}
