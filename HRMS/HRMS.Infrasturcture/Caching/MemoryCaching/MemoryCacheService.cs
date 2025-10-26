using HRMS.Application.Common.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Caching.MemoryCaching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public T GetOrCreate<T>(
            object key,
            Func<T> factory,
            TimeSpan? absoluteExpiration = null)
        {
            if (_cache.TryGetValue(key, out T? value) && value is not null)
                return value;

            value = factory();
            _cache.Set(key, value, absoluteExpiration ?? TimeSpan.FromHours(1));
            return value;
        }
        public async Task<T> GetOrCreateAsync<T>(
            object key,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpiration = null)
        {
            if (_cache.TryGetValue(key, out T? value) && value is not null)
                return value;

            var semaphore = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                if (_cache.TryGetValue(key, out value) && value is not null)
                    return value;

                value = await factory();
                _cache.Set(key, value, absoluteExpiration ?? TimeSpan.FromHours(1));
                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Remove(string key) => _cache.Remove(key);
    }
}
