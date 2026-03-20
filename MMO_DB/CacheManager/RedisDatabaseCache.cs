// NOTE: Temporary disable Redis database cache for Unity project
#if !UNITY_2017_1_OR_NEWER
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;
using LiteNetLibManager;
#else
using Microsoft.Extensions.Logging;
#endif

namespace MultiplayerARPG.MMO
{
    public class RedisDatabaseCache : IDatabaseCache
    {
        private const string PLAYER_CHARACTER_KEY = "playerCharacter:";
        private const string SOCIAL_CHARACTER_KEY = "socialCharacter:";
        private const string BUILDING_KEY = "building:";
        private const string PARTY_KEY = "party:";
        private const string GUILD_KEY = "guild:";
        private const string STORAGE_ITEMS_KEY = "storageItems:";
        private const string SUMMON_BUFFS_KEY = "summonBuffs:";

        private readonly StackExchange.Redis.IDatabase _redis;
        private readonly StackExchange.Redis.IServer _server;
        private static TimeSpan s_cacheTTL = new TimeSpan(0, 15, 0);
        private static TimeSpan s_partyCacheTTL = new TimeSpan(0, 10, 0);
        private static TimeSpan s_guildCacheTTL = new TimeSpan(0, 5, 0);
        private static string s_connectionConfig = "localhost";
        private static string s_dbPrefix = "gamedb:";
#if !UNITY_2017_1_OR_NEWER
        private ILogger _logger;
#endif

#if UNITY_2017_1_OR_NEWER
        public RedisDatabaseCache()
#else
        public RedisDatabaseCache(ILogger<RedisDatabaseCache> logger)
#endif
        {
#if !UNITY_2017_1_OR_NEWER
            _logger = logger;
#endif
            // Json file read
            bool configFileFound = false;
            string configFolder = "./Config";
            string configFilePath = configFolder + "/redisConfig.json";
            RedisConfig config = new RedisConfig()
            {
                redisConnectionConfig = s_connectionConfig,
                redisDbPrefix = s_dbPrefix,
            };
            if (File.Exists(configFilePath))
            {
                string dataAsJson = File.ReadAllText(configFilePath);
                RedisConfig replacingConfig = JsonConvert.DeserializeObject<RedisConfig>(dataAsJson);
                if (replacingConfig.redisConnectionConfig != null)
                    config.redisConnectionConfig = replacingConfig.redisConnectionConfig;
                if (replacingConfig.redisDbPrefix != null)
                    config.redisDbPrefix = replacingConfig.redisDbPrefix;
                configFileFound = true;
            }

            s_connectionConfig = config.redisConnectionConfig;
            s_dbPrefix = config.redisDbPrefix;

            // Read configs from ENV
            string envVal;
            envVal = Environment.GetEnvironmentVariable("redisConnectionConfig");
            if (!string.IsNullOrEmpty(envVal))
                s_connectionConfig = envVal;
            envVal = Environment.GetEnvironmentVariable("redisDbPrefix");
            if (!string.IsNullOrEmpty(envVal))
                s_dbPrefix = envVal;

            if (!configFileFound)
            {
                // Write config file
                if (!Directory.Exists(configFolder))
                    Directory.CreateDirectory(configFolder);
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
            }

            LogInformation(nameof(RedisDatabaseCache), $"Connecting with config: {s_connectionConfig}, prefix: {s_dbPrefix}");
            var muxer = ConnectionMultiplexer.Connect(s_connectionConfig);
            _redis = muxer.GetDatabase();
            _server = muxer.GetServer(muxer.GetEndPoints().First());
        }

        public void LogInformation(string tag, string msg)
        {
#if UNITY_2017_1_OR_NEWER
            Logging.Log(tag, msg);
#else
            _logger.LogInformation(msg);
#endif
        }

        public void LogWarning(string tag, string msg)
        {
#if UNITY_2017_1_OR_NEWER
            Logging.LogWarning(tag, msg);
#else
            _logger.LogWarning(msg);
#endif
        }

        public void LogError(string tag, string msg)
        {
#if UNITY_2017_1_OR_NEWER
            Logging.LogError(tag, msg);
#else
            _logger.LogError(msg);
#endif
        }

        public void LogException(string tag, Exception ex)
        {
#if UNITY_2017_1_OR_NEWER
            Logging.LogException(tag, ex);
#else
            _logger.LogCritical(ex, string.Empty);
#endif
        }

        private static string GetPlayerCharacterKey(string id) => ZString.Concat(s_dbPrefix, PLAYER_CHARACTER_KEY, id);
        private static string GetSocialCharacterKey(string id) => ZString.Concat(s_dbPrefix, SOCIAL_CHARACTER_KEY, id);
        private static string GetBuildingKey(string channel, string mapName, string id) => ZString.Concat(s_dbPrefix, BUILDING_KEY, channel, ":", mapName, ":", id);
        private static string GetPartyKey(int id) => ZString.Concat(s_dbPrefix, PARTY_KEY, id.ToString());
        private static string GetGuildKey(int id) => ZString.Concat(s_dbPrefix, GUILD_KEY, id.ToString());
        private static string GetStorageItemsKey(StorageType type, string ownerId) => ZString.Concat(s_dbPrefix, STORAGE_ITEMS_KEY, ((int)type).ToString(), ":", ownerId);
        private static string GetSummonBuffsKey(string characterId) => ZString.Concat(s_dbPrefix, SUMMON_BUFFS_KEY, characterId);

        public async UniTask<bool> SetAsync<T>(string key, T value, TimeSpan? expiry)
        {
            try
            {
                byte[] data = MessagePackSerializer.Serialize(value, ContractlessStandardResolver.Options);
                return await _redis.StringSetAsync(key, data, expiry: expiry);
            }
            catch (Exception ex)
            {
                LogException(nameof(RedisDatabaseCache), ex);
                return false;
            }
        }

        public async UniTask<DatabaseCacheResult<T>> GetAsync<T>(string key)
        {
            try
            {
                RedisValue data = await _redis.StringGetAsync(key);
                if (!data.HasValue)
                    return new DatabaseCacheResult<T>();
                T value = MessagePackSerializer.Deserialize<T>(data, ContractlessStandardResolver.Options);
                return new DatabaseCacheResult<T>(value);
            }
            catch (Exception ex)
            {
                LogException(nameof(RedisDatabaseCache), ex);
                return new DatabaseCacheResult<T>();
            }
        }

        public async UniTask<bool> RemoveAsync(string key)
        {
            try
            {
                return await _redis.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                LogException(nameof(RedisDatabaseCache), ex);
                return false;
            }
        }

        public async UniTask<bool> SetAsync<T>(RedisKey key, T value, TimeSpan? expiry)
        {
            try
            {
                byte[] data = MessagePackSerializer.Serialize(value, ContractlessStandardResolver.Options);
                return await _redis.StringSetAsync(key, data, expiry: expiry);
            }
            catch (Exception ex)
            {
                LogException(nameof(RedisDatabaseCache), ex);
                return false;
            }
        }

        public async UniTask<DatabaseCacheResult<T>> GetAsync<T>(RedisKey key)
        {
            try
            {
                RedisValue data = await _redis.StringGetAsync(key);
                if (!data.HasValue)
                    return new DatabaseCacheResult<T>();
                T value = MessagePackSerializer.Deserialize<T>(data, ContractlessStandardResolver.Options);
                return new DatabaseCacheResult<T>(value);
            }
            catch (Exception ex)
            {
                LogException(nameof(RedisDatabaseCache), ex);
                return new DatabaseCacheResult<T>();
            }
        }

        public async UniTask<bool> RemoveAsync(RedisKey key)
        {
            try
            {
                return await _redis.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                LogException(nameof(RedisDatabaseCache), ex);
                return false;
            }
        }

        public async UniTask<bool> SetPlayerCharacter(PlayerCharacterData playerCharacter)
        {
            var prevPlayerCharacter = await GetPlayerCharacter(playerCharacter.Id);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                playerCharacter.PartyId = prevPlayerCharacterValue.PartyId;
                playerCharacter.GuildId = prevPlayerCharacterValue.GuildId;
                playerCharacter.GuildRole = prevPlayerCharacterValue.GuildRole;
            }
            return await SetAsync(GetPlayerCharacterKey(playerCharacter.Id), playerCharacter, s_cacheTTL);
        }
        public UniTask<DatabaseCacheResult<PlayerCharacterData>> GetPlayerCharacter(string characterId)
        {
            return GetAsync<PlayerCharacterData>(GetPlayerCharacterKey(characterId));
        }
        public UniTask<bool> RemovePlayerCharacter(string characterId)
        {
            return RemoveAsync(GetPlayerCharacterKey(characterId));
        }
        public async UniTask<bool> SetPlayerCharacterPartyId(string characterId, int partyId)
        {
            var prevPlayerCharacter = await GetPlayerCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.PartyId = partyId;
                return await SetAsync(GetPlayerCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }
        public async UniTask<bool> SetPlayerCharacterGuildId(string characterId, int guildId)
        {
            var prevPlayerCharacter = await GetPlayerCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.GuildId = guildId;
                return await SetAsync(GetPlayerCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }
        public async UniTask<bool> SetPlayerCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            var prevPlayerCharacter = await GetPlayerCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.GuildId = guildId;
                prevPlayerCharacterValue.GuildRole = guildRole;
                return await SetAsync(GetPlayerCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }

        public async UniTask<bool> SetPlayerCharacterSelectableWeaponSets(string characterId, List<EquipWeapons> selectableWeaponSets)
        {
            var prevPlayerCharacter = await GetPlayerCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.SelectableWeaponSets = selectableWeaponSets;
                return await SetAsync(GetPlayerCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }

        public async UniTask<bool> SetPlayerCharacterEquipItems(string characterId, List<CharacterItem> equipItems)
        {
            var prevPlayerCharacter = await GetPlayerCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.EquipItems = equipItems;
                return await SetAsync(GetPlayerCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }

        public async UniTask<bool> SetPlayerCharacterNonEquipItems(string characterId, List<CharacterItem> nonEquipItems)
        {
            var prevPlayerCharacter = await GetPlayerCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.NonEquipItems = nonEquipItems;
                return await SetAsync(GetPlayerCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }

        public async UniTask<bool> SetSocialCharacter(SocialCharacterData playerCharacter)
        {
            var prevPlayerCharacter = await GetSocialCharacter(playerCharacter.id);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                playerCharacter.partyId = prevPlayerCharacterValue.partyId;
                playerCharacter.guildId = prevPlayerCharacterValue.guildId;
                playerCharacter.guildRole = prevPlayerCharacterValue.guildRole;
            }
            return await SetAsync(GetSocialCharacterKey(playerCharacter.id), playerCharacter, s_cacheTTL);
        }
        public UniTask<DatabaseCacheResult<SocialCharacterData>> GetSocialCharacter(string characterId)
        {
            return GetAsync<SocialCharacterData>(GetSocialCharacterKey(characterId));
        }
        public UniTask<bool> RemoveSocialCharacter(string characterId)
        {
            return RemoveAsync(GetSocialCharacterKey(characterId));
        }
        public async UniTask<bool> SetSocialCharacterPartyId(string characterId, int partyId)
        {
            var prevPlayerCharacter = await GetSocialCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.partyId = partyId;
                return await SetAsync(GetSocialCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }
        public async UniTask<bool> SetSocialCharacterGuildId(string characterId, int guildId)
        {
            var prevPlayerCharacter = await GetSocialCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.guildId = guildId;
                return await SetAsync(GetSocialCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }
        public async UniTask<bool> SetSocialCharacterGuildIdAndRole(string characterId, int guildId, byte guildRole)
        {
            var prevPlayerCharacter = await GetSocialCharacter(characterId);
            if (prevPlayerCharacter.HasValue)
            {
                var prevPlayerCharacterValue = prevPlayerCharacter.Value;
                prevPlayerCharacterValue.guildId = guildId;
                prevPlayerCharacterValue.guildRole = guildRole;
                return await SetAsync(GetSocialCharacterKey(characterId), prevPlayerCharacterValue, s_cacheTTL);
            }
            return false;
        }

        public UniTask<bool> SetBuilding(string channel, string mapName, BuildingSaveData building)
        {
            return SetAsync(GetBuildingKey(channel, mapName, building.Id), building, s_cacheTTL);
        }
        public UniTask<DatabaseCacheResult<BuildingSaveData>> GetBuilding(string channel, string mapName, string buildingId)
        {
            return GetAsync<BuildingSaveData>(GetBuildingKey(channel, mapName, buildingId));
        }
        public UniTask<bool> RemoveBuilding(string channel, string mapName, string buildingId)
        {
            return RemoveAsync(GetBuildingKey(channel, mapName, buildingId));
        }

        public async UniTask<bool> SetBuildings(string channel, string mapName, IEnumerable<BuildingSaveData> buildings)
        {
            await RemoveBuildings(channel, mapName);
            bool result = true;
            foreach (var building in buildings)
            {
                if (!await SetBuilding(channel, mapName, building))
                    result = false;
            }
            return result;
        }
        public async UniTask<DatabaseCacheResult<IEnumerable<BuildingSaveData>>> GetBuildings(string channel, string mapName)
        {
            var keys = _server.Keys(pattern: GetBuildingKey(channel, mapName, "*"));
            List<BuildingSaveData> result = new List<BuildingSaveData>();
            foreach (var key in keys)
            {
                var entry = await GetAsync<BuildingSaveData>(key);
                if (!entry.HasValue)
                    continue;
                result.Add(entry.Value);
            }
            return new DatabaseCacheResult<IEnumerable<BuildingSaveData>>(result);
        }
        public async UniTask<bool> RemoveBuildings(string channel, string mapName)
        {
            var keys = _server.Keys(pattern: GetBuildingKey(channel, mapName, "*"));
            bool result = true;
            foreach (var key in keys)
            {
                if (!await RemoveAsync(key))
                    result = false;
            }
            return result;
        }

        public UniTask<bool> SetParty(PartyData party)
        {
            return SetAsync(GetPartyKey(party.id), party, s_partyCacheTTL);
        }
        public UniTask<DatabaseCacheResult<PartyData>> GetParty(int id)
        {
            return GetAsync<PartyData>(GetPartyKey(id));
        }
        public UniTask<bool> RemoveParty(int id)
        {
            return RemoveAsync(GetPartyKey(id));
        }

        public UniTask<bool> SetGuild(GuildData guild)
        {
            return SetAsync(GetGuildKey(guild.id), guild, s_guildCacheTTL);
        }
        public UniTask<DatabaseCacheResult<GuildData>> GetGuild(int id)
        {
            return GetAsync<GuildData>(GetGuildKey(id));
        }
        public UniTask<bool> RemoveGuild(int id)
        {
            return RemoveAsync(GetGuildKey(id));
        }

        public UniTask<bool> SetStorageItems(StorageType storageType, string storageOwnerId, List<CharacterItem> items)
        {
            return SetAsync(GetStorageItemsKey(storageType, storageOwnerId), items, s_cacheTTL);
        }
        public UniTask<DatabaseCacheResult<List<CharacterItem>>> GetStorageItems(StorageType storageType, string storageOwnerId)
        {
            return GetAsync<List<CharacterItem>>(GetStorageItemsKey(storageType, storageOwnerId));
        }
        public UniTask<bool> RemoveStorageItems(StorageType storageType, string storageOwnerId)
        {
            return RemoveAsync(GetStorageItemsKey(storageType, storageOwnerId));
        }

        public UniTask<bool> SetSummonBuffs(string characterId, List<CharacterBuff> buffs)
        {
            if (buffs == null)
                return UniTask.FromResult(true);
            return SetAsync(GetSummonBuffsKey(characterId), buffs, s_cacheTTL);
        }

        public UniTask<DatabaseCacheResult<List<CharacterBuff>>> GetSummonBuffs(string characterId)
        {
            return GetAsync<List<CharacterBuff>>(GetSummonBuffsKey(characterId));
        }

        public UniTask<bool> RemoveSummonBuffs(string characterId)
        {
            return RemoveAsync(GetSummonBuffsKey(characterId));
        }
    }
}
#endif