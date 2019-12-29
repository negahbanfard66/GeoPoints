using GP.Lib.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;

namespace GP.Lib.Common.Cache
{
    public class CacheProvider: ICacheProvider
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private readonly IMemoryCache _innerCache;

        public CacheProvider(IMemoryCache innerCache)
        {
            _innerCache = innerCache;
        }

        public T Set<T>(object key, T value)
        {
            var options = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.Normal).SetAbsoluteExpiration(TimeSpan.FromSeconds(300));
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _innerCache.Set(key, value, options);

            return value;
        }

        public bool Get<T>(object key,out T value)
        {
           return _innerCache.TryGetValue(key, out value);
        }

        public void Reset()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }
    }
}
