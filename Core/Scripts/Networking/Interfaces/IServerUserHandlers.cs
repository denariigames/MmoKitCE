using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerUserHandlers
    {
        /// <summary>
        /// Count online characters
        /// </summary>
        int PlayerCharactersCount { get; }

        /// <summary>
        /// Get all online characters
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPlayerCharacterData> GetPlayerCharacters();

        /// <summary>
        /// Get character from server's collection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        bool TryGetPlayerCharacter(long connectionId, out IPlayerCharacterData playerCharacter);

        /// <summary>
        /// Get character from server's collection by character's ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        bool TryGetPlayerCharacterById(string id, out IPlayerCharacterData playerCharacter);

        /// <summary>
        /// Get character from server's collection by user's ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        bool TryGetPlayerCharacterByUserId(string userId, out IPlayerCharacterData playerCharacter);

        /// <summary>
        /// Get character from server's collection by character's name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        bool TryGetPlayerCharacterByName(string name, out IPlayerCharacterData playerCharacter);

        /// <summary>
        /// Get connection ID by character's ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        bool TryGetConnectionIdById(string id, out long connectionId);

        /// <summary>
        /// Get connection ID by user's ID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        bool TryGetConnectionIdByUserId(string userId, out long connectionId);

        /// <summary>
        /// Get connection ID by character's name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        bool TryGetConnectionIdByName(string name, out long connectionId);

        /// <summary>
        /// Add character to server's collection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        bool AddPlayerCharacter(long connectionId, IPlayerCharacterData playerCharacter);

        /// <summary>
        /// Remove character from server's collection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="characterId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool RemovePlayerCharacter(long connectionId, out string characterId, out string userId);

        /// <summary>
        /// Clear server's collection (and other relates variables)
        /// </summary>
        void ClearUsersAndPlayerCharacters();

        /// <summary>
        /// Count online user IDs
        /// </summary>
        int UserIdsCount { get; }

        /// <summary>
        /// Get all online user IDs
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetUserIds();

        /// <summary>
        /// Get user id by connection id
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool TryGetUserId(long connectionId, out string userId);

        /// <summary>
        /// Add user id to server's collection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool AddUserId(long connectionId, string userId);

        /// <summary>
        /// Remove user id from server's collection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool RemoveUserId(long connectionId, out string userId);

        /// <summary>
        /// Count user's access tokens
        /// </summary>
        int AccessTokensCount { get; }

        /// <summary>
        /// Get all user's access tokens
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAccessTokens();

        /// <summary>
        /// Get access token by connection id
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        bool TryGetAccessToken(long connectionId, out string accessToken);

        /// <summary>
        /// Add access token to server's collection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        bool AddAccessToken(long connectionId, string accessToken);

        /// <summary>
        /// Remove access token from server's collection
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        bool RemoveAccessToken(long connectionId, out string accessToken);

        /// <summary>
        /// Ban user who own character which its name = `characterName`
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="days"></param>
        void BanUserByCharacterName(string characterName, int days);

        /// <summary>
        /// Unban user who own character which its name = `characterName`
        /// </summary>
        /// <param name="characterName"></param>
        void UnbanUserByCharacterName(string characterName);

        /// <summary>
        /// Mute character which its name = `characterName`
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="minutes"></param>
        void MuteCharacterByName(string characterName, int minutes);

        /// <summary>
        /// Unmute user who own character which its name = `characterName`
        /// </summary>
        /// <param name="characterName"></param>
        void UnmuteCharacterByName(string characterName);

        /// <summary>
        /// Change user gold, we will use this to change user gold to database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gold"></param>
        void ChangeUserGold(string userId, int gold);

        /// <summary>
        /// Change user cash, we will use this to change user cash to database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cash"></param>
        void ChangeUserCash(string userId, int cash);

        /// <summary>
        /// Check if the character name is existed or not
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        UniTask<UITextKeys> ValidateCharacterName(string characterName);
    }
}
