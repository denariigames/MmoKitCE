using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class GuildInfoCacheManager
    {
        public static float CACHE_EXPIRE_DURATION = 10f;
        private static readonly Dictionary<int, GuildListEntry> s_caches = new Dictionary<int, GuildListEntry>();
        private static readonly Dictionary<int, float> s_cachedTimes = new Dictionary<int, float>();
        private static readonly HashSet<int> s_loadingIds = new HashSet<int>();
        public static System.Action<GuildListEntry> onSetGuildInfo;

        public static void LoadOrGetGuildInfoFromCache(int guildId, System.Action<GuildListEntry> callback, bool force = false)
        {
            if (s_loadingIds.Contains(guildId))
            {
                if (s_cachedTimes.ContainsKey(guildId))
                    callback.Invoke(s_caches[guildId]);
                return;
            }
            if (!force && s_cachedTimes.ContainsKey(guildId) && Time.unscaledTime - s_cachedTimes[guildId] < CACHE_EXPIRE_DURATION)
            {
                callback.Invoke(s_caches[guildId]);
                return;
            }
            s_loadingIds.Add(guildId);
            GameInstance.ClientGuildHandlers.RequestGetGuildInfo(new RequestGetGuildInfoMessage()
            {
                guildId = guildId,
            }, (requestHandler, responseCode, response) =>
            {
                s_loadingIds.Remove(response.guild.Id);
                if (responseCode != AckResponseCode.Success)
                    return;
                SetCache(response.guild);
                callback.Invoke(response.guild);
            });
        }

        public static void SetCache(GuildListEntry data)
        {
            s_caches[data.Id] = data;
            s_cachedTimes[data.Id] = Time.unscaledTime;
            if (onSetGuildInfo != null)
                onSetGuildInfo.Invoke(data);
        }

        public static bool TryGetFromCache(int id, out GuildListEntry data)
        {
            return s_caches.TryGetValue(id, out data);
        }

        public static void ClearCache(int guildId)
        {
            s_caches.Remove(guildId);
            s_cachedTimes.Remove(guildId);
        }
    }
}
