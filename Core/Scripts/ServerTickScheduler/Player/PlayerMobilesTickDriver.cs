using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    /// <summary>
    /// Player-only tick-driving registry.
    /// </summary>
    public static class PlayerMobilesTickDriver
    {
        public static bool Enabled { get; set; }

        private static readonly List<BasePlayerCharacterEntity> s_Players = new List<BasePlayerCharacterEntity>(256);
        private static readonly HashSet<BasePlayerCharacterEntity> s_Set = new HashSet<BasePlayerCharacterEntity>();

        public static int Count { get { return s_Players.Count; } }
        public static IReadOnlyList<BasePlayerCharacterEntity> Players { get { return s_Players; } }

        public static bool ShouldTickDriveOnThisRuntime()
        {
            if (!Enabled)
                return false;

            if (!BaseGameNetworkManager.Singleton || !BaseGameNetworkManager.Singleton.IsServer)
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

        public static void Register(BasePlayerCharacterEntity player)
        {
            if (!player)
                return;

            if (s_Set.Add(player))
                s_Players.Add(player);
        }

        public static void Unregister(BasePlayerCharacterEntity player)
        {
            if (!player)
                return;

            if (!s_Set.Remove(player))
                return;

            for (int i = 0; i < s_Players.Count; ++i)
            {
                if (s_Players[i] == player)
                {
                    int last = s_Players.Count - 1;
                    s_Players[i] = s_Players[last];
                    s_Players.RemoveAt(last);
                    return;
                }
            }
        }

        public static void Compact()
        {
            for (int i = s_Players.Count - 1; i >= 0; --i)
            {
                BasePlayerCharacterEntity player = s_Players[i];
                if (!player)
                    s_Players.RemoveAt(i);
            }

            s_Set.Clear();
            for (int i = 0; i < s_Players.Count; ++i)
            {
                BasePlayerCharacterEntity player = s_Players[i];
                if (player)
                    s_Set.Add(player);
            }
        }

        public static void Clear()
        {
            s_Players.Clear();
            s_Set.Clear();
        }
    }
}
