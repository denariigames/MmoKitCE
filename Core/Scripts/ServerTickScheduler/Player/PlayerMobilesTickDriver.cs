// ------------------------------------------------------------------------------
// PlayerMobilesTickDriver.cs
// ------------------------------------------------------------------------------
// Player-only tick driving registry.
//
// Goals:
//   - Keep Player entities out of UpdateManager (avoid per-entity update overhead)
//   - Centralize player simulation into a few ServerTickScheduler systems
//   - Provide a safe opt-in that only activates on server runtimes
//
// Notes:
//   - This is intentionally PLAYER ONLY (BasePlayerCharacterEntity).
//   - It is designed to be used with ServerRuntimeSimulation + ServerTickScheduler.
//   - We avoid allocations by reusing internal lists.
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class PlayerMobilesTickDriver
    {
        /// <summary>
        /// Set TRUE by the server runtime bootstrap when it registers tick systems.
        /// When false, player entities will continue using UpdateManager as before.
        /// </summary>
        public static bool Enabled { get; set; }

        // Live player list (server-side only)
        private static readonly List<BasePlayerCharacterEntity> s_Players = new List<BasePlayerCharacterEntity>(256);

        // Index map to support O(1) removals without allocations
        private static readonly Dictionary<uint, int> s_PlayerIndexByObjectId = new Dictionary<uint, int>(256);

        public static int Count => s_Players.Count;

        public static IReadOnlyList<BasePlayerCharacterEntity> Players => s_Players;

        /// <summary>
        /// Returns TRUE when we should run tick-driven player simulation on this runtime.
        /// Default behavior: Dedicated/headless server only.
        /// </summary>
        public static bool ShouldTickDriveOnThisRuntime()
        {
            if (!Enabled)
                return false;

            // Server-only
            if (!BaseGameNetworkManager.Singleton || !BaseGameNetworkManager.Singleton.IsServer)
                return false;

            // Prefer headless/dedicated runtimes. Prevent breaking host-mode editor workflows.
            if (Application.isBatchMode)
                return true;

            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
                return true;

#if UNITY_SERVER
            return true;
#else
            return false;
#endif
        }

        public static void Register(BasePlayerCharacterEntity player)
        {
            if (player == null)
                return;

            uint key = player.ObjectId;
            if (s_PlayerIndexByObjectId.ContainsKey(key))
                return;

            s_PlayerIndexByObjectId[key] = s_Players.Count;
            s_Players.Add(player);
        }

        public static void Unregister(BasePlayerCharacterEntity player)
        {
            if (player == null)
                return;

            uint key = player.ObjectId;
            if (!s_PlayerIndexByObjectId.TryGetValue(key, out int index))
                return;

            int lastIndex = s_Players.Count - 1;
            BasePlayerCharacterEntity last = s_Players[lastIndex];

            // Swap-remove
            s_Players[index] = last;
            s_Players.RemoveAt(lastIndex);

            // Fix moved index
            s_PlayerIndexByObjectId.Remove(key);
            if (index < lastIndex && last != null)
                s_PlayerIndexByObjectId[last.ObjectId] = index;
        }

        public static void Clear()
        {
            s_Players.Clear();
            s_PlayerIndexByObjectId.Clear();
        }
    }
}
