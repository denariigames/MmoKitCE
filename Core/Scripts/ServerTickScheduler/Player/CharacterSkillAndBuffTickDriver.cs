using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    /// <summary>
    /// Registry for CharacterSkillAndBuffComponent instances that should be driven by ServerTickScheduler.
    /// </summary>
    public static class CharacterSkillAndBuffTickDriver
    {
        /// <summary>
        /// Allows you to disable tick-driving globally if needed.
        /// ServerRuntimeSimulation should set this when scheduler is running.
        /// </summary>
        public static bool Enabled = true;

        // Simple list registry; swap-remove to keep O(1) removals.
        private static readonly List<CharacterSkillAndBuffComponent> _components = new List<CharacterSkillAndBuffComponent>(1024);

        public static IReadOnlyList<CharacterSkillAndBuffComponent> Components => _components;

        /// <summary>
        /// True when we should tick-drive on this runtime (dedicated/headless server).
        /// </summary>
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

        public static void Register(CharacterSkillAndBuffComponent comp)
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

        public static void Unregister(CharacterSkillAndBuffComponent comp)
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
