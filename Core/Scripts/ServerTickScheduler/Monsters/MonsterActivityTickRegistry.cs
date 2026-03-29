using System.Collections.Generic;

namespace MultiplayerARPG.Server.AI
{
    public static class MonsterActivityTickRegistry
    {
        // Public setter so bootstrap code in other assemblies/asmdefs can enable tick-driving.
        public static bool TickDrivenEnabled { get; set; }

        private static readonly HashSet<MultiplayerARPG.MonsterActivityComponent> _components
            = new HashSet<MultiplayerARPG.MonsterActivityComponent>();

        // Reused snapshot list to avoid allocations
        private static readonly List<MultiplayerARPG.MonsterActivityComponent> _snapshot
            = new List<MultiplayerARPG.MonsterActivityComponent>(1024);

        public static void Register(MultiplayerARPG.MonsterActivityComponent component)
        {
            if (component != null)
                _components.Add(component);
        }

        public static void Unregister(MultiplayerARPG.MonsterActivityComponent component)
        {
            if (component != null)
                _components.Remove(component);
        }

        /// <summary>
        /// Safe for iteration during ticks (stable list for the duration of Execute()).
        /// </summary>
        public static List<MultiplayerARPG.MonsterActivityComponent> Snapshot()
        {
            _snapshot.Clear();
            foreach (var c in _components)
            {
                if (c != null)
                    _snapshot.Add(c);
            }
            return _snapshot;
        }
    }
}
