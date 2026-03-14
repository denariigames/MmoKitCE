using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientCharacterHandlers : MonoBehaviour, IClientCharacterHandlers
    {
        public static readonly Dictionary<string, IPlayerCharacterData> SubscribedPlayerCharactersById = new Dictionary<string, IPlayerCharacterData>();
        public static readonly Dictionary<string, IPlayerCharacterData> SubscribedPlayerCharactersByName = new Dictionary<string, IPlayerCharacterData>();

        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestIncreaseAttributeAmount(RequestIncreaseAttributeAmountMessage data, ResponseDelegate<ResponseIncreaseAttributeAmountMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.IncreaseAttributeAmount, data, responseDelegate: callback);
        }

        public bool RequestIncreaseSkillLevel(RequestIncreaseSkillLevelMessage data, ResponseDelegate<ResponseIncreaseSkillLevelMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.IncreaseSkillLevel, data, responseDelegate: callback);
        }

        public bool RequestRespawn(RequestRespawnMessage data, ResponseDelegate<ResponseRespawnMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.Respawn, data, responseDelegate: callback);
        }

        public void SubscribePlayerCharacter(IPlayerCharacterData playerCharacter)
        {
            if (playerCharacter == null)
                return;
            if (!string.IsNullOrEmpty(playerCharacter.Id))
                SubscribedPlayerCharactersById[playerCharacter.Id] = playerCharacter;
            if (!string.IsNullOrEmpty(playerCharacter.CharacterName))
                SubscribedPlayerCharactersByName[playerCharacter.CharacterName] = playerCharacter;
        }

        public void UnsubscribePlayerCharacter(IPlayerCharacterData playerCharacter)
        {
            if (playerCharacter == null)
                return;
            if (!string.IsNullOrEmpty(playerCharacter.Id))
                SubscribedPlayerCharactersById.Remove(playerCharacter.Id);
            if (!string.IsNullOrEmpty(playerCharacter.CharacterName))
                SubscribedPlayerCharactersByName.Remove(playerCharacter.CharacterName);
        }

        public bool TryGetSubscribedPlayerCharacterById(string characterId, out IPlayerCharacterData playerCharacter)
        {
            return SubscribedPlayerCharactersById.TryGetValue(characterId, out playerCharacter);
        }

        public bool TryGetSubscribedPlayerCharacterByName(string characterName, out IPlayerCharacterData playerCharacter)
        {
            return SubscribedPlayerCharactersByName.TryGetValue(characterName, out playerCharacter);
        }

        public void ClearSubscribedPlayerCharacters()
        {
            SubscribedPlayerCharactersById.Clear();
            SubscribedPlayerCharactersByName.Clear();
        }

        public bool RequestSetIcon(RequestSetIconMessage data, ResponseDelegate<ResponseSetIconMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SetIcon, data, responseDelegate: callback);
        }

        public bool RequestSetFrame(RequestSetFrameMessage data, ResponseDelegate<ResponseSetFrameMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SetFrame, data, responseDelegate: callback);
        }

        public bool RequestSetBackground(RequestSetBackgroundMessage data, ResponseDelegate<ResponseSetBackgroundMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SetBackground, data, responseDelegate: callback);
        }

        public bool RequestSetTitle(RequestSetTitleMessage data, ResponseDelegate<ResponseSetTitleMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SetTitle, data, responseDelegate: callback);
        }

        public bool RequestPlayerCharacterTransform(ResponseDelegate<ResponsePlayerCharacterTransformMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.PlayerCharacterTransform, EmptyMessage.Value, responseDelegate: callback);
        }
    }
}
