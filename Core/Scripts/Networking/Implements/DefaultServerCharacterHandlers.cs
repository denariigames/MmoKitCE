using Cysharp.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Collections.Concurrent;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultServerCharacterHandlers : MonoBehaviour, IServerCharacterHandlers
    {
        public static readonly ConcurrentDictionary<string, float> OnlineCharacterIds = new ConcurrentDictionary<string, float>();

        public BaseGameNetworkManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<BaseGameNetworkManager>();
        }

        public void HandleRequestOnlineCharacter(MessageHandlerData messageHandler)
        {
            string characterId = messageHandler.Reader.GetString();
            if (string.IsNullOrEmpty(characterId))
                return;
            if (OnlineCharacterIds.TryGetValue(characterId, out float lastOnlineTime))
            {
                // Notify back online character
                Manager.ServerSendPacket(messageHandler.ConnectionId, 0, DeliveryMethod.Sequenced, GameNetworkingConsts.NotifyOnlineCharacter, (writer) =>
                {
                    writer.Put(characterId);
                    writer.PutPackedInt(Mathf.FloorToInt(Time.unscaledTime - lastOnlineTime));
                });
            }
            // NOTE: For MMO games, it should get offline offsets from database for exact offline offsets
        }

        public void MarkOnlineCharacter(string characterId)
        {
            if (string.IsNullOrEmpty(characterId))
                return;
            OnlineCharacterIds[characterId] = Time.unscaledTime;
        }

        public void ClearOnlineCharacters()
        {
            OnlineCharacterIds.Clear();
        }

        public async UniTask Respawn(int option, IPlayerCharacterData playerCharacter)
        {
            WarpPortalType respawnPortalType = WarpPortalType.Default;
#if !DISABLE_DIFFER_MAP_RESPAWNING
            string respawnMapName = playerCharacter.RespawnMapName;
            Vector3 respawnPosition = playerCharacter.RespawnPosition;
#else
            string respawnMapName = playerCharacter.CurrentMapName;
            Vector3 respawnPosition = playerCharacter.CurrentPosition;
#endif
            bool respawnOverrideRotation = false;
            Vector3 respawnRotation = Vector3.zero;
            BaseMapInfo mapInfo = BaseGameNetworkManager.CurrentMapInfo;
            if (GameInstance.MapInfos.TryGetValue(playerCharacter.CurrentMapName, out mapInfo))
            {
                mapInfo.GetRespawnPoint(playerCharacter, out respawnPortalType, out respawnMapName, out respawnPosition, out respawnOverrideRotation, out respawnRotation);
            }
            if (playerCharacter is BasePlayerCharacterEntity entity)
            {
                switch (respawnPortalType)
                {
                    case WarpPortalType.Default:
                        bool isSameMap = respawnMapName.Equals(BaseGameNetworkManager.CurrentMapInfo.Id);
                        if (!isSameMap)
                        {
                            // Respawn immediately before move
                            GameInstance.Singleton.GameplayRule.OnCharacterRespawn(playerCharacter);
                        }
                        await Manager.WarpCharacter(entity, respawnMapName, respawnPosition, respawnOverrideRotation, respawnRotation);
                        if (isSameMap)
                        {
                            // Wait until teleported before respawn
                            GameInstance.Singleton.GameplayRule.OnCharacterRespawn(playerCharacter);
                            entity.OnRespawn();
                        }
                        break;
                    case WarpPortalType.EnterInstance:
                        // Respawn immediately before move
                        GameInstance.Singleton.GameplayRule.OnCharacterRespawn(playerCharacter);
                        await Manager.WarpCharacterToInstance(entity, respawnMapName, respawnPosition, respawnOverrideRotation, respawnRotation);
                        break;
                }
            }
            else
            {
                playerCharacter.CurrentMapName = respawnMapName;
                playerCharacter.CurrentPosition = respawnPosition;
            }
        }
    }
}
