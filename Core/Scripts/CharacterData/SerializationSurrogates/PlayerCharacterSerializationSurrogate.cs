using Insthync.DevExtension;
using Insthync.SerializationSurrogates;
using System.Runtime.Serialization;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class PlayerCharacterSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            PlayerCharacterData data = (PlayerCharacterData)obj;
            info.AddValue("id", data.Id);
            info.AddValue("dataId", data.DataId);
            info.AddValue("entityId", data.EntityId);
            info.AddValue("factionId", data.FactionId);
            info.AddValue("characterName", data.CharacterName);
            info.AddValue("level", data.Level);
            info.AddValue("exp", data.Exp);
            info.AddValue("currentHp", data.CurrentHp);
            info.AddValue("currentMp", data.CurrentMp);
            info.AddValue("currentStamina", data.CurrentStamina);
            info.AddValue("currentFood", data.CurrentFood);
            info.AddValue("currentWater", data.CurrentWater);
            info.AddValue("equipWeaponSet", data.EquipWeaponSet);
            info.AddListValue("selectableWeaponSets", data.SelectableWeaponSets);
            info.AddListValue("attributes", data.Attributes);
            info.AddListValue("skills", data.Skills);
            info.AddListValue("skillUsages", data.SkillUsages);
            info.AddListValue("buffs", data.Buffs);
            info.AddListValue("equipItems", data.EquipItems);
            info.AddListValue("nonEquipItems", data.NonEquipItems);
            info.AddListValue("summons", data.Summons);
            // Player Character
            info.AddValue("statPoint", data.StatPoint);
            info.AddValue("skillPoint", data.SkillPoint);
            info.AddValue("gold", data.Gold);
            info.AddValue("userGold", data.UserGold);
            info.AddValue("userCash", data.UserCash);
            info.AddValue("currentMapName", data.CurrentMapName);
            info.AddValue("currentPosition", (Vector3)data.CurrentPosition);
            info.AddValue("currentRotation", (Vector3)data.CurrentRotation);
#if !DISABLE_DIFFER_MAP_RESPAWNING
            info.AddValue("respawnMapName", data.RespawnMapName);
            info.AddValue("respawnPosition", (Vector3)data.RespawnPosition);
#endif
            info.AddValue("iconDataId", data.IconDataId);
            info.AddValue("frameDataId", data.FrameDataId);
            info.AddValue("titleDataId", data.TitleDataId);
            info.AddValue("lastDeadTime", data.LastDeadTime);
            info.AddValue("lastUpdate", data.LastUpdate);
#if !DISABLE_CLASSIC_PK
            info.AddValue("isPkOn", data.IsPkOn);
            info.AddValue("lastPkOnTime", data.LastPkOnTime);
            info.AddValue("pkPoint", data.PkPoint);
            info.AddValue("consecutivePkKills", data.ConsecutivePkKills);
            info.AddValue("highestPkPoint", data.HighestPkPoint);
            info.AddValue("highestConsecutivePkKills", data.HighestConsecutivePkKills);
#endif
            info.AddValue("reputation", data.Reputation);
            info.AddListValue("hotkeys", data.Hotkeys);
            info.AddListValue("quests", data.Quests);
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            info.AddListValue("currencies", data.Currencies);
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            info.AddListValue("serverBools", data.ServerBools);
            info.AddListValue("serverInts", data.ServerInts);
            info.AddListValue("serverFloats", data.ServerFloats);
            info.AddListValue("privateBools", data.PrivateBools);
            info.AddListValue("privateInts", data.PrivateInts);
            info.AddListValue("privateFloats", data.PrivateFloats);
            info.AddListValue("publicBools", data.PublicBools);
            info.AddListValue("publicInts", data.PublicInts);
            info.AddListValue("publicFloats", data.PublicFloats);
#endif
            info.AddValue("mount", data.Mount);
            info.AddValue("equipWeapons", data.EquipWeapons);
            this.InvokeInstanceDevExtMethods("GetObjectData", obj, info, context);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            PlayerCharacterData data = (PlayerCharacterData)obj;
            data.Id = info.GetString("id");
            data.DataId = info.GetInt32("dataId");
            data.EntityId = info.GetInt32("entityId");
            data.CharacterName = info.GetString("characterName");
            data.Level = info.GetInt32("level");
            data.Exp = info.GetInt32("exp");
            data.CurrentHp = info.GetInt32("currentHp");
            data.CurrentMp = info.GetInt32("currentMp");
            data.CurrentStamina = info.GetInt32("currentStamina");
            data.CurrentFood = info.GetInt32("currentFood");
            data.CurrentWater = info.GetInt32("currentWater");
            data.Attributes = info.GetListValue<CharacterAttribute>("attributes");
            data.Skills = info.GetListValue<CharacterSkill>("skills");
            data.SkillUsages = info.GetListValue<CharacterSkillUsage>("skillUsages");
            data.Buffs = info.GetListValue<CharacterBuff>("buffs");
            data.EquipItems = info.GetListValue<CharacterItem>("equipItems");
            data.NonEquipItems = info.GetListValue<CharacterItem>("nonEquipItems");
            data.Summons = info.GetListValue<CharacterSummon>("summons");
            // Player Character
            data.StatPoint = info.GetSingle("statPoint");
            data.SkillPoint = info.GetSingle("skillPoint");
            data.Gold = info.GetInt32("gold");
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.UserGold = info.GetInt32("userGold");
                data.UserCash = info.GetInt32("userCash");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.FactionId = info.GetInt32("factionId");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.EquipWeaponSet = info.GetByte("equipWeaponSet");
                data.SelectableWeaponSets = info.GetListValue<EquipWeapons>("selectableWeaponSets");
            }
            catch { }
            data.CurrentMapName = info.GetString("currentMapName");
            data.CurrentPosition = (Vector3)info.GetValue("currentPosition", typeof(Vector3));
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.CurrentRotation = (Vector3)info.GetValue("currentRotation", typeof(Vector3));
            }
            catch { }
#if !DISABLE_DIFFER_MAP_RESPAWNING
            data.RespawnMapName = info.GetString("respawnMapName");
            data.RespawnPosition = (Vector3)info.GetValue("respawnPosition", typeof(Vector3));
#endif
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.IconDataId = info.GetInt32("iconDataId");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.FrameDataId = info.GetInt32("frameDataId");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.TitleDataId = info.GetInt32("titleDataId");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.LastDeadTime = info.GetInt64("lastDeadTime");
            }
            catch { }
            data.LastUpdate = info.GetInt64("lastUpdate");
#if !DISABLE_CLASSIC_PK
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.IsPkOn = info.GetBoolean("isPkOn");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.LastPkOnTime = info.GetInt64("lastPkOnTime");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.PkPoint = info.GetInt32("pkPoint");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.ConsecutivePkKills = info.GetInt32("consecutivePkKills");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.HighestPkPoint = info.GetInt32("highestPkPoint");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.HighestConsecutivePkKills = info.GetInt32("highestConsecutivePkKills");
            }
            catch { }
#endif
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.Reputation = info.GetInt32("reputation");
            }
            catch { }
            data.Hotkeys = info.GetListValue<CharacterHotkey>("hotkeys");
            data.Quests = info.GetListValue<CharacterQuest>("quests");
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.Currencies = info.GetListValue<CharacterCurrency>("currencies");
            }
            catch { }
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.ServerBools = info.GetListValue<CharacterDataBoolean>("serverBools");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.ServerInts = info.GetListValue<CharacterDataInt32>("serverInts");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.ServerFloats = info.GetListValue<CharacterDataFloat32>("serverFloats");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.PrivateBools = info.GetListValue<CharacterDataBoolean>("privateBools");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.PrivateInts = info.GetListValue<CharacterDataInt32>("privateInts");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.PrivateFloats = info.GetListValue<CharacterDataFloat32>("privateFloats");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.PublicBools = info.GetListValue<CharacterDataBoolean>("publicBools");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.PublicInts = info.GetListValue<CharacterDataInt32>("publicInts");
            }
            catch { }
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.PublicFloats = info.GetListValue<CharacterDataFloat32>("publicFloats");
            }
            catch { }
#endif
            // TODO: Backward compatible, this will be removed in future version
            try
            {
                data.Mount = (CharacterMount)info.GetValue("mount", typeof(CharacterMount));
            }
            catch { }
            data.EquipWeapons = (EquipWeapons)info.GetValue("equipWeapons", typeof(EquipWeapons));
            this.InvokeInstanceDevExtMethods("SetObjectData", obj, info, context, selector);

            obj = data;
            return obj;
        }
    }
}
