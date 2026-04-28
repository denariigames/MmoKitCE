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
        public static bool Enabled = true;

        private static readonly List<CharacterSkillAndBuffComponent> _components = new List<CharacterSkillAndBuffComponent>(1024);
        private static readonly HashSet<CharacterSkillAndBuffComponent> _set = new HashSet<CharacterSkillAndBuffComponent>();

        public static IReadOnlyList<CharacterSkillAndBuffComponent> Components { get { return _components; } }
        public static int Count { get { return _components.Count; } }

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

            if (_set.Add(comp))
                _components.Add(comp);
        }

        public static void Unregister(CharacterSkillAndBuffComponent comp)
        {
            if (!comp)
                return;

            if (!_set.Remove(comp))
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

        public static void Compact()
        {
            for (int i = _components.Count - 1; i >= 0; --i)
            {
                CharacterSkillAndBuffComponent comp = _components[i];
                if (!comp)
                    _components.RemoveAt(i);
            }

            _set.Clear();
            for (int i = 0; i < _components.Count; ++i)
            {
                CharacterSkillAndBuffComponent comp = _components[i];
                if (comp)
                    _set.Add(comp);
            }
        }

        public static void Clear()
        {
            _components.Clear();
            _set.Clear();
        }
    }
}
