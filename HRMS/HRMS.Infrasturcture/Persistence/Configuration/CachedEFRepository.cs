using HRMS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrastructure.Persistence.Configuration
{
    internal class CachedEFRepository<TEntity> : EFRepository<TEntity> where TEntity : class, ICacheableEntity
    {
        private readonly IMemoryCache _cache;
        private readonly string _cacheKey;

        public CachedEFRepository(ApplicationDbContext context, IMemoryCache cache)
            : base(context)
        {
            _cache = cache;
            _cacheKey = $"EFRepo_{typeof(TEntity).Name}";
        }

        public IEnumerable<TEntity> TableCached
        {
            get
            {
                if (!_cache.TryGetValue(_cacheKey, out List<TEntity>? cachedList))
                {
                    cachedList = Entities.AsNoTracking().ToList();
                    _cache.Set(_cacheKey, cachedList, TimeSpan.FromMinutes(60));
                }

                return cachedList ?? [];
            }
        }

        protected override void AfterEntityChanged()
        {
            // refresh cache when entity changes
            _cache.Remove(_cacheKey);
            _cache.Set(_cacheKey, Entities.AsNoTracking().ToList(), TimeSpan.FromMinutes(60));
        }
    }
}
