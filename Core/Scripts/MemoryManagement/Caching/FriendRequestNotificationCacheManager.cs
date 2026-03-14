using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class FriendRequestNotificationCacheManager
    {
        public static float CACHE_EXPIRE_DURATION = 30f;
        private static int s_amount = 0;
        private static float s_cachedTime = 0f;
        private static bool s_isLoading = false;
        public static System.Action<int> onSetFriendRequestNotification;

        public static void LoadOrGetFriendRequestNotificationFromCache(System.Action<int> callback, bool force = false)
        {
            if (s_isLoading)
            {
                callback.Invoke(s_amount);
                return;
            }
            if (!force && Time.unscaledTime - s_cachedTime < CACHE_EXPIRE_DURATION)
            {
                callback.Invoke(s_amount);
                return;
            }
            s_isLoading = true;
            GameInstance.ClientFriendHandlers.RequestFriendRequestNotification((requestHandler, responseCode, response) =>
            {
                s_isLoading = false;
                if (responseCode != AckResponseCode.Success)
                    return;
                SetCache(response.notificationCount);
                callback.Invoke(response.notificationCount);
            });
        }

        public static void SetCache(int count)
        {
            s_amount = count;
            s_cachedTime = Time.unscaledTime;
            if (onSetFriendRequestNotification != null)
                onSetFriendRequestNotification.Invoke(count);
        }

        public static void ClearCache()
        {
            s_amount = 0;
            s_cachedTime = 0f;
        }
    }
}
