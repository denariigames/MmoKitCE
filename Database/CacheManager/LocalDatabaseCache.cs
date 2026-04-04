using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class LocalDatabaseCache : IDatabaseCache
    {
        private ConcurrentDictionary<string, PlayerCharacterData> _cachedPlayerCharacters = new ConcurrentDictionary<string, PlayerCharacterData>();
        private ConcurrentDictionary<string, SocialCharacterData> _cachedSocialCharacters = new ConcurrentDictionary<string, SocialCharacterData>();
        private ConcurrentDictionary<string, ConcurrentDictionary<string, BuildingSaveData>> _cachedBuilding = new ConcurrentDictionary<string, ConcurrentDictionary<string, BuildingSaveData>>();
        private ConcurrentDictionary<int, PartyData> _cachedParties = new ConcurrentDictionary<int, PartyData>();
        private ConcurrentDictionary<int, GuildData> _cachedGuilds = new ConcurrentDictionary<int, GuildData>();
        private ConcurrentDictionary<StorageId, List<CharacterItem>> _cachedStorageItems = new ConcurrentDictionary<StorageId, List<CharacterItem>>();
        private ConcurrentDictionary<string, List<CharacterBuff>> _cachedSummonBuffs = new ConcurrentDictionary<string, List<CharacterBuff>>();

        public UniTask<bool> SetPlayerCharacter(PlayerCharacterData playerCharacter)
        {
            if (_cachedPlayerCharacters.TryGetValue(playerCharacter.Id, out var prevPlayerCharacter))
            {
                playerCharacter.PartyId = prevPlayerCharacter.PartyId;
                playerCharacter.GuildId = prevPlayerCharacter.GuildId;
                playerCharacter.GuildRole = prevPlayerCharacter.GuildRole;
            }
            _cachedPlayerCharacters[playerCharacter.Id] = playerCharacter;
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<PlayerCharacterData>> GetPlayerCharacter(string characterId)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
                return UniTask.FromResult(new DatabaseCacheResult<PlayerCharacterData>(playerCharacter));
            return UniTask.FromResult(new DatabaseCacheResult<PlayerCharacterData>());
        }
        public UniTask<bool> RemovePlayerCharacter(string characterId)
        {
            return UniTask.FromResult(_cachedPlayerCharacters.TryRemove(characterId, out _));
        }
        public async UniTask<bool> SetPlayerCharacterPartyId(string characterId, int partyId)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.PartyId = partyId;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetPlayerCharacterGuildId(string characterId, int guildId)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.GuildId = guildId;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetPlayerCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.GuildId = guildId;
                playerCharacter.GuildRole = guildRole;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }

        public async UniTask<bool> SetPlayerCharacterSelectableWeaponSets(string characterId, List<EquipWeapons> selectableWeaponSets)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.SelectableWeaponSets = selectableWeaponSets;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }

        public async UniTask<bool> SetPlayerCharacterEquipItems(string characterId, List<CharacterItem> equipItems)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.EquipItems = equipItems;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }

        public async UniTask<bool> SetPlayerCharacterNonEquipItems(string characterId, List<CharacterItem> nonEquipItems)
        {
            if (_cachedPlayerCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.NonEquipItems = nonEquipItems;
                return await SetPlayerCharacter(playerCharacter);
            }
            return false;
        }

        public UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter)
        {
            if (_cachedSocialCharacters.TryGetValue(playerCharacter.id, out var prevPlayerCharacter))
            {
                playerCharacter.partyId = prevPlayerCharacter.partyId;
                playerCharacter.guildId = prevPlayerCharacter.guildId;
                playerCharacter.guildRole = prevPlayerCharacter.guildRole;
            }
            _cachedSocialCharacters[playerCharacter.id] = playerCharacter;
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId)
        {
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
                return UniTask.FromResult(new DatabaseCacheResult<SocialCharacterData>(playerCharacter));
            return UniTask.FromResult(new DatabaseCacheResult<SocialCharacterData>());
        }
        public UniTask<bool> RemoveSocialCharacter(string characterId)
        {
            return UniTask.FromResult(_cachedSocialCharacters.TryRemove(characterId, out _));
        }
        public async UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId)
        {
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.partyId = partyId;
                await SetSocialCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId)
        {
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.guildId = guildId;
                await SetSocialCharacter(playerCharacter);
            }
            return false;
        }
        public async UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            if (_cachedSocialCharacters.TryGetValue(characterId, out var playerCharacter))
            {
                playerCharacter.guildId = guildId;
                playerCharacter.guildRole = guildRole;
                await SetSocialCharacter(playerCharacter);
            }
            return false;
        }

        public UniTask<bool> SetBuilding(string channel, string mapName, BuildingSaveData building)
        {
            string key = $"{channel}_{mapName}";
            if (!_cachedBuilding.ContainsKey(key))
                _cachedBuilding[key] = new ConcurrentDictionary<string, BuildingSaveData>();
            _cachedBuilding[key][building.Id] = building;
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<BuildingSaveData>> GetBuilding(string channel, string mapName, string buildingId)
        {
            string key = $"{channel}_{mapName}";
            if (_cachedBuilding.TryGetValue(key, out var buildings) && buildings.TryGetValue(buildingId, out var building))
                return UniTask.FromResult(new DatabaseCacheResult<BuildingSaveData>(building));
            return UniTask.FromResult(new DatabaseCacheResult<BuildingSaveData>());
        }
        public UniTask<bool> RemoveBuilding(string channel, string mapName, string buildingId)
        {
            string key = $"{channel}_{mapName}";
            return UniTask.FromResult(_cachedBuilding.TryGetValue(key, out var buildings) && buildings.TryRemove(buildingId, out _));
        }

        public UniTask<bool> SetBuildings(string channel, string mapName, IEnumerable<BuildingSaveData> buildings)
        {
            string key = $"{channel}_{mapName}";
            if (!_cachedBuilding.ContainsKey(key))
                _cachedBuilding[key] = new ConcurrentDictionary<string, BuildingSaveData>();
            foreach (BuildingSaveData building in buildings)
                _cachedBuilding[key][building.Id] = building;
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<IEnumerable<BuildingSaveData>>> GetBuildings(string channel, string mapName)
        {
            string key = $"{channel}_{mapName}";
            if (_cachedBuilding.TryGetValue(key, out var buildings))
                return UniTask.FromResult(new DatabaseCacheResult<IEnumerable<BuildingSaveData>>(buildings.Values));
            return UniTask.FromResult(new DatabaseCacheResult<IEnumerable<BuildingSaveData>>());
        }
        public UniTask<bool> RemoveBuildings(string channel, string mapName)
        {
            string key = $"{channel}_{mapName}";
            return UniTask.FromResult(_cachedBuilding.TryRemove(key, out _));
        }

        public UniTask<bool> SetParty(PartyData party)
        {
            _cachedParties[party.id] = party;
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<PartyData>> GetParty(int id)
        {
            if (_cachedParties.TryGetValue(id, out var party))
                return UniTask.FromResult(new DatabaseCacheResult<PartyData>(party));
            return UniTask.FromResult(new DatabaseCacheResult<PartyData>());
        }
        public UniTask<bool> RemoveParty(int id)
        {
            return UniTask.FromResult(_cachedParties.TryRemove(id, out _));
        }

        public UniTask<bool> SetGuild(GuildData guild)
        {
            _cachedGuilds[guild.id] = guild;
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id)
        {
            if (_cachedGuilds.TryGetValue(id, out var guild))
                return UniTask.FromResult(new DatabaseCacheResult<GuildData>(guild));
            return UniTask.FromResult(new DatabaseCacheResult<GuildData>());
        }
        public UniTask<bool> RemoveGuild(int id)
        {
            return UniTask.FromResult(_cachedGuilds.TryRemove(id, out _));
        }

        public UniTask<bool> SetStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> items)
        {
            StorageId storageId = new StorageId(storageType, storageOwnerId);
            _cachedStorageItems[storageId] = items;
            return UniTask.FromResult(true);
        }
        public UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageType storageType, string storageOwnerId)
        {
            StorageId storageId = new StorageId(storageType, storageOwnerId);
            if (_cachedStorageItems.TryGetValue(storageId, out var items))
                return UniTask.FromResult(new DatabaseCacheResult<List<CharacterItem>>(items));
            return UniTask.FromResult(new DatabaseCacheResult<List<CharacterItem>>());
        }
        public UniTask<bool> RemoveStorageItems(StorageType storageType, string storageOwnerId)
        {
            StorageId storageId = new StorageId(storageType, storageOwnerId);
            return UniTask.FromResult(_cachedStorageItems.TryRemove(storageId, out _));
        }

        public UniTask<bool> SetSummonBuffs(string characterId, List<CharacterBuff> buffs)
        {
            if (buffs == null)
                return UniTask.FromResult(true);
            _cachedSummonBuffs[characterId] = buffs;
            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<List<CharacterBuff>>> GetSummonBuffs(string characterId)
        {
            if (_cachedSummonBuffs.TryGetValue(characterId, out var buffs))
                return UniTask.FromResult(new DatabaseCacheResult<List<CharacterBuff>>(buffs));
            return UniTask.FromResult(new DatabaseCacheResult<List<CharacterBuff>>());
        }

        public UniTask<bool> RemoveSummonBuffs(string characterId)
        {
            return UniTask.FromResult(_cachedSummonBuffs.TryRemove(characterId, out _));
        }
    }
}
