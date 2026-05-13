// ce scability: #53

using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class DisabledDatabaseCache : IDatabaseCache
    {
        private readonly object _lock = new object();

        private readonly Dictionary<string, PlayerCharacterData> _playerCharacters = new Dictionary<string, PlayerCharacterData>();
        private readonly Dictionary<string, SocialCharacterData> _socialCharacters = new Dictionary<string, SocialCharacterData>();
        private readonly Dictionary<string, List<CharacterBuff>> _summonBuffs = new Dictionary<string, List<CharacterBuff>>();

        private readonly Dictionary<int, PartyData> _parties = new Dictionary<int, PartyData>();
        private readonly Dictionary<int, GuildData> _guilds = new Dictionary<int, GuildData>();

        private readonly Dictionary<string, Dictionary<string, BuildingSaveData>> _buildingsByMap =
            new Dictionary<string, Dictionary<string, BuildingSaveData>>();
        private readonly Dictionary<string, List<CharacterItem>> _storageItems =
            new Dictionary<string, List<CharacterItem>>();

        private static string GetMapKey(string channel, string mapName)
        {
            return $"{channel}:{mapName}";
        }

        private static string GetStorageKey(StorageType storageType, string storageOwnerId)
        {
            return $"{(int)storageType}:{storageOwnerId}";
        }

        public UniTask<bool> SetPlayerCharacter(PlayerCharacterData playerCharacter)
        {
            if (playerCharacter == null || string.IsNullOrEmpty(playerCharacter.Id))
                return UniTask.FromResult(false);

            lock (_lock)
            {
                _playerCharacters[playerCharacter.Id] = playerCharacter;
            }

            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<PlayerCharacterData>> GetPlayerCharacter(string characterId)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _playerCharacters.TryGetValue(characterId, out PlayerCharacterData playerCharacter))
                {
                    return UniTask.FromResult(new DatabaseCacheResult<PlayerCharacterData>(playerCharacter));
                }
            }

            return UniTask.FromResult(new DatabaseCacheResult<PlayerCharacterData>());
        }

        public UniTask<bool> RemovePlayerCharacter(string characterId)
        {
            bool removed = false;

            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId))
                    removed = _playerCharacters.Remove(characterId);

                _summonBuffs.Remove(characterId);
            }

            return UniTask.FromResult(removed);
        }

        public UniTask<bool> SetPlayerCharacterPartyId(string characterId, int partyId)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _playerCharacters.TryGetValue(characterId, out PlayerCharacterData playerCharacter))
                {
                    playerCharacter.PartyId = partyId;
                    _playerCharacters[characterId] = playerCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetPlayerCharacterGuildId(string characterId, int guildId)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _playerCharacters.TryGetValue(characterId, out PlayerCharacterData playerCharacter))
                {
                    playerCharacter.GuildId = guildId;
                    _playerCharacters[characterId] = playerCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetPlayerCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _playerCharacters.TryGetValue(characterId, out PlayerCharacterData playerCharacter))
                {
                    playerCharacter.GuildId = guildId;
                    playerCharacter.GuildRole = guildRole;
                    _playerCharacters[characterId] = playerCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetPlayerCharacterSelectableWeaponSets(string characterId, List<EquipWeapons> selectableWeaponSets)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _playerCharacters.TryGetValue(characterId, out PlayerCharacterData playerCharacter))
                {
                    playerCharacter.SelectableWeaponSets = selectableWeaponSets ?? new List<EquipWeapons>();
                    _playerCharacters[characterId] = playerCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetPlayerCharacterEquipItems(string characterId, List<CharacterItem> equipItems)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _playerCharacters.TryGetValue(characterId, out PlayerCharacterData playerCharacter))
                {
                    playerCharacter.EquipItems = equipItems ?? new List<CharacterItem>();
                    _playerCharacters[characterId] = playerCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetPlayerCharacterNonEquipItems(string characterId, List<CharacterItem> nonEquipItems)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _playerCharacters.TryGetValue(characterId, out PlayerCharacterData playerCharacter))
                {
                    playerCharacter.NonEquipItems = nonEquipItems ?? new List<CharacterItem>();
                    _playerCharacters[characterId] = playerCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

public UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter)
{
    if (string.IsNullOrEmpty(playerCharacter.id))
        return UniTask.FromResult(false);

    lock (_lock)
    {
        _socialCharacters[playerCharacter.id] = playerCharacter;
    }

    return UniTask.FromResult(true);
}

        public UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _socialCharacters.TryGetValue(characterId, out SocialCharacterData socialCharacter))
                {
                    return UniTask.FromResult(new DatabaseCacheResult<SocialCharacterData>(socialCharacter));
                }
            }

            return UniTask.FromResult(new DatabaseCacheResult<SocialCharacterData>());
        }

        public UniTask<bool> RemoveSocialCharacter(string characterId)
        {
            bool removed = false;

            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId))
                    removed = _socialCharacters.Remove(characterId);
            }

            return UniTask.FromResult(removed);
        }

        public UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _socialCharacters.TryGetValue(characterId, out SocialCharacterData socialCharacter))
                {
                    socialCharacter.partyId = partyId;
                    _socialCharacters[characterId] = socialCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _socialCharacters.TryGetValue(characterId, out SocialCharacterData socialCharacter))
                {
                    socialCharacter.guildId = guildId;
                    _socialCharacters[characterId] = socialCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _socialCharacters.TryGetValue(characterId, out SocialCharacterData socialCharacter))
                {
                    socialCharacter.guildId = guildId;
                    socialCharacter.guildRole = guildRole;
                    _socialCharacters[characterId] = socialCharacter;
                    return UniTask.FromResult(true);
                }
            }

            return UniTask.FromResult(false);
        }

        public UniTask<bool> SetBuilding(string channel, string mapName, BuildingSaveData building)
        {
            if (building == null || string.IsNullOrEmpty(building.Id))
                return UniTask.FromResult(false);

            string mapKey = GetMapKey(channel, mapName);

            lock (_lock)
            {
                if (!_buildingsByMap.TryGetValue(mapKey, out Dictionary<string, BuildingSaveData> buildings))
                {
                    buildings = new Dictionary<string, BuildingSaveData>();
                    _buildingsByMap[mapKey] = buildings;
                }

                buildings[building.Id] = building;
            }

            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<BuildingSaveData>> GetBuilding(string channel, string mapName, string buildingId)
        {
            string mapKey = GetMapKey(channel, mapName);

            lock (_lock)
            {
                if (_buildingsByMap.TryGetValue(mapKey, out Dictionary<string, BuildingSaveData> buildings) &&
                    !string.IsNullOrEmpty(buildingId) &&
                    buildings.TryGetValue(buildingId, out BuildingSaveData building))
                {
                    return UniTask.FromResult(new DatabaseCacheResult<BuildingSaveData>(building));
                }
            }

            return UniTask.FromResult(new DatabaseCacheResult<BuildingSaveData>());
        }

        public UniTask<bool> RemoveBuilding(string channel, string mapName, string buildingId)
        {
            bool removed = false;
            string mapKey = GetMapKey(channel, mapName);

            lock (_lock)
            {
                if (_buildingsByMap.TryGetValue(mapKey, out Dictionary<string, BuildingSaveData> buildings) &&
                    !string.IsNullOrEmpty(buildingId))
                {
                    removed = buildings.Remove(buildingId);
                }
            }

            return UniTask.FromResult(removed);
        }

        public UniTask<bool> SetBuildings(string channel, string mapName, IEnumerable<BuildingSaveData> buildings)
        {
            string mapKey = GetMapKey(channel, mapName);
            var newMap = new Dictionary<string, BuildingSaveData>();

            if (buildings != null)
            {
                foreach (BuildingSaveData building in buildings)
                {
                    if (building != null && !string.IsNullOrEmpty(building.Id))
                        newMap[building.Id] = building;
                }
            }

            lock (_lock)
            {
                _buildingsByMap[mapKey] = newMap;
            }

            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<IEnumerable<BuildingSaveData>>> GetBuildings(string channel, string mapName)
        {
            string mapKey = GetMapKey(channel, mapName);

            lock (_lock)
            {
                if (_buildingsByMap.TryGetValue(mapKey, out Dictionary<string, BuildingSaveData> buildings))
                {
                    return UniTask.FromResult(
                        new DatabaseCacheResult<IEnumerable<BuildingSaveData>>(
                            new List<BuildingSaveData>(buildings.Values)));
                }
            }

            return UniTask.FromResult(new DatabaseCacheResult<IEnumerable<BuildingSaveData>>());
        }

        public UniTask<bool> RemoveBuildings(string channel, string mapName)
        {
            bool removed = false;
            string mapKey = GetMapKey(channel, mapName);

            lock (_lock)
            {
                removed = _buildingsByMap.Remove(mapKey);
            }

            return UniTask.FromResult(removed);
        }

        public UniTask<bool> SetParty(PartyData party)
        {
            if (party == null)
                return UniTask.FromResult(false);

            lock (_lock)
            {
                _parties[party.id] = party;
            }

            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<PartyData>> GetParty(int id)
        {
            lock (_lock)
            {
                if (_parties.TryGetValue(id, out PartyData party))
                    return UniTask.FromResult(new DatabaseCacheResult<PartyData>(party));
            }

            return UniTask.FromResult(new DatabaseCacheResult<PartyData>());
        }

        public UniTask<bool> RemoveParty(int id)
        {
            bool removed = false;

            lock (_lock)
            {
                removed = _parties.Remove(id);
            }

            return UniTask.FromResult(removed);
        }

        public UniTask<bool> SetGuild(GuildData guild)
        {
            if (guild == null)
                return UniTask.FromResult(false);

            lock (_lock)
            {
                _guilds[guild.id] = guild;
            }

            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id)
        {
            lock (_lock)
            {
                if (_guilds.TryGetValue(id, out GuildData guild))
                    return UniTask.FromResult(new DatabaseCacheResult<GuildData>(guild));
            }

            return UniTask.FromResult(new DatabaseCacheResult<GuildData>());
        }

        public UniTask<bool> RemoveGuild(int id)
        {
            bool removed = false;

            lock (_lock)
            {
                removed = _guilds.Remove(id);
            }

            return UniTask.FromResult(removed);
        }

        public UniTask<bool> SetStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> items)
        {
            string storageKey = GetStorageKey(storageType, storageOwnerId);

            lock (_lock)
            {
                _storageItems[storageKey] = items ?? new List<CharacterItem>();
            }

            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageType storageType, string storageOwnerId)
        {
            string storageKey = GetStorageKey(storageType, storageOwnerId);

            lock (_lock)
            {
                if (_storageItems.TryGetValue(storageKey, out List<CharacterItem> items))
                    return UniTask.FromResult(new DatabaseCacheResult<List<CharacterItem>>(new List<CharacterItem>(items)));
            }

            return UniTask.FromResult(new DatabaseCacheResult<List<CharacterItem>>());
        }

        public UniTask<bool> RemoveStorageItems(StorageType storageType, string storageOwnerId)
        {
            bool removed = false;
            string storageKey = GetStorageKey(storageType, storageOwnerId);

            lock (_lock)
            {
                removed = _storageItems.Remove(storageKey);
            }

            return UniTask.FromResult(removed);
        }

        public UniTask<bool> SetSummonBuffs(string characterId, List<CharacterBuff> items)
        {
            if (string.IsNullOrEmpty(characterId))
                return UniTask.FromResult(false);

            lock (_lock)
            {
                _summonBuffs[characterId] = items ?? new List<CharacterBuff>();
            }

            return UniTask.FromResult(true);
        }

        public UniTask<DatabaseCacheResult<List<CharacterBuff>>> GetSummonBuffs(string characterId)
        {
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId) &&
                    _summonBuffs.TryGetValue(characterId, out List<CharacterBuff> items))
                {
                    return UniTask.FromResult(new DatabaseCacheResult<List<CharacterBuff>>(new List<CharacterBuff>(items)));
                }
            }

            return UniTask.FromResult(new DatabaseCacheResult<List<CharacterBuff>>());
        }

        public UniTask<bool> RemoveSummonBuffs(string characterId)
        {
            bool removed = false;

            lock (_lock)
            {
                if (!string.IsNullOrEmpty(characterId))
                    removed = _summonBuffs.Remove(characterId);
            }

            return UniTask.FromResult(removed);
        }
    }
}