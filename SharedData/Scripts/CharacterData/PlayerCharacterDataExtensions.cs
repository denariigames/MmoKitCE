using Insthync.DevExtension;
using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public static partial class PlayerCharacterDataExtensions
    {
        public static System.Type ClassType { get; private set; }

        static PlayerCharacterDataExtensions()
        {
            ClassType = typeof(PlayerCharacterDataExtensions);
        }

        public static T CloneTo<T>(this IPlayerCharacterData from, T to,
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
            bool withPublicCustomData = true,
            bool withCurrentLocationData = true) where T : IPlayerCharacterData
        {
            to.Id = from.Id;
            to.DataId = from.DataId;
            to.EntityId = from.EntityId;
            to.UserId = from.UserId;
            to.FactionId = from.FactionId;
            to.CharacterName = from.CharacterName;
            to.Level = from.Level;
            to.Exp = from.Exp;
            to.CurrentHp = from.CurrentHp;
            to.CurrentMp = from.CurrentMp;
            to.CurrentStamina = from.CurrentStamina;
            to.CurrentFood = from.CurrentFood;
            to.CurrentWater = from.CurrentWater;
            to.StatPoint = from.StatPoint;
            to.SkillPoint = from.SkillPoint;
            to.Gold = from.Gold;
            to.UserGold = from.UserGold;
            to.UserCash = from.UserCash;
            to.PartyId = from.PartyId;
            to.GuildId = from.GuildId;
            to.GuildRole = from.GuildRole;
            to.EquipWeaponSet = from.EquipWeaponSet;
            if (withCurrentLocationData)
            {
                to.CurrentChannel = from.CurrentChannel;
                to.CurrentMapName = from.CurrentMapName;
                to.CurrentPosition = from.CurrentPosition;
                to.CurrentRotation = from.CurrentRotation;
                to.CurrentSafeArea = from.CurrentSafeArea;
            }
#if !DISABLE_DIFFER_MAP_RESPAWNING
            to.RespawnMapName = from.RespawnMapName;
            to.RespawnPosition = from.RespawnPosition;
#endif
            to.IconDataId = from.IconDataId;
            to.FrameDataId = from.FrameDataId;
            to.TitleDataId = from.TitleDataId;
            to.LastDeadTime = from.LastDeadTime;
            to.UnmuteTime = from.UnmuteTime;
            to.LastUpdate = from.LastUpdate;
#if !DISABLE_CLASSIC_PK
            to.IsPkOn = from.IsPkOn;
            to.LastPkOnTime = from.LastPkOnTime;
            to.PkPoint = from.PkPoint;
            to.ConsecutivePkKills = from.ConsecutivePkKills;
            to.HighestPkPoint = from.HighestPkPoint;
            to.HighestConsecutivePkKills = from.HighestConsecutivePkKills;
#endif
            to.Reputation = from.Reputation;
            if (withEquipWeapons)
                to.SelectableWeaponSets = from.SelectableWeaponSets.Clone();
            if (withAttributes)
                to.Attributes = from.Attributes.Clone();
            if (withSkills)
                to.Skills = from.Skills.Clone();
            if (withSkillUsages)
                to.SkillUsages = from.SkillUsages.Clone();
            if (withBuffs)
                to.Buffs = from.Buffs.Clone();
            if (withEquipItems)
                to.EquipItems = from.EquipItems.Clone();
            if (withNonEquipItems)
                to.NonEquipItems = from.NonEquipItems.Clone();
            if (withSummons)
                to.Summons = from.Summons.Clone();
            if (withHotkeys)
                to.Hotkeys = from.Hotkeys.Clone();
            if (withQuests)
                to.Quests = from.Quests.Clone();
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            if (withCurrencies)
                to.Currencies = from.Currencies.Clone();
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            if (withServerCustomData)
            {
                to.ServerBools = from.ServerBools.Clone();
                to.ServerInts = from.ServerInts.Clone();
                to.ServerFloats = from.ServerFloats.Clone();
            }
            if (withPrivateCustomData)
            {
                to.PrivateBools = from.PrivateBools.Clone();
                to.PrivateInts = from.PrivateInts.Clone();
                to.PrivateFloats = from.PrivateFloats.Clone();
            }
            if (withPublicCustomData)
            {
                to.PublicBools = from.PublicBools.Clone();
                to.PublicInts = from.PublicInts.Clone();
                to.PublicFloats = from.PublicFloats.Clone();
            }
#endif
            to.GuildSkills = from.GuildSkills.Clone();
            to.Mount = from.Mount.Clone();
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "CloneTo", from, to);
            return to;
        }

        public static void SerializeCharacterData<T>(this T characterData, NetDataWriter writer,
            bool withTransforms = true,
            bool withRespawningMap = true,
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
            bool withPublicCustomData = true) where T : IPlayerCharacterData
        {
            writer.Put(characterData.Id);
            writer.PutPackedInt(characterData.DataId);
            writer.PutPackedInt(characterData.EntityId);
            writer.Put(characterData.UserId);
            writer.PutPackedInt(characterData.FactionId);
            writer.Put(characterData.CharacterName);
            writer.PutPackedInt(characterData.Level);
            writer.PutPackedInt(characterData.Exp);
            writer.PutPackedInt(characterData.CurrentHp);
            writer.PutPackedInt(characterData.CurrentMp);
            writer.PutPackedInt(characterData.CurrentStamina);
            writer.PutPackedInt(characterData.CurrentFood);
            writer.PutPackedInt(characterData.CurrentWater);
            writer.Put(characterData.StatPoint);
            writer.Put(characterData.SkillPoint);
            writer.PutPackedInt(characterData.Gold);
            writer.PutPackedInt(characterData.UserGold);
            writer.PutPackedInt(characterData.UserCash);
            writer.PutPackedInt(characterData.PartyId);
            writer.PutPackedInt(characterData.GuildId);
            writer.Put(characterData.GuildRole);
            writer.Put(characterData.CurrentChannel);
            writer.Put(characterData.CurrentMapName);
            if (withTransforms)
            {
                writer.Put(characterData.CurrentPosition.x);
                writer.Put(characterData.CurrentPosition.y);
                writer.Put(characterData.CurrentPosition.z);
                writer.Put(characterData.CurrentRotation.x);
                writer.Put(characterData.CurrentRotation.y);
                writer.Put(characterData.CurrentRotation.z);
            }
            writer.Put(characterData.CurrentSafeArea);
#if !DISABLE_DIFFER_MAP_RESPAWNING
            if (withRespawningMap)
            {
                writer.Put(characterData.RespawnMapName);
                writer.Put(characterData.RespawnPosition.x);
                writer.Put(characterData.RespawnPosition.y);
                writer.Put(characterData.RespawnPosition.z);
            }
#endif
            writer.PutPackedInt(characterData.IconDataId);
            writer.PutPackedInt(characterData.FrameDataId);
            writer.PutPackedInt(characterData.TitleDataId);
            writer.PutPackedLong(characterData.LastDeadTime);
            writer.PutPackedLong(characterData.UnmuteTime);
            writer.PutPackedLong(characterData.LastUpdate);
#if !DISABLE_CLASSIC_PK
            writer.Put(characterData.IsPkOn);
            writer.PutPackedLong(characterData.LastPkOnTime);
            writer.PutPackedInt(characterData.PkPoint);
            writer.PutPackedInt(characterData.ConsecutivePkKills);
            writer.PutPackedInt(characterData.HighestPkPoint);
            writer.PutPackedInt(characterData.HighestConsecutivePkKills);
#endif
            writer.PutPackedInt(characterData.Reputation);
            int i;
            // Attributes
            if (withAttributes)
            {
                writer.PutPackedInt(characterData.Attributes.Count);
                for (i = 0; i < characterData.Attributes.Count; ++i)
                {
                    CharacterAttribute entry = characterData.Attributes[i];
                    writer.Put(entry);
                }
            }
            // Buffs
            if (withBuffs)
            {
                writer.PutPackedInt(characterData.Buffs.Count);
                for (i = 0; i < characterData.Buffs.Count; ++i)
                {
                    CharacterBuff entry = characterData.Buffs[i];
                    writer.Put(entry);
                }
            }
            // Skills
            if (withSkills)
            {
                writer.PutPackedInt(characterData.Skills.Count);
                for (i = 0; i < characterData.Skills.Count; ++i)
                {
                    CharacterSkill entry = characterData.Skills[i];
                    writer.Put(entry);
                }
            }
            // Skill Usages
            if (withSkillUsages)
            {
                writer.PutPackedInt(characterData.SkillUsages.Count);
                for (i = 0; i < characterData.SkillUsages.Count; ++i)
                {
                    CharacterSkillUsage entry = characterData.SkillUsages[i];
                    writer.Put(entry);
                }
            }
            // Summons
            if (withSummons)
            {
                writer.PutPackedInt(characterData.Summons.Count);
                for (i = 0; i < characterData.Summons.Count; ++i)
                {
                    CharacterSummon entry = characterData.Summons[i];
                    writer.Put(entry);
                }
            }
            // Equip Items
            if (withEquipItems)
            {
                writer.PutPackedInt(characterData.EquipItems.Count);
                for (i = 0; i < characterData.EquipItems.Count; ++i)
                {
                    CharacterItem entry = characterData.EquipItems[i];
                    writer.Put(entry);
                }
            }
            // Non Equip Items
            if (withNonEquipItems)
            {
                writer.PutPackedInt(characterData.NonEquipItems.Count);
                for (i = 0; i < characterData.NonEquipItems.Count; ++i)
                {
                    CharacterItem entry = characterData.NonEquipItems[i];
                    writer.Put(entry);
                }
            }
            // Hotkeys
            if (withHotkeys)
            {
                writer.PutPackedInt(characterData.Hotkeys.Count);
                for (i = 0; i < characterData.Hotkeys.Count; ++i)
                {
                    CharacterHotkey entry = characterData.Hotkeys[i];
                    writer.Put(entry);
                }
            }
            // Quests
            if (withQuests)
            {
                writer.PutPackedInt(characterData.Quests.Count);
                for (i = 0; i < characterData.Quests.Count; ++i)
                {
                    CharacterQuest entry = characterData.Quests[i];
                    writer.Put(entry);
                }
            }
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            // Currencies
            if (withCurrencies)
            {
                writer.PutPackedInt(characterData.Currencies.Count);
                for (i = 0; i < characterData.Currencies.Count; ++i)
                {
                    CharacterCurrency entry = characterData.Currencies[i];
                    writer.Put(entry);
                }
            }
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            // Server custom data
            if (withServerCustomData)
            {
                writer.PutPackedInt(characterData.ServerBools.Count);
                for (i = 0; i < characterData.ServerBools.Count; ++i)
                {
                    CharacterDataBoolean entry = characterData.ServerBools[i];
                    writer.Put(entry);
                }
                writer.PutPackedInt(characterData.ServerInts.Count);
                for (i = 0; i < characterData.ServerInts.Count; ++i)
                {
                    CharacterDataInt32 entry = characterData.ServerInts[i];
                    writer.Put(entry);
                }
                writer.PutPackedInt(characterData.ServerFloats.Count);
                for (i = 0; i < characterData.ServerFloats.Count; ++i)
                {
                    CharacterDataFloat32 entry = characterData.ServerFloats[i];
                    writer.Put(entry);
                }
            }
            // Private custom data
            if (withPrivateCustomData)
            {
                writer.PutPackedInt(characterData.PrivateBools.Count);
                for (i = 0; i < characterData.PrivateBools.Count; ++i)
                {
                    CharacterDataBoolean entry = characterData.PrivateBools[i];
                    writer.Put(entry);
                }
                writer.PutPackedInt(characterData.PrivateInts.Count);
                for (i = 0; i < characterData.PrivateInts.Count; ++i)
                {
                    CharacterDataInt32 entry = characterData.PrivateInts[i];
                    writer.Put(entry);
                }
                writer.PutPackedInt(characterData.PrivateFloats.Count);
                for (i = 0; i < characterData.PrivateFloats.Count; ++i)
                {
                    CharacterDataFloat32 entry = characterData.PrivateFloats[i];
                    writer.Put(entry);
                }
            }
            // Public custom data
            if (withPublicCustomData)
            {
                writer.PutPackedInt(characterData.PublicBools.Count);
                for (i = 0; i < characterData.PublicBools.Count; ++i)
                {
                    CharacterDataBoolean entry = characterData.PublicBools[i];
                    writer.Put(entry);
                }
                writer.PutPackedInt(characterData.PublicInts.Count);
                for (i = 0; i < characterData.PublicInts.Count; ++i)
                {
                    CharacterDataInt32 entry = characterData.PublicInts[i];
                    writer.Put(entry);
                }
                writer.PutPackedInt(characterData.PublicFloats.Count);
                for (i = 0; i < characterData.PublicFloats.Count; ++i)
                {
                    CharacterDataFloat32 entry = characterData.PublicFloats[i];
                    writer.Put(entry);
                }
            }
#endif
            // Equip weapon set
            writer.Put(characterData.EquipWeaponSet);
            // Selectable weapon sets
            if (withEquipWeapons)
            {
                writer.PutPackedInt(characterData.SelectableWeaponSets.Count);
                for (i = 0; i < characterData.SelectableWeaponSets.Count; ++i)
                {
                    EquipWeapons entry = characterData.SelectableWeaponSets[i];
                    writer.Put(entry);
                }
            }

            writer.Put(characterData.Mount);
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "SerializeCharacterData", characterData, writer);
        }

        public static PlayerCharacterData DeserializeCharacterData(this NetDataReader reader)
        {
            return new PlayerCharacterData().DeserializeCharacterData(reader);
        }

        public static void DeserializeCharacterData(this NetDataReader reader, ref PlayerCharacterData characterData)
        {
            characterData = reader.DeserializeCharacterData();
        }

        public static T DeserializeCharacterData<T>(this T characterData, NetDataReader reader,
            bool withTransforms = true,
            bool withRespawningMap = true,
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
            bool withPublicCustomData = true) where T : IPlayerCharacterData
        {
            characterData.Id = reader.GetString();
            characterData.DataId = reader.GetPackedInt();
            characterData.EntityId = reader.GetPackedInt();
            characterData.UserId = reader.GetString();
            characterData.FactionId = reader.GetPackedInt();
            characterData.CharacterName = reader.GetString();
            characterData.Level = reader.GetPackedInt();
            characterData.Exp = reader.GetPackedInt();
            characterData.CurrentHp = reader.GetPackedInt();
            characterData.CurrentMp = reader.GetPackedInt();
            characterData.CurrentStamina = reader.GetPackedInt();
            characterData.CurrentFood = reader.GetPackedInt();
            characterData.CurrentWater = reader.GetPackedInt();
            characterData.StatPoint = reader.GetFloat();
            characterData.SkillPoint = reader.GetFloat();
            characterData.Gold = reader.GetPackedInt();
            characterData.UserGold = reader.GetPackedInt();
            characterData.UserCash = reader.GetPackedInt();
            characterData.PartyId = reader.GetPackedInt();
            characterData.GuildId = reader.GetPackedInt();
            characterData.GuildRole = reader.GetByte();
            characterData.CurrentChannel = reader.GetString();
            characterData.CurrentMapName = reader.GetString();
            if (withTransforms)
            {
                characterData.CurrentPosition = new Vec3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
                characterData.CurrentRotation = new Vec3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            }
            characterData.CurrentSafeArea = reader.GetString();
#if !DISABLE_DIFFER_MAP_RESPAWNING
            if (withRespawningMap)
            {
                characterData.RespawnMapName = reader.GetString();
                characterData.RespawnPosition = new Vec3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
            }
#endif
            characterData.IconDataId = reader.GetPackedInt();
            characterData.FrameDataId = reader.GetPackedInt();
            characterData.TitleDataId = reader.GetPackedInt();
            characterData.LastDeadTime = reader.GetPackedLong();
            characterData.UnmuteTime = reader.GetPackedLong();
            characterData.LastUpdate = reader.GetPackedLong();
#if !DISABLE_CLASSIC_PK
            characterData.IsPkOn = reader.GetBool();
            characterData.LastPkOnTime = reader.GetPackedLong();
            characterData.PkPoint = reader.GetPackedInt();
            characterData.ConsecutivePkKills = reader.GetPackedInt();
            characterData.HighestPkPoint = reader.GetPackedInt();
            characterData.HighestConsecutivePkKills = reader.GetPackedInt();
#endif
            characterData.Reputation = reader.GetPackedInt();
            int count;
            int i;
            // Attributes
            if (withAttributes)
            {
                characterData.Attributes.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.Attributes.Add(reader.Get<CharacterAttribute>());
                }
            }
            // Buffs
            if (withBuffs)
            {
                characterData.Buffs.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.Buffs.Add(reader.Get<CharacterBuff>());
                }
            }
            // Skills
            if (withSkills)
            {
                characterData.Skills.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.Skills.Add(reader.Get<CharacterSkill>());
                }
            }
            // Skill Usages
            if (withSkillUsages)
            {
                characterData.SkillUsages.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.SkillUsages.Add(reader.Get<CharacterSkillUsage>());
                }
            }
            // Summons
            if (withSummons)
            {
                characterData.Summons.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.Summons.Add(reader.Get<CharacterSummon>());
                }
            }
            // Equip Items
            if (withEquipItems)
            {
                characterData.EquipItems.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.EquipItems.Add(reader.Get<CharacterItem>());
                }
            }
            // Non Equip Items
            if (withNonEquipItems)
            {
                characterData.NonEquipItems.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.NonEquipItems.Add(reader.Get<CharacterItem>());
                }
            }
            // Hotkeys
            if (withHotkeys)
            {
                characterData.Hotkeys.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.Hotkeys.Add(reader.Get<CharacterHotkey>());
                }
            }
            // Quests
            if (withQuests)
            {
                characterData.Quests.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.Quests.Add(reader.Get<CharacterQuest>());
                }
            }
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            // Currencies
            if (withCurrencies)
            {
                characterData.Currencies.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.Currencies.Add(reader.Get<CharacterCurrency>());
                }
            }
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            // Server custom data
            if (withServerCustomData)
            {
                characterData.ServerBools.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.ServerBools.Add(reader.Get<CharacterDataBoolean>());
                }
                characterData.ServerInts.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.ServerInts.Add(reader.Get<CharacterDataInt32>());
                }
                characterData.ServerFloats.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.ServerFloats.Add(reader.Get<CharacterDataFloat32>());
                }
            }
            // Private custom data
            if (withPrivateCustomData)
            {
                characterData.PrivateBools.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.PrivateBools.Add(reader.Get<CharacterDataBoolean>());
                }
                characterData.PrivateInts.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.PrivateInts.Add(reader.Get<CharacterDataInt32>());
                }
                characterData.PrivateFloats.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.PrivateFloats.Add(reader.Get<CharacterDataFloat32>());
                }
            }
            // Public custom data
            if (withPublicCustomData)
            {
                characterData.PublicBools.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.PublicBools.Add(reader.Get<CharacterDataBoolean>());
                }
                characterData.PublicInts.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.PublicInts.Add(reader.Get<CharacterDataInt32>());
                }
                characterData.PublicFloats.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.PublicFloats.Add(reader.Get<CharacterDataFloat32>());
                }
            }
#endif
            // Equip weapon set
            characterData.EquipWeaponSet = reader.GetByte();
            // Selectable weapon sets
            if (withEquipWeapons)
            {
                characterData.SelectableWeaponSets.Clear();
                count = reader.GetPackedInt();
                for (i = 0; i < count; ++i)
                {
                    characterData.SelectableWeaponSets.Add(reader.Get<EquipWeapons>());
                }
            }

            characterData.Mount = reader.Get<CharacterMount>();
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "DeserializeCharacterData", characterData, reader);
            return characterData;
        }

        public static int IndexOfHotkey(this IPlayerCharacterData data, string hotkeyId)
        {
            return data.Hotkeys.IndexOf(hotkeyId);
        }

        public static int IndexOfQuest(this IPlayerCharacterData data, int dataId)
        {
            return data.Quests.IndexOf(dataId);
        }

        public static int IndexOfCurrency(this IPlayerCharacterData data, int dataId)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            return data.Currencies.IndexOf(dataId);
#else
            return -1;
#endif
        }

        public static int IndexOfServerBoolean(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerBools.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfServerInt32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerInts.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfServerFloat32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerFloats.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPrivateBoolean(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateBools.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPrivateInt32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateInts.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPrivateFloat32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateFloats.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPublicBoolean(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicBools.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPublicInt32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicInts.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static int IndexOfPublicFloat32(this IPlayerCharacterData data, int hashedKey)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicFloats.IndexOf(hashedKey);
#else
            return -1;
#endif
        }

        public static bool GetServerBoolean(this IPlayerCharacterData data, int hashedKey, bool defaultValue = false)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerBools.GetValue(hashedKey, defaultValue);
#else
            return false;
#endif
        }

        public static void SetServerBoolean(this IPlayerCharacterData data, int hashedKey, bool value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.ServerBools.SetValue(hashedKey, value);
#endif
        }

        public static int GetServerInt32(this IPlayerCharacterData data, int hashedKey, int defaultValue = 0)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerInts.GetValue(hashedKey, defaultValue);
#else
            return 0;
#endif
        }

        public static void SetServerInt32(this IPlayerCharacterData data, int hashedKey, int value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.ServerInts.SetValue(hashedKey, value);
#endif
        }

        public static float GetServerFloat32(this IPlayerCharacterData data, int hashedKey, float defaultValue = 0f)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.ServerFloats.GetValue(hashedKey, defaultValue);
#else
            return 0f;
#endif
        }

        public static void SetServerFloat32(this IPlayerCharacterData data, int hashedKey, float value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.ServerFloats.SetValue(hashedKey, value);
#endif
        }

        public static bool GetPrivateBoolean(this IPlayerCharacterData data, int hashedKey, bool defaultValue = false)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateBools.GetValue(hashedKey, defaultValue);
#else
            return false;
#endif
        }

        public static void SetPrivateBoolean(this IPlayerCharacterData data, int hashedKey, bool value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PrivateBools.SetValue(hashedKey, value);
#endif
        }

        public static int GetPrivateInt32(this IPlayerCharacterData data, int hashedKey, int defaultValue = 0)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateInts.GetValue(hashedKey, defaultValue);
#else
            return 0;
#endif
        }

        public static void SetPrivateInt32(this IPlayerCharacterData data, int hashedKey, int value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PrivateInts.SetValue(hashedKey, value);
#endif
        }

        public static float GetPrivateFloat32(this IPlayerCharacterData data, int hashedKey, float defaultValue = 0f)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PrivateFloats.GetValue(hashedKey, defaultValue);
#else
            return 0f;
#endif
        }

        public static void SetPrivateFloat32(this IPlayerCharacterData data, int hashedKey, float value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PrivateFloats.SetValue(hashedKey, value);
#endif
        }

        public static bool GetPublicBoolean(this IPlayerCharacterData data, int hashedKey, bool defaultValue = false)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicBools.GetValue(hashedKey, defaultValue);
#else
            return false;
#endif
        }

        public static void SetPublicBoolean(this IPlayerCharacterData data, int hashedKey, bool value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PublicBools.SetValue(hashedKey, value);
#endif
        }

        public static int GetPublicInt32(this IPlayerCharacterData data, int hashedKey, int defaultValue = 0)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicInts.GetValue(hashedKey, defaultValue);
#else
            return 0;
#endif
        }

        public static void SetPublicInt32(this IPlayerCharacterData data, int hashedKey, int value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PublicInts.SetValue(hashedKey, value);
#endif
        }

        public static float GetPublicFloat32(this IPlayerCharacterData data, int hashedKey, float defaultValue = 0f)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            return data.PublicFloats.GetValue(hashedKey, defaultValue);
#else
            return 0f;
#endif
        }

        public static void SetPublicFloat32(this IPlayerCharacterData data, int hashedKey, float value)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            data.PublicFloats.SetValue(hashedKey, value);
#endif
        }
    }
}
