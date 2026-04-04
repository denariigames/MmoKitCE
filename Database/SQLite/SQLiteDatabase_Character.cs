#if NET || NETCOREAPP
using Microsoft.Data.Sqlite;
#elif (UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE
using Mono.Data.Sqlite;
#endif

#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class SQLiteDatabase
    {
        private void FillCharacterAttributes(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterAttributes(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Attributes.Count; ++i)
                {
                    CreateCharacterAttribute(transaction, insertedIds, i, characterData.Id, characterData.Attributes[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing attributes of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterBuffs(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterBuffs(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Buffs.Count; ++i)
                {
                    CreateCharacterBuff(transaction, insertedIds, characterData.Id, characterData.Buffs[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing buffs of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterHotkeys(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterHotkeys(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Hotkeys.Count; ++i)
                {
                    CreateCharacterHotkey(transaction, insertedIds, characterData.Id, characterData.Hotkeys[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing hotkeys of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterItems(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            FillCharacterItems(transaction, characterData.Id, 
                characterData.SelectableWeaponSets, 
                characterData.EquipItems, 
                characterData.NonEquipItems);
        }

        private void FillCharacterItems(SqliteTransaction transaction,
            string characterId,
            IList<EquipWeapons> selectableWeaponSets,
            IList<CharacterItem> equipItems,
            IList<CharacterItem> nonEquipItems)
        {
            try
            {
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                if (nonEquipItems != null)
                {
                    DeleteCharacterItems(transaction, InventoryType.NonEquipItems, characterId);
                }
                if (equipItems != null)
                {
                    DeleteCharacterItems(transaction, InventoryType.EquipItems, characterId);
                }
                if (selectableWeaponSets != null)
                {
                    DeleteCharacterItems(transaction, InventoryType.EquipWeaponRight, characterId);
                    DeleteCharacterItems(transaction, InventoryType.EquipWeaponLeft, characterId);
                }
                if (nonEquipItems != null)
                {
                    for (i = 0; i < nonEquipItems.Count; ++i)
                    {
                        CreateCharacterNonEquipItem(transaction, insertedIds, i, characterId, nonEquipItems[i]);
                    }
                }
                if (equipItems != null)
                {
                    for (i = 0; i < equipItems.Count; ++i)
                    {
                        CreateCharacterEquipItem(transaction, insertedIds, i, characterId, equipItems[i]);
                    }
                }
                if (selectableWeaponSets != null)
                {
                    for (i = 0; i < selectableWeaponSets.Count; ++i)
                    {
                        CreateCharacterEquipWeapons(transaction, insertedIds, i, characterId, selectableWeaponSets[i]);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing items of character: " + characterId);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterQuests(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterQuests(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Quests.Count; ++i)
                {
                    CreateCharacterQuest(transaction, insertedIds, i, characterData.Id, characterData.Quests[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing quests of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterCurrencies(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            try
            {
                DeleteCharacterCurrencies(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Currencies.Count; ++i)
                {
                    CreateCharacterCurrency(transaction, insertedIds, i, characterData.Id, characterData.Currencies[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing currencies of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
#endif
        }

        private void FillCharacterSkills(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterSkills(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Skills.Count; ++i)
                {
                    CreateCharacterSkill(transaction, insertedIds, i, characterData.Id, characterData.Skills[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skills of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterSkillUsages(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterSkillUsages(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.SkillUsages.Count; ++i)
                {
                    CreateCharacterSkillUsage(transaction, insertedIds, characterData.Id, characterData.SkillUsages[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skill usages of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterSummons(SqliteTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                DeleteCharacterSummons(transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Summons.Count; ++i)
                {
                    CreateCharacterSummon(transaction, insertedIds, i, characterData.Id, characterData.Summons[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skill usages of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterDataBooleans(SqliteTransaction transaction, string tableName, string characterId, IList<CharacterDataBoolean> list)
        {
            try
            {
                DeleteCharacterDataBooleans(transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    CreateCharacterDataBoolean(transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom boolean of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterDataInt32s(SqliteTransaction transaction, string tableName, string characterId, IList<CharacterDataInt32> list)
        {
            try
            {
                DeleteCharacterDataInt32s(transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    CreateCharacterDataInt32(transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom int32 of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterDataFloat32s(SqliteTransaction transaction, string tableName, string characterId, IList<CharacterDataFloat32> list)
        {
            try
            {
                DeleteCharacterDataFloat32s(transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    CreateCharacterDataFloat32(transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom float32 of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillSummonBuffs(SqliteTransaction transaction, string characterId, List<CharacterBuff> summonBuffs)
        {
            try
            {
                DeleteSummonBuff(transaction, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < summonBuffs.Count; ++i)
                {
                    CreateSummonBuff(transaction, insertedIds, characterId, summonBuffs[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing buffs of summon: " + characterId);
                LogException(LogTag, ex);
                throw;
            }
        }

        private void FillCharacterRelatesData(TransactionUpdateCharacterState state, SqliteTransaction transaction, IPlayerCharacterData characterData, List<CharacterBuff> summonBuffs)
        {
            if (state.Has(TransactionUpdateCharacterState.Attributes))
                FillCharacterAttributes(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Buffs))
                FillCharacterBuffs(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Hotkeys))
                FillCharacterHotkeys(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Items))
                FillCharacterItems(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Quests))
                FillCharacterQuests(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Currencies))
                FillCharacterCurrencies(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Skills))
                FillCharacterSkills(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.SkillUsages))
                FillCharacterSkillUsages(transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Summons))
                FillCharacterSummons(transaction, characterData);

#if !DISABLE_CUSTOM_CHARACTER_DATA
            if (state.Has(TransactionUpdateCharacterState.ServerCustomData))
            {
                FillCharacterDataBooleans(transaction, "character_server_boolean", characterData.Id, characterData.ServerBools);
                FillCharacterDataInt32s(transaction, "character_server_int32", characterData.Id, characterData.ServerInts);
                FillCharacterDataFloat32s(transaction, "character_server_float32", characterData.Id, characterData.ServerFloats);
            }
            if (state.Has(TransactionUpdateCharacterState.PrivateCustomData))
            {
                FillCharacterDataBooleans(transaction, "character_private_boolean", characterData.Id, characterData.PrivateBools);
                FillCharacterDataInt32s(transaction, "character_private_int32", characterData.Id, characterData.PrivateInts);
                FillCharacterDataFloat32s(transaction, "character_private_float32", characterData.Id, characterData.PrivateFloats);
            }
            if (state.Has(TransactionUpdateCharacterState.PublicCustomData))
            {
                FillCharacterDataBooleans(transaction, "character_public_boolean", characterData.Id, characterData.PublicBools);
                FillCharacterDataInt32s(transaction, "character_public_int32", characterData.Id, characterData.PublicInts);
                FillCharacterDataFloat32s(transaction, "character_public_float32", characterData.Id, characterData.PublicFloats);
            }
#endif

            if (state.Has(TransactionUpdateCharacterState.Mount))
                CreateOrUpdateCharacterMount(transaction, characterData.Id, characterData.Mount);

#if !DISABLE_CLASSIC_PK
            if (state.Has(TransactionUpdateCharacterState.Pk))
            {
                ExecuteNonQuery(transaction, @"INSERT INTO character_pk
                    (id, isPkOn, lastPkOnTime, pkPoint, consecutivePkKills, highestPkPoint, highestConsecutivePkKills) VALUES
                    (@id, @isPkOn, @lastPkOnTime, @pkPoint, @consecutivePkKills, @highestPkPoint, @highestConsecutivePkKills)
                    ON CONFLICT(id) DO UPDATE SET
                    isPkOn = @isPkOn,
                    lastPkOnTime = @lastPkOnTime,
                    pkPoint = @pkPoint,
                    consecutivePkKills = @consecutivePkKills,
                    highestPkPoint = @highestPkPoint,
                    highestConsecutivePkKills = @highestConsecutivePkKills",
                    new SqliteParameter("@id", characterData.Id),
                    new SqliteParameter("@isPkOn", characterData.IsPkOn),
                    new SqliteParameter("@lastPkOnTime", characterData.LastPkOnTime),
                    new SqliteParameter("@pkPoint", characterData.PkPoint),
                    new SqliteParameter("@consecutivePkKills", characterData.ConsecutivePkKills),
                    new SqliteParameter("@highestPkPoint", characterData.HighestPkPoint),
                    new SqliteParameter("@highestConsecutivePkKills", characterData.HighestConsecutivePkKills));
            }
#endif

            if (summonBuffs != null)
                FillSummonBuffs(transaction, characterData.Id, summonBuffs);
        }

        public override async UniTask CreateCharacter(string userId, IPlayerCharacterData character)
        {
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                ExecuteNonQuery(transaction, "INSERT INTO characters " +
                    "(id, userId, dataId, entityId, factionId, characterName, level, exp, currentHp, currentMp, currentStamina, currentFood, currentWater, equipWeaponSet, statPoint, skillPoint, gold, currentChannel, currentMapName, currentPositionX, currentPositionY, currentPositionZ, currentRotationX, currentRotationY, currentRotationZ, currentSafeArea, respawnMapName, respawnPositionX, respawnPositionY, respawnPositionZ, iconDataId, frameDataId, backgroundDataId, titleDataId) VALUES " +
                    "(@id, @userId, @dataId, @entityId, @factionId, @characterName, @level, @exp, @currentHp, @currentMp, @currentStamina, @currentFood, @currentWater, @equipWeaponSet, @statPoint, @skillPoint, @gold, @currentChannel, @currentMapName, @currentPositionX, @currentPositionY, @currentPositionZ, @currentRotationX, @currentRotationY, @currentRotationZ, @currentSafeArea, @respawnMapName, @respawnPositionX, @respawnPositionY, @respawnPositionZ, @iconDataId, @frameDataId, @backgroundDataId, @titleDataId)",
                    new SqliteParameter("@id", character.Id),
                    new SqliteParameter("@userId", userId),
                    new SqliteParameter("@dataId", character.DataId),
                    new SqliteParameter("@entityId", character.EntityId),
                    new SqliteParameter("@factionId", character.FactionId),
                    new SqliteParameter("@characterName", character.CharacterName),
                    new SqliteParameter("@level", character.Level),
                    new SqliteParameter("@exp", character.Exp),
                    new SqliteParameter("@currentHp", character.CurrentHp),
                    new SqliteParameter("@currentMp", character.CurrentMp),
                    new SqliteParameter("@currentStamina", character.CurrentStamina),
                    new SqliteParameter("@currentFood", character.CurrentFood),
                    new SqliteParameter("@currentWater", character.CurrentWater),
                    new SqliteParameter("@equipWeaponSet", character.EquipWeaponSet),
                    new SqliteParameter("@statPoint", character.StatPoint),
                    new SqliteParameter("@skillPoint", character.SkillPoint),
                    new SqliteParameter("@gold", character.Gold),
                    new SqliteParameter("@currentChannel", string.Empty),
                    new SqliteParameter("@currentMapName", character.CurrentMapName),
                    new SqliteParameter("@currentPositionX", character.CurrentPosition.x),
                    new SqliteParameter("@currentPositionY", character.CurrentPosition.y),
                    new SqliteParameter("@currentPositionZ", character.CurrentPosition.z),
                    new SqliteParameter("@currentRotationX", character.CurrentRotation.x),
                    new SqliteParameter("@currentRotationY", character.CurrentRotation.y),
                    new SqliteParameter("@currentRotationZ", character.CurrentRotation.z),
                    new SqliteParameter("@currentSafeArea", string.Empty),
#if !DISABLE_DIFFER_MAP_RESPAWNING
                    new SqliteParameter("@respawnMapName", character.RespawnMapName),
                    new SqliteParameter("@respawnPositionX", character.RespawnPosition.x),
                    new SqliteParameter("@respawnPositionY", character.RespawnPosition.y),
                    new SqliteParameter("@respawnPositionZ", character.RespawnPosition.z),
#endif
                    new SqliteParameter("@iconDataId", character.IconDataId),
                    new SqliteParameter("@frameDataId", character.FrameDataId),
                    new SqliteParameter("@backgroundDataId", character.BackgroundDataId),
                    new SqliteParameter("@titleDataId", character.TitleDataId));
                TransactionUpdateCharacterState state = TransactionUpdateCharacterState.All;
                FillCharacterRelatesData(state, transaction, character, null);
                if (onCreateCharacter != null)
                    await onCreateCharacter.Invoke(_connection, transaction, userId, character);
                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while create character: " + character.Id);
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            transaction.Dispose();
        }

        private bool GetCharacter(SqliteDataReader reader, out PlayerCharacterData result)
        {
            if (reader.Read())
            {
                result = new PlayerCharacterData();
                result.Id = reader.GetString(0);
                result.UserId = reader.GetString(1);
                result.DataId = reader.GetInt32(2);
                result.EntityId = reader.GetInt32(3);
                result.FactionId = reader.GetInt32(4);
                result.CharacterName = reader.GetString(5);
                result.Level = reader.GetInt32(6);
                result.Exp = reader.GetInt32(7);
                result.CurrentHp = reader.GetInt32(8);
                result.CurrentMp = reader.GetInt32(9);
                result.CurrentStamina = reader.GetInt32(10);
                result.CurrentFood = reader.GetInt32(11);
                result.CurrentWater = reader.GetInt32(12);
                result.EquipWeaponSet = reader.GetByte(13);
                result.StatPoint = reader.GetFloat(14);
                result.SkillPoint = reader.GetFloat(15);
                result.Gold = reader.GetInt32(16);
                result.PartyId = reader.GetInt32(17);
                result.GuildId = reader.GetInt32(18);
                result.GuildRole = reader.GetByte(19);
                result.CurrentChannel = reader.GetString(20);
                result.CurrentMapName = reader.GetString(21);
                result.CurrentPosition = new Vec3(reader.GetFloat(22), reader.GetFloat(23), reader.GetFloat(24));
                result.CurrentRotation = new Vec3(reader.GetFloat(25), reader.GetFloat(26), reader.GetFloat(27));
                result.CurrentSafeArea = reader.GetString(28);
#if !DISABLE_DIFFER_MAP_RESPAWNING
                result.RespawnMapName = reader.GetString(29);
                result.RespawnPosition = new Vec3(reader.GetFloat(30), reader.GetFloat(31), reader.GetFloat(32));
#endif
                result.IconDataId = reader.GetInt32(33);
                result.FrameDataId = reader.GetInt32(34);
                result.BackgroundDataId = reader.GetInt32(35);
                result.TitleDataId = reader.GetInt32(36);
                result.Reputation = reader.GetInt32(37);
                result.LastDeadTime = reader.GetInt64(38);
                result.UnmuteTime = reader.GetInt64(39);
                result.LastUpdate = ((System.DateTimeOffset)System.DateTime.SpecifyKind(reader.GetDateTime(40), System.DateTimeKind.Utc)).ToUnixTimeSeconds();
#if !DISABLE_CLASSIC_PK
                if (!reader.IsDBNull(41))
                    result.IsPkOn = reader.GetBoolean(41);
                if (!reader.IsDBNull(42))
                    result.LastPkOnTime = reader.GetInt64(42);
                if (!reader.IsDBNull(43))
                    result.PkPoint = reader.GetInt32(43);
                if (!reader.IsDBNull(44))
                    result.ConsecutivePkKills = reader.GetInt32(44);
                if (!reader.IsDBNull(45))
                    result.HighestPkPoint = reader.GetInt32(45);
                if (!reader.IsDBNull(46))
                    result.HighestConsecutivePkKills = reader.GetInt32(46);
#endif
                CharacterMount mount = new CharacterMount();
                if (!reader.IsDBNull(47))
                    mount.type = (MountType)reader.GetInt16(47);
                if (!reader.IsDBNull(48))
                    mount.sourceId = reader.GetString(48);
                if (!reader.IsDBNull(49))
                    mount.mountRemainsDuration = reader.GetFloat(49);
                if (!reader.IsDBNull(50))
                    mount.level = reader.GetInt32(50);
                if (!reader.IsDBNull(51))
                    mount.currentHp = reader.GetInt32(51);
                result.Mount = mount;
                return true;
            }
            result = null;
            return false;
        }

        public override async UniTask<PlayerCharacterData> GetCharacter(
            string id,
            bool withEquipWeapons = true,
            bool withAttributes = true,
            bool withSkills = true,
            bool withSkillUsages = true,
            bool withBuffs = true,
            bool withEquipItems = true,
            bool withNonEquipItems = true,
            bool withSummons = true,
            bool withHotkeys = true,
            bool withQuests = true,
            bool withCurrencies = true,
            bool withServerCustomData = true,
            bool withPrivateCustomData = true,
            bool withPublicCustomData = true)
        {
            PlayerCharacterData result = null;
            ExecuteReader((reader) =>
            {
                GetCharacter(reader, out result);
            }, @"SELECT
                c.id, c.userId, c.dataId, c.entityId, c.factionId, c.characterName, c.level, c.exp,
                c.currentHp, c.currentMp, c.currentStamina, c.currentFood, c.currentWater,
                c.equipWeaponSet, c.statPoint, c.skillPoint, c.gold, c.partyId, c.guildId, c.guildRole,
                c.currentChannel,
                c.currentMapName, c.currentPositionX, c.currentPositionY, c.currentPositionZ, c.currentRotationX, currentRotationY, currentRotationZ,
                c.currentSafeArea,
                c.respawnMapName, c.respawnPositionX, c.respawnPositionY, c.respawnPositionZ,
                c.iconDataId, c.frameDataId, c.backgroundDataId, c.titleDataId, c.reputation, c.lastDeadTime, c.unmuteTime, c.updateAt,
                cpk.isPkOn, cpk.lastPkOnTime, cpk.pkPoint, cpk.consecutivePkKills, cpk.highestPkPoint, cpk.highestConsecutivePkKills,
                cmnt.type, cmnt.sourceId, cmnt.mountRemainsDuration, cmnt.level, cmnt.currentHp
                FROM characters AS c 
                LEFT JOIN character_pk AS cpk ON c.id = cpk.id
                LEFT JOIN charactermount AS cmnt ON c.id = cmnt.id
                WHERE c.id=@id LIMIT 1",
                new SqliteParameter("@id", id));
            // Found character, then read its relates data
            if (result != null)
            {
                List<EquipWeapons> selectableWeaponSets = new List<EquipWeapons>();
                List<CharacterAttribute> attributes = new List<CharacterAttribute>();
                List<CharacterSkill> skills = new List<CharacterSkill>();
                List<CharacterSkillUsage> skillUsages = new List<CharacterSkillUsage>();
                List<CharacterBuff> buffs = new List<CharacterBuff>();
                List<CharacterItem> equipItems = new List<CharacterItem>();
                List<CharacterItem> nonEquipItems = new List<CharacterItem>();
                List<CharacterSummon> summons = new List<CharacterSummon>();
                List<CharacterHotkey> hotkeys = new List<CharacterHotkey>();
                List<CharacterQuest> quests = new List<CharacterQuest>();
                List<CharacterCurrency> currencies = new List<CharacterCurrency>();

                List<CharacterDataBoolean> serverBools = new List<CharacterDataBoolean>();
                List<CharacterDataInt32> serverInts = new List<CharacterDataInt32>();
                List<CharacterDataFloat32> serverFloats = new List<CharacterDataFloat32>();

                List<CharacterDataBoolean> privateBools = new List<CharacterDataBoolean>();
                List<CharacterDataInt32> privateInts = new List<CharacterDataInt32>();
                List<CharacterDataFloat32> privateFloats = new List<CharacterDataFloat32>();

                List<CharacterDataBoolean> publicBools = new List<CharacterDataBoolean>();
                List<CharacterDataInt32> publicInts = new List<CharacterDataInt32>();
                List<CharacterDataFloat32> publicFloats = new List<CharacterDataFloat32>();

                // Read data
                if (withEquipWeapons)
                    ReadCharacterEquipWeapons(id, selectableWeaponSets);
                if (withAttributes)
                    ReadCharacterAttributes(id, attributes);
                if (withSkills)
                    ReadCharacterSkills(id, skills);
                if (withSkillUsages)
                    ReadCharacterSkillUsages(id, skillUsages);
                if (withBuffs)
                    ReadCharacterBuffs(id, buffs);
                if (withEquipItems)
                    ReadCharacterEquipItems(id, equipItems);
                if (withNonEquipItems)
                    ReadCharacterNonEquipItems(id, nonEquipItems);
                if (withSummons)
                    ReadCharacterSummons(id, summons);
                if (withHotkeys)
                    ReadCharacterHotkeys(id, hotkeys);
                if (withQuests)
                    ReadCharacterQuests(id, quests);
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
                if (withCurrencies)
                    ReadCharacterCurrencies(id, currencies);
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
                if (withServerCustomData)
                {
                    ReadCharacterDataBooleans("character_server_boolean", id, serverBools);
                    ReadCharacterDataInt32s("character_server_int32", id, serverInts);
                    ReadCharacterDataFloat32s("character_server_float32", id, serverFloats);
                }
                if (withPrivateCustomData)
                {
                    ReadCharacterDataBooleans("character_private_boolean", id, privateBools);
                    ReadCharacterDataInt32s("character_private_int32", id, privateInts);
                    ReadCharacterDataFloat32s("character_private_float32", id, privateFloats);
                }
                if (withPublicCustomData)
                {
                    ReadCharacterDataBooleans("character_public_boolean", id, publicBools);
                    ReadCharacterDataInt32s("character_public_int32", id, publicInts);
                    ReadCharacterDataFloat32s("character_public_float32", id, publicFloats);
                }
#endif
                // Assign read data
                if (withEquipWeapons)
                    result.SelectableWeaponSets = selectableWeaponSets;
                if (withAttributes)
                    result.Attributes = attributes;
                if (withSkills)
                    result.Skills = skills;
                if (withSkillUsages)
                    result.SkillUsages = skillUsages;
                if (withBuffs)
                    result.Buffs = buffs;
                if (withEquipItems)
                    result.EquipItems = equipItems;
                if (withNonEquipItems)
                    result.NonEquipItems = nonEquipItems;
                if (withSummons)
                    result.Summons = summons;
                if (withHotkeys)
                    result.Hotkeys = hotkeys;
                if (withQuests)
                    result.Quests = quests;
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
                if (withCurrencies)
                    result.Currencies = currencies;
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
                if (withServerCustomData)
                {
                    result.ServerBools = serverBools;
                    result.ServerInts = serverInts;
                    result.ServerFloats = serverFloats;
                }
                if (withPrivateCustomData)
                {
                    result.PrivateBools = privateBools;
                    result.PrivateInts = privateInts;
                    result.PrivateFloats = privateFloats;
                }
                if (withPublicCustomData)
                {
                    result.PublicBools = publicBools;
                    result.PublicInts = publicInts;
                    result.PublicFloats = publicFloats;
                }
#endif
                if (onGetCharacter != null)
                {
                    result = await onGetCharacter.Invoke(
                        result,
                        withEquipWeapons,
                        withAttributes,
                        withSkills,
                        withSkillUsages,
                        withBuffs,
                        withEquipItems,
                        withNonEquipItems,
                        withSummons,
                        withHotkeys,
                        withQuests,
                        withCurrencies,
                        withServerCustomData,
                        withPrivateCustomData,
                        withPublicCustomData);
                }
            }
            return result;
        }

        public override async UniTask<List<PlayerCharacterData>> GetCharacters(string userId)
        {
            List<PlayerCharacterData> result = new List<PlayerCharacterData>();
            List<string> characterIds = new List<string>();
            ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    characterIds.Add(reader.GetString(0));
                }
            }, "SELECT id FROM characters WHERE userId=@userId ORDER BY updateAt DESC", new SqliteParameter("@userId", userId));
            foreach (string characterId in characterIds)
            {
                result.Add(await GetCharacter(characterId, true, false, false, false, false, true, false, false, false, false, false, false, false, true));
            }
            return result;
        }

        public override async UniTask UpdateCharacter(TransactionUpdateCharacterState state, IPlayerCharacterData character, List<CharacterBuff> summonBuffs, bool deleteStorageReservation)
        {
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                if (state.Has(TransactionUpdateCharacterState.Character))
                {
                    ExecuteNonQuery(transaction, "UPDATE characters SET " +
                        " dataId=@dataId," +
                        " entityId=@entityId," +
                        " factionId=@factionId," +
                        " characterName=@characterName," +
                        " level=@level," +
                        " exp=@exp," +
                        " currentHp=@currentHp," +
                        " currentMp=@currentMp," +
                        " currentStamina=@currentStamina," +
                        " currentFood=@currentFood," +
                        " currentWater=@currentWater," +
                        " equipWeaponSet=@equipWeaponSet," +
                        " statPoint=@statPoint," +
                        " skillPoint=@skillPoint," +
                        " gold=@gold," +
                        " currentChannel=@currentChannel," +
                        " currentMapName=@currentMapName," +
                        " currentPositionX=@currentPositionX," +
                        " currentPositionY=@currentPositionY," +
                        " currentPositionZ=@currentPositionZ," +
                        " currentRotationX=@currentRotationX," +
                        " currentRotationY=@currentRotationY," +
                        " currentRotationZ=@currentRotationZ," +
                        " currentSafeArea=@currentSafeArea," +
#if !DISABLE_DIFFER_MAP_RESPAWNING
                        " respawnMapName=@respawnMapName," +
                        " respawnPositionX=@respawnPositionX," +
                        " respawnPositionY=@respawnPositionY," +
                        " respawnPositionZ=@respawnPositionZ," +
#endif
                        " iconDataId=@iconDataId," +
                        " frameDataId=@frameDataId," +
                        " backgroundDataId=@backgroundDataId," +
                        " titleDataId=@titleDataId," +
                        " reputation=@reputation," +
                        " lastDeadTime=@lastDeadTime," +
                        " unmuteTime=@unmuteTime," +
                        " updateAt=datetime('now', 'utc')" +
                        " WHERE id=@id",
                        new SqliteParameter("@dataId", character.DataId),
                        new SqliteParameter("@entityId", character.EntityId),
                        new SqliteParameter("@factionId", character.FactionId),
                        new SqliteParameter("@characterName", character.CharacterName),
                        new SqliteParameter("@level", character.Level),
                        new SqliteParameter("@exp", character.Exp),
                        new SqliteParameter("@currentHp", character.CurrentHp),
                        new SqliteParameter("@currentMp", character.CurrentMp),
                        new SqliteParameter("@currentStamina", character.CurrentStamina),
                        new SqliteParameter("@currentFood", character.CurrentFood),
                        new SqliteParameter("@currentWater", character.CurrentWater),
                        new SqliteParameter("@equipWeaponSet", character.EquipWeaponSet),
                        new SqliteParameter("@statPoint", character.StatPoint),
                        new SqliteParameter("@skillPoint", character.SkillPoint),
                        new SqliteParameter("@gold", character.Gold),
                        new SqliteParameter("@currentChannel", character.CurrentChannel),
                        new SqliteParameter("@currentMapName", character.CurrentMapName),
                        new SqliteParameter("@currentPositionX", character.CurrentPosition.x),
                        new SqliteParameter("@currentPositionY", character.CurrentPosition.y),
                        new SqliteParameter("@currentPositionZ", character.CurrentPosition.z),
                        new SqliteParameter("@currentRotationX", character.CurrentRotation.x),
                        new SqliteParameter("@currentRotationY", character.CurrentRotation.y),
                        new SqliteParameter("@currentRotationZ", character.CurrentRotation.z),
                        new SqliteParameter("@currentSafeArea", character.CurrentSafeArea),
#if !DISABLE_DIFFER_MAP_RESPAWNING
                        new SqliteParameter("@respawnMapName", character.RespawnMapName),
                        new SqliteParameter("@respawnPositionX", character.RespawnPosition.x),
                        new SqliteParameter("@respawnPositionY", character.RespawnPosition.y),
                        new SqliteParameter("@respawnPositionZ", character.RespawnPosition.z),
#endif
                        new SqliteParameter("@iconDataId", character.IconDataId),
                        new SqliteParameter("@frameDataId", character.FrameDataId),
                        new SqliteParameter("@backgroundDataId", character.BackgroundDataId),
                        new SqliteParameter("@titleDataId", character.TitleDataId),
                        new SqliteParameter("@reputation", character.Reputation),
                        new SqliteParameter("@lastDeadTime", character.LastDeadTime),
                        new SqliteParameter("@unmuteTime", character.UnmuteTime),
                        new SqliteParameter("@id", character.Id));
                }
                FillCharacterRelatesData(state, transaction, character, summonBuffs);
                if (deleteStorageReservation)
                {
                    ExecuteNonQuery(transaction, "DELETE FROM storage_reservation WHERE reserverId=@reserverId",
                        new SqliteParameter("@reserverId", character.Id));
                }
                if (onUpdateCharacter != null)
                    await onUpdateCharacter.Invoke(_connection, transaction, state, character);
                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while update character: " + character.Id);
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            transaction.Dispose();
        }

        public override async UniTask DeleteCharacter(string userId, string id)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM characters WHERE id=@id AND userId=@userId",
                new SqliteParameter("@id", id),
                new SqliteParameter("@userId", userId));
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                SqliteTransaction transaction = _connection.BeginTransaction();
                try
                {
                    ExecuteNonQuery(transaction, "DELETE FROM characters WHERE id=@characterId", new SqliteParameter("@characterId", id));
                    ExecuteNonQuery(transaction, "DELETE FROM character_pk WHERE id=@characterId", new SqliteParameter("@characterId", id));
                    ExecuteNonQuery(transaction, "DELETE FROM friend WHERE characterId1 LIKE @characterId OR characterId2 LIKE @characterId", new SqliteParameter("@characterId", id));
                    DeleteCharacterAttributes(transaction, id);
                    DeleteCharacterCurrencies(transaction, id);
                    DeleteCharacterBuffs(transaction, id);
                    DeleteCharacterHotkeys(transaction, id);
                    DeleteCharacterItems(transaction, id);
                    DeleteCharacterQuests(transaction, id);
                    DeleteCharacterSkills(transaction, id);
                    DeleteCharacterSkillUsages(transaction, id);
                    DeleteCharacterSummons(transaction, id);
                    DeleteCharacterMount(transaction, id);

                    DeleteCharacterDataBooleans(transaction, "character_server_boolean", id);
                    DeleteCharacterDataInt32s(transaction, "character_server_int32", id);
                    DeleteCharacterDataFloat32s(transaction, "character_server_float32", id);

                    DeleteCharacterDataBooleans(transaction, "character_private_boolean", id);
                    DeleteCharacterDataInt32s(transaction, "character_private_int32", id);
                    DeleteCharacterDataFloat32s(transaction, "character_private_float32", id);

                    DeleteCharacterDataBooleans(transaction, "character_public_boolean", id);
                    DeleteCharacterDataInt32s(transaction, "character_public_int32", id);
                    DeleteCharacterDataFloat32s(transaction, "character_public_float32", id);

                    if (onDeleteCharacter != null)
                        await onDeleteCharacter.Invoke(_connection, transaction, userId, id);
                    transaction.Commit();
                }
                catch (System.Exception ex)
                {
                    LogError(LogTag, "Transaction, Error occurs while deleting character: " + id);
                    LogException(LogTag, ex);
                    transaction.Rollback();
                }
                transaction.Dispose();
            }
        }

        public override UniTask<long> FindCharacterName(string characterName)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM characters WHERE characterName LIKE @characterName",
                new SqliteParameter("@characterName", characterName));
            return new UniTask<long>(result != null ? (long)result : 0);
        }

        public override UniTask<string> GetIdByCharacterName(string characterName)
        {
            object result = ExecuteScalar("SELECT id FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new SqliteParameter("@characterName", characterName));
            return new UniTask<string>(result != null ? (string)result : string.Empty);
        }

        public override UniTask<string> GetUserIdByCharacterName(string characterName)
        {
            object result = ExecuteScalar("SELECT userId FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new SqliteParameter("@characterName", characterName));
            return new UniTask<string>(result != null ? (string)result : string.Empty);
        }

        public override UniTask<List<SocialCharacterData>> FindCharacters(string finderId, string characterName, int skip, int limit)
        {
            string excludeIdsQuery = "(id!='" + finderId + "'";
            // Exclude friend, requested characters
            ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    excludeIdsQuery += " AND id!='" + reader.GetString(0) + "'";
                }
            }, "SELECT characterId2 FROM friend WHERE characterId1='" + finderId + "'");
            excludeIdsQuery += ")";
            List<SocialCharacterData> result = new List<SocialCharacterData>();
            ExecuteReader((reader) =>
            {
                SocialCharacterData socialCharacterData;
                while (reader.Read())
                {
                    // Get some required data, other data will be set at server side
                    socialCharacterData = new SocialCharacterData();
                    socialCharacterData.id = reader.GetString(0);
                    socialCharacterData.dataId = reader.GetInt32(1);
                    socialCharacterData.characterName = reader.GetString(2);
                    socialCharacterData.level = reader.GetInt32(3);
                    result.Add(socialCharacterData);
                }
            }, "SELECT id, dataId, characterName, level FROM characters WHERE characterName LIKE @characterName AND " + excludeIdsQuery + " LIMIT " + skip + ", " + limit,
                new SqliteParameter("@characterName", "%" + characterName + "%"));
            return new UniTask<List<SocialCharacterData>>(result);
        }

        public override UniTask CreateFriend(string id1, string id2, byte state)
        {
            SqliteTransaction transaction = _connection.BeginTransaction();
            try
            {
                ExecuteNonQuery(transaction, "DELETE FROM friend WHERE " +
                   "characterId1 LIKE @characterId1 AND " +
                   "characterId2 LIKE @characterId2",
                   new SqliteParameter("@characterId1", id1),
                   new SqliteParameter("@characterId2", id2));
                ExecuteNonQuery(transaction, "INSERT INTO friend " +
                    "(characterId1, characterId2, state) VALUES " +
                    "(@characterId1, @characterId2, @state)",
                    new SqliteParameter("@characterId1", id1),
                    new SqliteParameter("@characterId2", id2),
                    new SqliteParameter("@state", state));
                transaction.Commit();
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while creating friend: " + id1 + " " + id2);
                LogException(LogTag, ex);
                transaction.Rollback();
            }
            transaction.Dispose();
            return new UniTask();
        }

        public override UniTask DeleteFriend(string id1, string id2)
        {
            ExecuteNonQuery("DELETE FROM friend WHERE " +
                "characterId1 LIKE @characterId1 AND " +
                "characterId2 LIKE @characterId2",
                new SqliteParameter("@characterId1", id1),
                new SqliteParameter("@characterId2", id2));
            return new UniTask();
        }

        public override UniTask<List<SocialCharacterData>> GetFriends(string id, bool readById2, byte state, int skip, int limit)
        {
            List<SocialCharacterData> result = new List<SocialCharacterData>();
            List<string> characterIds = new List<string>();
            if (readById2)
            {
                ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        characterIds.Add(reader.GetString(0));
                    }
                }, "SELECT characterId1 FROM friend WHERE characterId2=@id AND state=" + state + " LIMIT " + skip + ", " + limit,
                    new SqliteParameter("@id", id));
            }
            else
            {
                ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        characterIds.Add(reader.GetString(0));
                    }
                }, "SELECT characterId2 FROM friend WHERE characterId1=@id AND state=" + state + " LIMIT " + skip + ", " + limit,
                    new SqliteParameter("@id", id));
            }
            SocialCharacterData socialCharacterData;
            foreach (string characterId in characterIds)
            {
                ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        // Get some required data, other data will be set at server side
                        socialCharacterData = new SocialCharacterData();
                        socialCharacterData.id = reader.GetString(0);
                        socialCharacterData.dataId = reader.GetInt32(1);
                        socialCharacterData.characterName = reader.GetString(2);
                        socialCharacterData.level = reader.GetInt32(3);
                        result.Add(socialCharacterData);
                    }
                }, "SELECT id, dataId, characterName, level FROM characters WHERE id LIKE @id",
                    new SqliteParameter("@id", characterId));
            }
            return new UniTask<List<SocialCharacterData>>(result);
        }

        public override UniTask<int> GetFriendRequestNotification(string characterId)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM friend WHERE characterId2=@characterId AND state=1",
                new SqliteParameter("@characterId", characterId));
            return new UniTask<int>((int)(long)result);
        }
    }
}
#endif