namespace Juno.ForeignExchange.DataAccess
{
    using Microsoft.Extensions.Caching.Memory;
    using System;
    using System.Threading.Tasks;
    using Domain.Service;

    public class CacheManager : ICacheManager
    {
        private static IMemoryCache Cache { get; set; }
        private TimeSpan LifeTime { get; }

        static CacheManager()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        public CacheManager() : this(TimeSpan.FromHours(1)) { }
        public CacheManager(TimeSpan lifeTime) { this.LifeTime = lifeTime; }

        public T AddOrUpdate<T>(string key, T value)
        {
            return AddOrUpdate(key, value, this.LifeTime);
        }

        public T AddOrUpdate<T>(string key, T value, TimeSpan lifeTime)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(lifeTime);

            return Cache.Set(key, value, cacheEntryOptions);
        }

        public void Clear()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        public T GetAndRemove<T>(string key)
        {
            var item = this.Get<T>(key);

            this.Remove(key);

            return item;
        }

        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> handler)
        {
            return GetOrCreateAsync(key, handler, this.LifeTime);
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> handler, TimeSpan lifeTime)
        {
            var item = this.Get<T>(key);

            if (item != null)
            {
                return item;
            }

            item = await handler();

            return AddOrUpdate(key, item, lifeTime);
        }
    }
}
