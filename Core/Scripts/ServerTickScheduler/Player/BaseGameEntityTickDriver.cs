using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    /// <summary>
    /// Registry for BaseGameEntity instances that should be driven by ServerTickScheduler.
    /// Intended for dedicated/headless server runtimes only.
    /// </summary>
    public static class BaseGameEntityTickDriver
    {
        public static bool Enabled = true;

        private static readonly List<BaseGameEntity> _entities = new List<BaseGameEntity>(4096);
        private static readonly HashSet<BaseGameEntity> _set = new HashSet<BaseGameEntity>();

        public static IReadOnlyList<BaseGameEntity> Entities { get { return _entities; } }
        public static int Count { get { return _entities.Count; } }

        public static bool ShouldTickDriveOnThisRuntime()
        {
            if (!Enabled)
                return false;

            if (Application.isBatchMode)
                return true;

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                return true;

#if UNITY_SERVER
            return true;
#else
            return false;
#endif
        }

        public static void Register(BaseGameEntity ent)
        {
            if (!ent)
                return;

            if (_set.Add(ent))
                _entities.Add(ent);
        }

        public static void Unregister(BaseGameEntity ent)
        {
            if (!ent)
                return;

            if (!_set.Remove(ent))
                return;

            for (int i = 0; i < _entities.Count; ++i)
            {
                if (_entities[i] == ent)
                {
                    int last = _entities.Count - 1;
                    _entities[i] = _entities[last];
                    _entities.RemoveAt(last);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes destroyed/missing entries. Useful after scene reloads or missed unregisters.
        /// </summary>
        public static void Compact()
        {
            for (int i = _entities.Count - 1; i >= 0; --i)
            {
                BaseGameEntity ent = _entities[i];
                if (!ent)
                {
                    _entities.RemoveAt(i);
                }
            }

            _set.Clear();
            for (int i = 0; i < _entities.Count; ++i)
            {
                BaseGameEntity ent = _entities[i];
                if (ent)
                    _set.Add(ent);
            }
        }

        public static void Clear()
        {
            _entities.Clear();
            _set.Clear();
        }
    }
}
