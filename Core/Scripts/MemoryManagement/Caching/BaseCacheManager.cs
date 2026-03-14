using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseCacheManager<T, TCache>
        where TCache : BaseCacheData<T>, new()
    {
        public float cacheLifeTime = 30f;

        public abstract ProfilerMarker ProfilerMarker { get; }
        protected Dictionary<string, TCache> _caches = new Dictionary<string, TCache>();

        public void OnUpdate()
        {
            if (_caches.Count <= 0)
                return;
            using (ProfilerMarker.Auto())
            {
                float time = Time.unscaledTime;
                List<string> keys = new List<string>(_caches.Keys);
                foreach (string key in keys)
                {
                    if (time - _caches[key].TouchedTime < cacheLifeTime)
                        continue;
                    _caches[key].Clear();
                    _caches.Remove(key);
                }
            }
        }

        public void Clear()
        {
            foreach (TCache cache in _caches.Values)
            {
                cache?.Clear();
            }
            _caches.Clear();
        }

        public TCache GetOrMakeCache(string id, in T data)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            if (_caches.TryGetValue(id, out TCache cacheData))
                return cacheData.Prepare(in data) as TCache;
            cacheData = new TCache().Prepare(in data) as TCache;
            _caches[id] = cacheData;
            return cacheData;
        }
    }
}