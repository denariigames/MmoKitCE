using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    /// <summary>
    /// Server-side registry for movement components that are driven by the server scheduler
    /// instead of their normal per-frame MonoBehaviour path.
    /// </summary>
    public static class MovementTickDriver
    {
        public static bool Enabled = true;

        private static readonly List<NavMeshEntityMovement> _navMesh = new List<NavMeshEntityMovement>(4096);
        private static readonly HashSet<NavMeshEntityMovement> _navMeshSet = new HashSet<NavMeshEntityMovement>();

        private static readonly List<SimpleNavMeshEntityMovement> _simple = new List<SimpleNavMeshEntityMovement>(4096);
        private static readonly HashSet<SimpleNavMeshEntityMovement> _simpleSet = new HashSet<SimpleNavMeshEntityMovement>();

        public static IReadOnlyList<NavMeshEntityMovement> NavMesh => _navMesh;
        public static IReadOnlyList<SimpleNavMeshEntityMovement> Simple => _simple;

        public static int NavMeshCount => _navMesh.Count;
        public static int SimpleCount => _simple.Count;

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
            if (!comp)
                return;
            if (_navMeshSet.Add(comp))
                _navMesh.Add(comp);
        }

        public static void Unregister(NavMeshEntityMovement comp)
        {
            if (!comp)
                return;
            if (!_navMeshSet.Remove(comp))
                return;

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
            if (!comp)
                return;
            if (_simpleSet.Add(comp))
                _simple.Add(comp);
        }

        public static void Unregister(SimpleNavMeshEntityMovement comp)
        {
            if (!comp)
                return;
            if (!_simpleSet.Remove(comp))
                return;

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

        /// <summary>
        /// Removes destroyed/null Unity objects that failed to unregister. Intended for shutdown,
        /// scene transitions, and occasional diagnostics cleanup, not per tick.
        /// </summary>
        public static void Compact()
        {
            for (int i = _navMesh.Count - 1; i >= 0; --i)
            {
                NavMeshEntityMovement comp = _navMesh[i];
                if (comp)
                    continue;
                _navMesh.RemoveAt(i);
            }

            _navMeshSet.Clear();
            for (int i = 0; i < _navMesh.Count; ++i)
            {
                NavMeshEntityMovement comp = _navMesh[i];
                if (comp)
                    _navMeshSet.Add(comp);
            }

            for (int i = _simple.Count - 1; i >= 0; --i)
            {
                SimpleNavMeshEntityMovement comp = _simple[i];
                if (comp)
                    continue;
                _simple.RemoveAt(i);
            }

            _simpleSet.Clear();
            for (int i = 0; i < _simple.Count; ++i)
            {
                SimpleNavMeshEntityMovement comp = _simple[i];
                if (comp)
                    _simpleSet.Add(comp);
            }
        }

        public static void Clear()
        {
            _navMesh.Clear();
            _navMeshSet.Clear();
            _simple.Clear();
            _simpleSet.Clear();
        }
    }
}
