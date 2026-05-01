using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public interface ILoginServerDataManager
    {
        string GenerateCharacterId();
        bool CanCreateCharacter(ref int dataId, ref int entityId, ref int factionId, IList<CharacterDataBoolean> publicBools, IList<CharacterDataInt32> publicInts, IList<CharacterDataFloat32> publicFloats, out UITextKeys errorMessage);
        void SetNewPlayerCharacterData(PlayerCharacterData playerCharacterData, string characterName, int dataId, int entityId, int factionId, IList<CharacterDataBoolean> publicBools, IList<CharacterDataInt32> publicInts, IList<CharacterDataFloat32> publicFloats);
        string GenerateAccessToken(string userId);
        string GetUserIdFromAccessToken(string accessToken);
    }
}
