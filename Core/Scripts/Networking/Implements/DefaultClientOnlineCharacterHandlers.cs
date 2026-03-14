using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Collections.Concurrent;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientOnlineCharacterHandlers : MonoBehaviour, IClientOnlineCharacterHandlers
    {
        public static readonly ConcurrentDictionary<string, float> OnlineRequestTimes = new ConcurrentDictionary<string, float>();
        public static readonly ConcurrentDictionary<string, int> OnlineCharacterIds = new ConcurrentDictionary<string, int>();

        public const int OnlineDuration = 5;
        public const float RequestDuration = 5f * 0.75f;
        public int DefaultOfflineOffsets { get; private set; }

        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
            DefaultOfflineOffsets = (int)(System.DateTime.Now - new System.DateTime(2021, 01, 01)).TotalSeconds;
        }

        public bool IsCharacterOnline(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
                return false;
            int offlineOffsets = GetCharacterOfflineOffsets(characterId);
            return offlineOffsets <= OnlineDuration;
        }

        public int GetCharacterOfflineOffsets(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
                return DefaultOfflineOffsets;
            int offlineOffsets;
            if (!OnlineCharacterIds.TryGetValue(characterId, out offlineOffsets))
                return DefaultOfflineOffsets;
            if (offlineOffsets < 0)
                return DefaultOfflineOffsets;
            return offlineOffsets;
        }

        public void RequestOnlineCharacter(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
                return;
            float currentTime = Time.unscaledTime;
            float lastRequestTime;
            if (OnlineRequestTimes.TryGetValue(characterId, out lastRequestTime) &&
                currentTime - lastRequestTime <= RequestDuration)
            {
                // Requested too frequently, so skip it
                return;
            }

            OnlineRequestTimes.TryRemove(characterId, out _);
            OnlineRequestTimes.TryAdd(characterId, currentTime);

            Manager.ClientSendPacket(0, DeliveryMethod.Sequenced, GameNetworkingConsts.NotifyOnlineCharacter, (writer) =>
            {
                writer.Put(characterId);
            });
        }

        public void HandleNotifyOnlineCharacter(MessageHandlerData messageHandler)
        {
            string characterId = messageHandler.Reader.GetString();
            int offlineOffsets = messageHandler.Reader.GetPackedInt();
            OnlineCharacterIds.TryRemove(characterId, out _);
            OnlineCharacterIds.TryAdd(characterId, offlineOffsets);
        }

        public void ClearOnlineCharacters()
        {
            OnlineRequestTimes.Clear();
            OnlineCharacterIds.Clear();
        }
    }
}
