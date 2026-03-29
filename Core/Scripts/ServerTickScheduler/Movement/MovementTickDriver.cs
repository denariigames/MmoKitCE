using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    public static class MovementTickDriver
    {
        public static bool Enabled = true;

        private static readonly List<NavMeshEntityMovement> _navMesh = new List<NavMeshEntityMovement>(4096);
        private static readonly List<SimpleNavMeshEntityMovement> _simple = new List<SimpleNavMeshEntityMovement>(4096);

        public static IReadOnlyList<NavMeshEntityMovement> NavMesh => _navMesh;
        public static IReadOnlyList<SimpleNavMeshEntityMovement> Simple => _simple;

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

        public static void Register(NavMeshEntityMovement comp)
        {
            if (!comp) return;
            for (int i = 0; i < _navMesh.Count; ++i) if (_navMesh[i] == comp) return;
            _navMesh.Add(comp);
        }

        public static void Unregister(NavMeshEntityMovement comp)
        {
            if (!comp) return;
            for (int i = 0; i < _navMesh.Count; ++i)
            {
                if (_navMesh[i] == comp)
                {
                    int last = _navMesh.Count - 1;
                    _navMesh[i] = _navMesh[last];
                    _navMesh.RemoveAt(last);
                    return;
                }
            }
        }

        public static void Register(SimpleNavMeshEntityMovement comp)
        {
            if (!comp) return;
            for (int i = 0; i < _simple.Count; ++i) if (_simple[i] == comp) return;
            _simple.Add(comp);
        }

        public static void Unregister(SimpleNavMeshEntityMovement comp)
        {
            if (!comp) return;
            for (int i = 0; i < _simple.Count; ++i)
            {
                if (_simple[i] == comp)
                {
                    int last = _simple.Count - 1;
                    _simple[i] = _simple[last];
                    _simple.RemoveAt(last);
                    return;
                }
            }
        }

        public static void Clear()
        {
            _navMesh.Clear();
            _simple.Clear();
        }
    }
}
