#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
using Cysharp.Threading.Tasks;
using MySqlConnector;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
    public partial class MySQLDatabase
    {
        private async UniTask FillCharacterAttributes(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                await DeleteCharacterAttributes(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Attributes.Count; ++i)
                {
                    await CreateCharacterAttribute(connection, transaction, insertedIds, characterData.Id, characterData.Attributes[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing attributes of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterBuffs(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                await DeleteCharacterBuffs(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Buffs.Count; ++i)
                {
                    await CreateCharacterBuff(connection, transaction, insertedIds, characterData.Id, characterData.Buffs[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing buffs of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterHotkeys(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                await DeleteCharacterHotkeys(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Hotkeys.Count; ++i)
                {
                    await CreateCharacterHotkey(connection, transaction, insertedIds, characterData.Id, characterData.Hotkeys[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing hotkeys of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterItems(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            await FillCharacterItems(connection, transaction, characterData.Id,
                characterData.SelectableWeaponSets,
                characterData.EquipItems,
                characterData.NonEquipItems);
        }

        private async UniTask FillCharacterItems(MySqlConnection connection,
            MySqlTransaction transaction,
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
                    await DeleteCharacterItems(connection, transaction, InventoryType.NonEquipItems, characterId);
                }
                if (equipItems != null)
                {
                    await DeleteCharacterItems(connection, transaction, InventoryType.EquipItems, characterId);
                }
                if (selectableWeaponSets != null)
                {
                    await DeleteCharacterItems(connection, transaction, InventoryType.EquipWeaponRight, characterId);
                    await DeleteCharacterItems(connection, transaction, InventoryType.EquipWeaponLeft, characterId);
                }
                if (nonEquipItems != null)
                {
                    for (i = 0; i < nonEquipItems.Count; ++i)
                    {
                        await CreateCharacterNonEquipItem(connection, transaction, insertedIds, i, characterId, nonEquipItems[i]);
                    }
                }
                if (equipItems != null)
                {
                    for (i = 0; i < equipItems.Count; ++i)
                    {
                        await CreateCharacterEquipItem(connection, transaction, insertedIds, i, characterId, equipItems[i]);
                    }
                }
                if (selectableWeaponSets != null)
                {
                    for (i = 0; i < selectableWeaponSets.Count; ++i)
                    {
                        await CreateCharacterEquipWeapons(connection, transaction, insertedIds, i, characterId, selectableWeaponSets[i]);
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

        private async UniTask FillCharacterQuests(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                await DeleteCharacterQuests(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Quests.Count; ++i)
                {
                    await CreateCharacterQuest(connection, transaction, insertedIds, characterData.Id, characterData.Quests[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing quests of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterCurrencies(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            try
            {
                await DeleteCharacterCurrencies(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Currencies.Count; ++i)
                {
                    await CreateCharacterCurrency(connection, transaction, insertedIds, characterData.Id, characterData.Currencies[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing currencies of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
#else
            await UniTask.Yield();
#endif
        }

        private async UniTask FillCharacterSkills(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                await DeleteCharacterSkills(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Skills.Count; ++i)
                {
                    await CreateCharacterSkill(connection, transaction, insertedIds, characterData.Id, characterData.Skills[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skills of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterSkillUsages(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                await DeleteCharacterSkillUsages(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.SkillUsages.Count; ++i)
                {
                    await CreateCharacterSkillUsage(connection, transaction, insertedIds, characterData.Id, characterData.SkillUsages[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skill usages of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterSummons(MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData)
        {
            try
            {
                await DeleteCharacterSummons(connection, transaction, characterData.Id);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < characterData.Summons.Count; ++i)
                {
                    await CreateCharacterSummon(connection, transaction, insertedIds, i, characterData.Id, characterData.Summons[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing skill usages of character: " + characterData.Id);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterDataBooleans(MySqlConnection connection, MySqlTransaction transaction, string tableName, string characterId, IList<CharacterDataBoolean> list)
        {
            try
            {
                await DeleteCharacterDataBooleans(connection, transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    await CreateCharacterDataBoolean(connection, transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom boolean of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterDataInt32s(MySqlConnection connection, MySqlTransaction transaction, string tableName, string characterId, IList<CharacterDataInt32> list)
        {
            try
            {
                await DeleteCharacterDataInt32s(connection, transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    await CreateCharacterDataInt32(connection, transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom int32 of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterDataFloat32s(MySqlConnection connection, MySqlTransaction transaction, string tableName, string characterId, IList<CharacterDataFloat32> list)
        {
            try
            {
                await DeleteCharacterDataFloat32s(connection, transaction, tableName, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < list.Count; ++i)
                {
                    await CreateCharacterDataFloat32(connection, transaction, tableName, insertedIds, characterId, list[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing custom float32 of character: " + characterId + ", table: " + tableName);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillSummonBuffs(MySqlConnection connection, MySqlTransaction transaction, string characterId, List<CharacterBuff> summonBuffs)
        {
            try
            {
                await DeleteSummonBuff(connection, transaction, characterId);
                HashSet<string> insertedIds = new HashSet<string>();
                int i;
                for (i = 0; i < summonBuffs.Count; ++i)
                {
                    await CreateSummonBuff(connection, transaction, insertedIds, characterId, summonBuffs[i]);
                }
            }
            catch (System.Exception ex)
            {
                LogError(LogTag, "Transaction, Error occurs while replacing buffs of summon: " + characterId);
                LogException(LogTag, ex);
                throw;
            }
        }

        private async UniTask FillCharacterRelatesData(TransactionUpdateCharacterState state, MySqlConnection connection, MySqlTransaction transaction, IPlayerCharacterData characterData, List<CharacterBuff> summonBuffs)
        {
            if (state.Has(TransactionUpdateCharacterState.Attributes))
                await FillCharacterAttributes(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Buffs))
                await FillCharacterBuffs(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Hotkeys))
                await FillCharacterHotkeys(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Items))
                await FillCharacterItems(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Quests))
                await FillCharacterQuests(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Currencies))
                await FillCharacterCurrencies(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Skills))
                await FillCharacterSkills(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.SkillUsages))
                await FillCharacterSkillUsages(connection, transaction, characterData);
            if (state.Has(TransactionUpdateCharacterState.Summons))
                await FillCharacterSummons(connection, transaction, characterData);

#if !DISABLE_CUSTOM_CHARACTER_DATA
            if (state.Has(TransactionUpdateCharacterState.ServerCustomData))
            {
                await FillCharacterDataBooleans(connection, transaction, "character_server_boolean", characterData.Id, characterData.ServerBools);
                await FillCharacterDataInt32s(connection, transaction, "character_server_int32", characterData.Id, characterData.ServerInts);
                await FillCharacterDataFloat32s(connection, transaction, "character_server_float32", characterData.Id, characterData.ServerFloats);
            }
            if (state.Has(TransactionUpdateCharacterState.PrivateCustomData))
            {
                await FillCharacterDataBooleans(connection, transaction, "character_private_boolean", characterData.Id, characterData.PrivateBools);
                await FillCharacterDataInt32s(connection, transaction, "character_private_int32", characterData.Id, characterData.PrivateInts);
                await FillCharacterDataFloat32s(connection, transaction, "character_private_float32", characterData.Id, characterData.PrivateFloats);
            }
            if (state.Has(TransactionUpdateCharacterState.PublicCustomData))
            {
                await FillCharacterDataBooleans(connection, transaction, "character_public_boolean", characterData.Id, characterData.PublicBools);
                await FillCharacterDataInt32s(connection, transaction, "character_public_int32", characterData.Id, characterData.PublicInts);
                await FillCharacterDataFloat32s(connection, transaction, "character_public_float32", characterData.Id, characterData.PublicFloats);
            }
#endif

            if (state.Has(TransactionUpdateCharacterState.Mount))
                await CreateOrUpdateCharacterMount(connection, transaction, characterData.Id, characterData.Mount);

#if !DISABLE_CLASSIC_PK
            if (state.Has(TransactionUpdateCharacterState.Pk))
            {
                await ExecuteNonQuery(connection, transaction, @"INSERT INTO character_pk
                    (id, isPkOn, lastPkOnTime, pkPoint, consecutivePkKills, highestPkPoint, highestConsecutivePkKills) VALUES
                    (@id, @isPkOn, @lastPkOnTime, @pkPoint, @consecutivePkKills, @highestPkPoint, @highestConsecutivePkKills)
                    ON DUPLICATE KEY UPDATE
                    isPkOn = @isPkOn,
                    lastPkOnTime = @lastPkOnTime,
                    pkPoint = @pkPoint,
                    consecutivePkKills = @consecutivePkKills,
                    highestPkPoint = @highestPkPoint,
                    highestConsecutivePkKills = @highestConsecutivePkKills",
                    new MySqlParameter("@id", characterData.Id),
                    new MySqlParameter("@isPkOn", characterData.IsPkOn),
                    new MySqlParameter("@lastPkOnTime", characterData.LastPkOnTime),
                    new MySqlParameter("@pkPoint", characterData.PkPoint),
                    new MySqlParameter("@consecutivePkKills", characterData.ConsecutivePkKills),
                    new MySqlParameter("@highestPkPoint", characterData.HighestPkPoint),
                    new MySqlParameter("@highestConsecutivePkKills", characterData.HighestConsecutivePkKills));
            }
#endif

            if (summonBuffs != null)
                await FillSummonBuffs(connection, transaction, characterData.Id, summonBuffs);
        }

        public override async UniTask CreateCharacter(string userId, IPlayerCharacterData character)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await ExecuteNonQuery(connection, transaction, "INSERT INTO characters " +
                    "(id, userId, dataId, entityId, factionId, characterName, level, exp, currentHp, currentMp, currentStamina, currentFood, currentWater, equipWeaponSet, statPoint, skillPoint, gold, currentChannel, currentMapName, currentPositionX, currentPositionY, currentPositionZ, currentRotationX, currentRotationY, currentRotationZ, currentSafeArea, respawnMapName, respawnPositionX, respawnPositionY, respawnPositionZ, iconDataId, frameDataId, backgroundDataId, titleDataId) VALUES " +
                    "(@id, @userId, @dataId, @entityId, @factionId, @characterName, @level, @exp, @currentHp, @currentMp, @currentStamina, @currentFood, @currentWater, @equipWeaponSet, @statPoint, @skillPoint, @gold, @currentChannel, @currentMapName, @currentPositionX, @currentPositionY, @currentPositionZ, @currentRotationX, @currentRotationY, @currentRotationZ, @currentSafeArea, @respawnMapName, @respawnPositionX, @respawnPositionY, @respawnPositionZ, @iconDataId, @frameDataId, @backgroundDataId, @titleDataId)",
                            new MySqlParameter("@id", character.Id),
                            new MySqlParameter("@userId", userId),
                            new MySqlParameter("@dataId", character.DataId),
                            new MySqlParameter("@entityId", character.EntityId),
                            new MySqlParameter("@factionId", character.FactionId),
                            new MySqlParameter("@characterName", character.CharacterName),
                            new MySqlParameter("@level", character.Level),
                            new MySqlParameter("@exp", character.Exp),
                            new MySqlParameter("@currentHp", character.CurrentHp),
                            new MySqlParameter("@currentMp", character.CurrentMp),
                            new MySqlParameter("@currentStamina", character.CurrentStamina),
                            new MySqlParameter("@currentFood", character.CurrentFood),
                            new MySqlParameter("@currentWater", character.CurrentWater),
                            new MySqlParameter("@equipWeaponSet", character.EquipWeaponSet),
                            new MySqlParameter("@statPoint", character.StatPoint),
                            new MySqlParameter("@skillPoint", character.SkillPoint),
                            new MySqlParameter("@gold", character.Gold),
                            new MySqlParameter("@currentChannel", string.Empty),
                            new MySqlParameter("@currentMapName", character.CurrentMapName),
                            new MySqlParameter("@currentPositionX", character.CurrentPosition.x),
                            new MySqlParameter("@currentPositionY", character.CurrentPosition.y),
                            new MySqlParameter("@currentPositionZ", character.CurrentPosition.z),
                            new MySqlParameter("@currentRotationX", character.CurrentRotation.x),
                            new MySqlParameter("@currentRotationY", character.CurrentRotation.y),
                            new MySqlParameter("@currentRotationZ", character.CurrentRotation.z),
                            new MySqlParameter("@currentSafeArea", string.Empty),
#if !DISABLE_DIFFER_MAP_RESPAWNING
                            new MySqlParameter("@respawnMapName", character.RespawnMapName),
                            new MySqlParameter("@respawnPositionX", character.RespawnPosition.x),
                            new MySqlParameter("@respawnPositionY", character.RespawnPosition.y),
                            new MySqlParameter("@respawnPositionZ", character.RespawnPosition.z),
#endif
                            new MySqlParameter("@iconDataId", character.IconDataId),
                            new MySqlParameter("@frameDataId", character.FrameDataId),
                            new MySqlParameter("@backgroundDataId", character.BackgroundDataId),
                            new MySqlParameter("@titleDataId", character.TitleDataId));
                        TransactionUpdateCharacterState state = TransactionUpdateCharacterState.All;
                        await FillCharacterRelatesData(state, connection, transaction, character, null);
                        if (onCreateCharacter != null)
                            await onCreateCharacter.Invoke(connection, transaction, userId, character);
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception ex)
                    {
                        LogError(LogTag, "Transaction, Error occurs while create character: " + character.Id);
                        LogException(LogTag, ex);
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        private bool GetCharacter(MySqlDataReader reader, out PlayerCharacterData result)
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
            await ExecuteReader((reader) =>
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
                new MySqlParameter("@id", id));
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
                List<UniTask> tasks = new List<UniTask>();
                if (withEquipWeapons)
                    tasks.Add(ReadCharacterEquipWeapons(id, selectableWeaponSets));
                if (withAttributes)
                    tasks.Add(ReadCharacterAttributes(id, attributes));
                if (withSkills)
                    tasks.Add(ReadCharacterSkills(id, skills));
                if (withSkillUsages)
                    tasks.Add(ReadCharacterSkillUsages(id, skillUsages));
                if (withBuffs)
                    tasks.Add(ReadCharacterBuffs(id, buffs));
                if (withEquipItems)
                    tasks.Add(ReadCharacterEquipItems(id, equipItems));
                if (withNonEquipItems)
                    tasks.Add(ReadCharacterNonEquipItems(id, nonEquipItems));
                if (withSummons)
                    tasks.Add(ReadCharacterSummons(id, summons));
                if (withHotkeys)
                    tasks.Add(ReadCharacterHotkeys(id, hotkeys));
                if (withQuests)
                    tasks.Add(ReadCharacterQuests(id, quests));
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
                if (withCurrencies)
                    tasks.Add(ReadCharacterCurrencies(id, currencies));
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
                if (withServerCustomData)
                {
                    tasks.Add(ReadCharacterDataBooleans("character_server_boolean", id, serverBools));
                    tasks.Add(ReadCharacterDataInt32s("character_server_int32", id, serverInts));
                    tasks.Add(ReadCharacterDataFloat32s("character_server_float32", id, serverFloats));
                }
                if (withPrivateCustomData)
                {
                    tasks.Add(ReadCharacterDataBooleans("character_private_boolean", id, privateBools));
                    tasks.Add(ReadCharacterDataInt32s("character_private_int32", id, privateInts));
                    tasks.Add(ReadCharacterDataFloat32s("character_private_float32", id, privateFloats));
                }
                if (withPublicCustomData)
                {
                    tasks.Add(ReadCharacterDataBooleans("character_public_boolean", id, publicBools));
                    tasks.Add(ReadCharacterDataInt32s("character_public_int32", id, publicInts));
                    tasks.Add(ReadCharacterDataFloat32s("character_public_float32", id, publicFloats));
                }
#endif
                await UniTask.WhenAll(tasks);
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
            await ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    characterIds.Add(reader.GetString(0));
                }
            }, "SELECT id FROM characters WHERE userId=@userId ORDER BY updateAt DESC", new MySqlParameter("@userId", userId));
            foreach (string characterId in characterIds)
            {
                result.Add(await GetCharacter(characterId, true, false, false, false, false, true, false, false, false, false, false, false, false, true));
            }
            return result;
        }

        public override async UniTask UpdateCharacter(TransactionUpdateCharacterState state, IPlayerCharacterData character, List<CharacterBuff> summonBuffs, bool deleteStorageReservation)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (state.Has(TransactionUpdateCharacterState.Character))
                        {
                            await ExecuteNonQuery(connection, transaction, "UPDATE characters SET " +
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
                                " updateAt=UTC_TIME()" +
                                " WHERE id=@id",
                                new MySqlParameter("@dataId", character.DataId),
                                new MySqlParameter("@entityId", character.EntityId),
                                new MySqlParameter("@factionId", character.FactionId),
                                new MySqlParameter("@characterName", character.CharacterName),
                                new MySqlParameter("@level", character.Level),
                                new MySqlParameter("@exp", character.Exp),
                                new MySqlParameter("@currentHp", character.CurrentHp),
                                new MySqlParameter("@currentMp", character.CurrentMp),
                                new MySqlParameter("@currentStamina", character.CurrentStamina),
                                new MySqlParameter("@currentFood", character.CurrentFood),
                                new MySqlParameter("@currentWater", character.CurrentWater),
                                new MySqlParameter("@equipWeaponSet", character.EquipWeaponSet),
                                new MySqlParameter("@statPoint", character.StatPoint),
                                new MySqlParameter("@skillPoint", character.SkillPoint),
                                new MySqlParameter("@gold", character.Gold),
                                new MySqlParameter("@currentChannel", character.CurrentChannel),
                                new MySqlParameter("@currentMapName", character.CurrentMapName),
                                new MySqlParameter("@currentPositionX", character.CurrentPosition.x),
                                new MySqlParameter("@currentPositionY", character.CurrentPosition.y),
                                new MySqlParameter("@currentPositionZ", character.CurrentPosition.z),
                                new MySqlParameter("@currentRotationX", character.CurrentRotation.x),
                                new MySqlParameter("@currentRotationY", character.CurrentRotation.y),
                                new MySqlParameter("@currentRotationZ", character.CurrentRotation.z),
                                new MySqlParameter("@currentSafeArea", character.CurrentSafeArea),
#if !DISABLE_DIFFER_MAP_RESPAWNING
                                new MySqlParameter("@respawnMapName", character.RespawnMapName),
                                new MySqlParameter("@respawnPositionX", character.RespawnPosition.x),
                                new MySqlParameter("@respawnPositionY", character.RespawnPosition.y),
                                new MySqlParameter("@respawnPositionZ", character.RespawnPosition.z),
#endif
                                new MySqlParameter("@iconDataId", character.IconDataId),
                                new MySqlParameter("@frameDataId", character.FrameDataId),
                                new MySqlParameter("@backgroundDataId", character.BackgroundDataId),
                                new MySqlParameter("@titleDataId", character.TitleDataId),
                                new MySqlParameter("@reputation", character.Reputation),
                                new MySqlParameter("@lastDeadTime", character.LastDeadTime),
                                new MySqlParameter("@unmuteTime", character.UnmuteTime),
                                new MySqlParameter("@id", character.Id));
                        }
                        await FillCharacterRelatesData(state, connection, transaction, character, summonBuffs);
                        if (deleteStorageReservation)
                        {
                            await ExecuteNonQuery(connection, transaction, "DELETE FROM storage_reservation WHERE reserverId=@reserverId",
                                new MySqlParameter("@reserverId", character.Id));
                        }
                        if (onUpdateCharacter != null)
                            await onUpdateCharacter.Invoke(connection, transaction, state, character);
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception ex)
                    {
                        LogError(LogTag, "Transaction, Error occurs while update character: " + character.Id);
                        LogException(LogTag, ex);
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        public override async UniTask DeleteCharacter(string userId, string id)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM characters WHERE id=@id AND userId=@userId",
                new MySqlParameter("@id", id),
                new MySqlParameter("@userId", userId));
            long count = result != null ? (long)result : 0;
            if (count > 0)
            {
                using (MySqlConnection connection = NewConnection())
                {
                    await OpenConnection(connection);
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            await ExecuteNonQuery(connection, transaction, "DELETE FROM characters WHERE id=@characterId", new MySqlParameter("@characterId", id));
                            await ExecuteNonQuery(connection, transaction, "DELETE FROM character_pk WHERE id=@characterId", new MySqlParameter("@characterId", id));
                            await ExecuteNonQuery(connection, transaction, "DELETE FROM friend WHERE characterId1 LIKE @characterId OR characterId2 LIKE @characterId", new MySqlParameter("@characterId", id));
                            await DeleteCharacterAttributes(connection, transaction, id);
                            await DeleteCharacterCurrencies(connection, transaction, id);
                            await DeleteCharacterBuffs(connection, transaction, id);
                            await DeleteCharacterHotkeys(connection, transaction, id);
                            await DeleteCharacterItems(connection, transaction, id);
                            await DeleteCharacterQuests(connection, transaction, id);
                            await DeleteCharacterSkills(connection, transaction, id);
                            await DeleteCharacterSkillUsages(connection, transaction, id);
                            await DeleteCharacterSummons(connection, transaction, id);
                            await DeleteCharacterMount(connection, transaction, id);

                            await DeleteCharacterDataBooleans(connection, transaction, "character_server_boolean", id);
                            await DeleteCharacterDataInt32s(connection, transaction, "character_server_int32", id);
                            await DeleteCharacterDataFloat32s(connection, transaction, "character_server_float32", id);

                            await DeleteCharacterDataBooleans(connection, transaction, "character_private_boolean", id);
                            await DeleteCharacterDataInt32s(connection, transaction, "character_private_int32", id);
                            await DeleteCharacterDataFloat32s(connection, transaction, "character_private_float32", id);

                            await DeleteCharacterDataBooleans(connection, transaction, "character_public_boolean", id);
                            await DeleteCharacterDataInt32s(connection, transaction, "character_public_int32", id);
                            await DeleteCharacterDataFloat32s(connection, transaction, "character_public_float32", id);

                            if (onDeleteCharacter != null)
                                await onDeleteCharacter.Invoke(connection, transaction, userId, id);
                            await transaction.CommitAsync();
                        }
                        catch (System.Exception ex)
                        {
                            LogError(LogTag, "Transaction, Error occurs while deleting character: " + id);
                            LogException(LogTag, ex);
                            await transaction.RollbackAsync();
                        }
                    }
                }
            }
        }

        public override async UniTask<long> FindCharacterName(string characterName)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM characters WHERE characterName LIKE @characterName",
                new MySqlParameter("@characterName", characterName));
            return result != null ? (long)result : 0;
        }

        public override async UniTask<string> GetIdByCharacterName(string characterName)
        {
            object result = await ExecuteScalar("SELECT id FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new MySqlParameter("@characterName", characterName));
            return result != null ? (string)result : string.Empty;
        }

        public override async UniTask<string> GetUserIdByCharacterName(string characterName)
        {
            object result = await ExecuteScalar("SELECT userId FROM characters WHERE characterName LIKE @characterName LIMIT 1",
                new MySqlParameter("@characterName", characterName));
            return result != null ? (string)result : string.Empty;
        }

        public override async UniTask<List<SocialCharacterData>> FindCharacters(string finderId, string characterName, int skip, int limit)
        {
            string excludeIdsQuery = "(id!='" + finderId + "'";
            // Exclude friend, requested characters
            await ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    excludeIdsQuery += " AND id!='" + reader.GetString(0) + "'";
                }
            }, "SELECT characterId2 FROM friend WHERE characterId1='" + finderId + "'");
            excludeIdsQuery += ")";
            List<SocialCharacterData> result = new List<SocialCharacterData>();
            await ExecuteReader((reader) =>
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
            }, "SELECT id, dataId, characterName, level FROM characters WHERE characterName LIKE @characterName AND " + excludeIdsQuery + " ORDER BY RAND() LIMIT " + skip + ", " + limit,
                new MySqlParameter("@characterName", "%" + characterName + "%"));
            return result;
        }

        public override async UniTask CreateFriend(string id1, string id2, byte state)
        {
            using (MySqlConnection connection = NewConnection())
            {
                await OpenConnection(connection);
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await ExecuteNonQuery(connection, transaction, "DELETE FROM friend WHERE " +
                           "characterId1 LIKE @characterId1 AND " +
                           "characterId2 LIKE @characterId2",
                           new MySqlParameter("@characterId1", id1),
                           new MySqlParameter("@characterId2", id2));
                        await ExecuteNonQuery(connection, transaction, "INSERT INTO friend " +
                            "(characterId1, characterId2, state) VALUES " +
                            "(@characterId1, @characterId2, @state)",
                            new MySqlParameter("@characterId1", id1),
                            new MySqlParameter("@characterId2", id2),
                            new MySqlParameter("@state", state));
                        await transaction.CommitAsync();
                    }
                    catch (System.Exception ex)
                    {
                        LogError(LogTag, "Transaction, Error occurs while creating friend: " + id1 + " " + id2);
                        LogException(LogTag, ex);
                        await transaction.RollbackAsync();
                    }
                }
            }
        }

        public override async UniTask DeleteFriend(string id1, string id2)
        {
            await ExecuteNonQuery("DELETE FROM friend WHERE " +
               "characterId1 LIKE @characterId1 AND " +
               "characterId2 LIKE @characterId2",
               new MySqlParameter("@characterId1", id1),
               new MySqlParameter("@characterId2", id2));
        }

        public override async UniTask<List<SocialCharacterData>> GetFriends(string id, bool readById2, byte state, int skip, int limit)
        {
            List<SocialCharacterData> result = new List<SocialCharacterData>();
            List<string> characterIds = new List<string>();
            if (readById2)
            {
                await ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        characterIds.Add(reader.GetString(0));
                    }
                }, "SELECT characterId1 FROM friend WHERE characterId2=@id AND state=" + state + " LIMIT " + skip + ", " + limit,
                    new MySqlParameter("@id", id));
            }
            else
            {
                await ExecuteReader((reader) =>
                {
                    while (reader.Read())
                    {
                        characterIds.Add(reader.GetString(0));
                    }
                }, "SELECT characterId2 FROM friend WHERE characterId1=@id AND state=" + state + " LIMIT " + skip + ", " + limit,
                    new MySqlParameter("@id", id));
            }
            SocialCharacterData socialCharacterData;
            foreach (string characterId in characterIds)
            {
                await ExecuteReader((reader) =>
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
                }, "SELECT id, dataId, characterName, level FROM characters WHERE BINARY id = @id",
                    new MySqlParameter("@id", characterId));
            }
            return result;
        }

        public override async UniTask<int> GetFriendRequestNotification(string characterId)
        {
            object result = await ExecuteScalar("SELECT COUNT(*) FROM friend WHERE characterId2=@characterId AND state=1",
                new MySqlParameter("@characterId", characterId));
            return (int)(long)result;
        }
    }
}
#endif