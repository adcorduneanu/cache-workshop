namespace Juno.ForeignExchange.Domain.Service
{
    using System;
    using System.Threading.Tasks;

    public interface ICacheManager
    {
        T AddOrUpdate<T>(string key, T value);
        T AddOrUpdate<T>(string key, T value, TimeSpan lifeTime);
        T Get<T>(string key);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> handler);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> handler, TimeSpan lifeTime);
        T GetAndRemove<T>(string key);
        void Clear();
        void Remove(string key);
    }
}