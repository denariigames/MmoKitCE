using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    /// <summary>
    /// Registry for CharacterRecoveryComponent instances that should be driven by ServerTickScheduler.
    /// </summary>
    public static class CharacterRecoveryTickDriver
    {
        public static bool Enabled = true;

        private static readonly List<CharacterRecoveryComponent> _components = new List<CharacterRecoveryComponent>(1024);
        public static IReadOnlyList<CharacterRecoveryComponent> Components => _components;

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

        public static void Register(CharacterRecoveryComponent comp)
        {
            if (!comp)
                return;

            for (int i = 0; i < _components.Count; ++i)
            {
                if (_components[i] == comp)
                    return;
            }
            _components.Add(comp);
        }

        public static void Unregister(CharacterRecoveryComponent comp)
        {
            if (!comp)
                return;

            for (int i = 0; i < _components.Count; ++i)
            {
                if (_components[i] == comp)
                {
                    int last = _components.Count - 1;
                    _components[i] = _components[last];
                    _components.RemoveAt(last);
                    return;
                }
            }
        }

        public static void Clear()
        {
            _components.Clear();
        }
    }
}