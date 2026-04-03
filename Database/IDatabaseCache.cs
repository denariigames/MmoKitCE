using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial interface IDatabaseCache
    {
        UniTask<bool> SetPlayerCharacter(PlayerCharacterData playerCharacter);
        UniTask<DatabaseCacheResult<PlayerCharacterData>> GetPlayerCharacter(string characterId);
        UniTask<bool> RemovePlayerCharacter(string characterId);
        UniTask<bool> SetPlayerCharacterPartyId(string characterId, int partyId);
        UniTask<bool> SetPlayerCharacterGuildId(string characterId, int guildId);
        UniTask<bool> SetPlayerCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole);
        UniTask<bool> SetPlayerCharacterSelectableWeaponSets(string characterId, List<EquipWeapons> selectableWeaponSets);
        UniTask<bool> SetPlayerCharacterEquipItems(string characterId, List<CharacterItem> equipItems);
        UniTask<bool> SetPlayerCharacterNonEquipItems(string characterId, List<CharacterItem> nonEquipItems);

        UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter);
        UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId);
        UniTask<bool> RemoveSocialCharacter(string characterId);
        UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId);
        UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId);
        UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole);

        UniTask<bool> SetBuilding(string channel, string mapName, BuildingSaveData building);
        UniTask<DatabaseCacheResult<BuildingSaveData>> GetBuilding(string channel, string mapName, string buildingId);
        UniTask<bool> RemoveBuilding(string channel, string mapName, string buildingId);

        UniTask<bool> SetBuildings(string channel, string mapName, IEnumerable<BuildingSaveData> buildings);
        UniTask<DatabaseCacheResult<IEnumerable<BuildingSaveData>>> GetBuildings(string channel, string mapName);
        UniTask<bool> RemoveBuildings(string channel, string mapName);

        UniTask<bool> SetParty(PartyData party);
        UniTask<DatabaseCacheResult<PartyData>> GetParty(int id);
        UniTask<bool> RemoveParty(int id);

        UniTask<bool> SetGuild(GuildData guild);
        UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id);
        UniTask<bool> RemoveGuild(int id);

        UniTask<bool> SetStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> items);
        UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageType storageType, string storageOwnerId);
        UniTask<bool> RemoveStorageItems(StorageType storageType, string storageOwnerId);

        UniTask<bool> SetSummonBuffs(string characterId, List<CharacterBuff> items);
        UniTask<DatabaseCacheResult<List<CharacterBuff>>> GetSummonBuffs(string characterId);
        UniTask<bool> RemoveSummonBuffs(string characterId);
    }
}
