using System.Collections.Concurrent;
using ConfigurationProvider.Interface;

namespace ConfigurationProvider
{
    public class CacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<string, Config> _cache;

        public CacheProvider()
        {
            _cache = new ConcurrentDictionary<string, Config>();
        }

        public Config Get(string key)
        {
            return _cache.ContainsKey(key) ? _cache[key] : null;
        }

        public void Set(string key, Config value)
        {
            _cache[key] = value;
        }

        public void Invalidate(string key)
        {
            _cache[key] = null;
        }
    }
}