using System;
using System.Runtime.Caching;

namespace AlexaTVInfoSkill.Service
{
    public class AppCache
    {
        public static bool Contains(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        public static object Get(string key)
        {
            return MemoryCache.Default[key];
        }

        public static T Get<T>(string key)
        {
            return (T)MemoryCache.Default[key];
        }

        public static void Add(string key, object item)
        {
            MemoryCache.Default.Add(key, item, DateTime.Now.AddSeconds(5));
        }

        public static void Add(string key, object item, DateTime expiry)
        {
            MemoryCache.Default.Add(key, item, expiry);
        }

        public static object Delete(string key)
        {
            return MemoryCache.Default.Remove(key);
        }

        public static void Clear()
        {
            foreach (var item in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(item.Key);
            }
        }
    }
}
