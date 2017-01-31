using System;
using System.Web;
using System.Web.Caching;

namespace AlexaTVInfoSkill.Service.Scheduler
{
    public class HttpRuntimeCacheAdapter<T> : ICache<T>
        where T : class
    {
        public void Put(string key, T value, TimeSpan duration, Action<string, T> onExpiration = null)
        {
            var expiration = SystemTime.Now + duration;

            // don't fire if item was explcitly removed or implicitly removed due to Insert updating the entry
            CacheItemRemovedCallback removeCallback = (k, v, r) =>
            {
                if (r != CacheItemRemovedReason.Removed && onExpiration != null)
                    onExpiration(k, v as T);
            };

            HttpRuntime.Cache.Insert(key, value, null, expiration, Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, removeCallback);
        }

        public T Get(string key)
        {
            return HttpRuntime.Cache[key] as T;
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
