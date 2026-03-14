using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseCacheData<T>
    {
        public float TouchedTime { get; private set; }

        public virtual BaseCacheData<T> Prepare(in T source)
        {
            TouchedTime = Time.unscaledTime;
            return this;
        }

        public abstract void Clear();
    }
}