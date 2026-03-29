using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    public static class BaseGameEntityTickDriver
    {
        public static bool Enabled = true;

        private static readonly List<BaseGameEntity> _entities = new List<BaseGameEntity>(4096);
        public static IReadOnlyList<BaseGameEntity> Entities => _entities;

        public static bool ShouldTickDriveOnThisRuntime()
        {
            if (!Enabled)
                return false;

            // Dedicated/headless
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

            // Prevent duplicates (O(n), but only on enable/disable, not per tick)
            for (int i = 0; i < _entities.Count; ++i)
                if (_entities[i] == ent)
                    return;

            _entities.Add(ent);
        }

        public static void Unregister(BaseGameEntity ent)
        {
            if (!ent)
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

        public static void Clear() => _entities.Clear();
    }
}
