using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultServerUserHandlers : MonoBehaviour, IServerUserHandlers
    {
        public static readonly ConcurrentDictionary<long, IPlayerCharacterData> PlayerCharacters = new ConcurrentDictionary<long, IPlayerCharacterData>();
        public static readonly ConcurrentDictionary<string, IPlayerCharacterData> PlayerCharactersById = new ConcurrentDictionary<string, IPlayerCharacterData>();
        public static readonly ConcurrentDictionary<string, IPlayerCharacterData> PlayerCharactersByUserId = new ConcurrentDictionary<string, IPlayerCharacterData>();
        public static readonly ConcurrentDictionary<string, IPlayerCharacterData> PlayerCharactersByName = new ConcurrentDictionary<string, IPlayerCharacterData>();
        public static readonly ConcurrentDictionary<string, long> ConnectionIdsById = new ConcurrentDictionary<string, long>();
        public static readonly ConcurrentDictionary<string, long> ConnectionIdsByUserId = new ConcurrentDictionary<string, long>();
        public static readonly ConcurrentDictionary<string, long> ConnectionIdsByName = new ConcurrentDictionary<string, long>();
        public static readonly ConcurrentDictionary<long, string> UserIds = new ConcurrentDictionary<long, string>();
        public static readonly ConcurrentDictionary<long, string> AccessTokens = new ConcurrentDictionary<long, string>();

        public int PlayerCharactersCount
        {
            get { return PlayerCharacters.Count; }
        }

        public int UserIdsCount
        {
            get { return UserIds.Count; }
        }

        public int AccessTokensCount
        {
            get { return AccessTokens.Count; }
        }

        public IEnumerable<IPlayerCharacterData> GetPlayerCharacters()
        {
            return PlayerCharacters.Values;
        }

        public bool TryGetPlayerCharacter(long connectionId, out IPlayerCharacterData playerCharacter)
        {
            return PlayerCharacters.TryGetValue(connectionId, out playerCharacter);
        }

        public bool TryGetPlayerCharacterById(string id, out IPlayerCharacterData playerCharacter)
        {
            playerCharacter = null;
            return !string.IsNullOrWhiteSpace(id) && PlayerCharactersById.TryGetValue(id, out playerCharacter);
        }

        public bool TryGetPlayerCharacterByUserId(string userId, out IPlayerCharacterData playerCharacter)
        {
            playerCharacter = null;
            return !string.IsNullOrWhiteSpace(userId) && PlayerCharactersByUserId.TryGetValue(userId, out playerCharacter);
        }

        public bool TryGetPlayerCharacterByName(string name, out IPlayerCharacterData playerCharacter)
        {
            playerCharacter = null;
            return !string.IsNullOrWhiteSpace(name) && PlayerCharactersByName.TryGetValue(name, out playerCharacter);
        }

        public bool TryGetConnectionIdById(string id, out long connectionId)
        {
            connectionId = -1;
            return !string.IsNullOrWhiteSpace(id) && ConnectionIdsById.TryGetValue(id, out connectionId);
        }

        public bool TryGetConnectionIdByUserId(string userId, out long connectionId)
        {
            connectionId = -1;
            return !string.IsNullOrWhiteSpace(userId) && ConnectionIdsByUserId.TryGetValue(userId, out connectionId);
        }

        public bool TryGetConnectionIdByName(string name, out long connectionId)
        {
            connectionId = -1;
            return !string.IsNullOrWhiteSpace(name) && ConnectionIdsByName.TryGetValue(name, out connectionId);
        }

        public bool AddPlayerCharacter(long connectionId, IPlayerCharacterData playerCharacter)
        {
            if (playerCharacter == null || string.IsNullOrEmpty(playerCharacter.Id) || string.IsNullOrEmpty(playerCharacter.UserId) || string.IsNullOrEmpty(playerCharacter.CharacterName))
                return false;
            if (PlayerCharacters.TryAdd(connectionId, playerCharacter))
            {
                PlayerCharactersById[playerCharacter.Id] = playerCharacter;
                PlayerCharactersByUserId[playerCharacter.UserId] = playerCharacter;
                PlayerCharactersByName[playerCharacter.CharacterName] = playerCharacter;
                ConnectionIdsById[playerCharacter.Id] = connectionId;
                ConnectionIdsByUserId[playerCharacter.UserId] = connectionId;
                ConnectionIdsByName[playerCharacter.CharacterName] = connectionId;
                return true;
            }
            return false;
        }

        public bool RemovePlayerCharacter(long connectionId, out string characterId, out string userId)
        {
            IPlayerCharacterData playerCharacter;
            if (PlayerCharacters.TryRemove(connectionId, out playerCharacter))
            {
                characterId = playerCharacter.Id;
                userId = playerCharacter.UserId;
                string name = playerCharacter.CharacterName;
                PlayerCharactersById.TryRemove(characterId, out _);
                PlayerCharactersByUserId.TryRemove(userId, out _);
                PlayerCharactersByName.TryRemove(name, out _);
                ConnectionIdsById.TryRemove(characterId, out _);
                ConnectionIdsByUserId.TryRemove(userId, out _);
                ConnectionIdsByName.TryRemove(name, out _);
                return true;
            }
            characterId = null;
            userId = null;
            return false;
        }

        public void ClearUsersAndPlayerCharacters()
        {
            PlayerCharacters.Clear();
            PlayerCharactersById.Clear();
            PlayerCharactersByUserId.Clear();
            PlayerCharactersByName.Clear();
            ConnectionIdsById.Clear();
            ConnectionIdsByUserId.Clear();
            ConnectionIdsByName.Clear();
            UserIds.Clear();
            AccessTokens.Clear();
        }

        public IEnumerable<string> GetUserIds()
        {
            return UserIds.Values;
        }

        public bool TryGetUserId(long connectionId, out string userId)
        {
            return UserIds.TryGetValue(connectionId, out userId);
        }

        public bool AddUserId(long connectionId, string userId)
        {
            if (UserIds.TryAdd(connectionId, userId))
            {
                ConnectionIdsByUserId[userId] = connectionId;
                return true;
            }
            return false;
        }

        public bool RemoveUserId(long connectionId, out string userId)
        {
            if (UserIds.TryRemove(connectionId, out userId))
            {
                ConnectionIdsByUserId.TryRemove(userId, out _);
                return true;
            }
            return false;
        }

        public IEnumerable<string> GetAccessTokens()
        {
            return AccessTokens.Values;
        }

        public bool TryGetAccessToken(long connectionId, out string accessToken)
        {
            return AccessTokens.TryGetValue(connectionId, out accessToken);
        }

        public bool AddAccessToken(long connectionId, string accessToken)
        {
            return AccessTokens.TryAdd(connectionId, accessToken);
        }

        public bool RemoveAccessToken(long connectionId, out string accessToken)
        {
            return AccessTokens.TryRemove(connectionId, out accessToken);
        }

        public virtual void BanUserByCharacterName(string characterName, int days)
        {
            throw new System.NotImplementedException();
        }

        public virtual void UnbanUserByCharacterName(string characterName)
        {
            throw new System.NotImplementedException();
        }

        public virtual void MuteCharacterByName(string characterName, int minutes)
        {
            long time = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (60 * minutes);
            if (TryGetPlayerCharacterByName(characterName, out IPlayerCharacterData playerCharacter))
                playerCharacter.UnmuteTime = time;
        }

        public virtual void UnmuteCharacterByName(string characterName)
        {
            if (TryGetPlayerCharacterByName(characterName, out IPlayerCharacterData playerCharacter))
                playerCharacter.UnmuteTime = 0;
        }

        public virtual void ChangeUserGold(string userId, int gold)
        {
            if (!TryGetPlayerCharacterByUserId(userId, out IPlayerCharacterData playerCharacter))
                return;
            playerCharacter.UserGold = playerCharacter.UserGold.Increase(gold);
        }

        public virtual void ChangeUserCash(string userId, int cash)
        {
            if (!TryGetPlayerCharacterByUserId(userId, out IPlayerCharacterData playerCharacter))
                return;
            playerCharacter.UserCash = playerCharacter.UserCash.Increase(cash);
        }

        public virtual UniTask<UITextKeys> ValidateCharacterName(string characterName)
        {
            return new UniTask<UITextKeys>(UITextKeys.NONE);
        }
    }
}
