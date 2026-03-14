using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class CharacterCreationData
    {
        public Dictionary<int, Dictionary<int, PlayerCharacterData>> AvailableCharacters { get; private set; } = new Dictionary<int, Dictionary<int, PlayerCharacterData>>();
        public List<int> AvailableFactionIds { get; private set; } = new List<int>();

        public bool CanCreateCharacter(int dataId, int entityId, int factionId, IList<CharacterDataBoolean> publicBools, IList<CharacterDataInt32> publicInts, IList<CharacterDataFloat32> publicFloats)
        {
            return AvailableCharacters.ContainsKey(entityId) && AvailableCharacters[entityId].ContainsKey(dataId) && AvailableFactionIds.Contains(factionId);
        }

        public PlayerCharacterData GetCreateCharacterData(string id, string userId, string characterName, int dataId, int entityId, int factionId, IList<CharacterDataBoolean> publicBools, IList<CharacterDataInt32> publicInts, IList<CharacterDataFloat32> publicFloats)
        {
            PlayerCharacterData result = AvailableCharacters[entityId][dataId].CloneTo(new PlayerCharacterData());
            result.Id = id;
            result.UserId = userId;
            result.CharacterName = characterName;
            result.FactionId = factionId;
#if !DISABLE_CUSTOM_CHARACTER_DATA
            result.PublicBools = publicBools;
            result.PublicInts = publicInts;
            result.PublicFloats = publicFloats;
#endif
            return result;
        }

        public void SetCreateCharacterData(PlayerCharacterData data, string id, string userId, string characterName, int dataId, int entityId, int factionId, IList<CharacterDataBoolean> publicBools, IList<CharacterDataInt32> publicInts, IList<CharacterDataFloat32> publicFloats)
        {
            data = AvailableCharacters[entityId][dataId].CloneTo(data);
            data.Id = id;
            data.UserId = userId;
            data.CharacterName = characterName;
            data.FactionId = factionId;
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PublicBools = publicBools;
            data.PublicInts = publicInts;
            data.PublicFloats = publicFloats;
#endif
        }
    }
}
