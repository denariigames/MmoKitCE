using System;
using System.Runtime.Serialization;

namespace MultiplayerARPG
{
    public class PlayerCharacterDataTypeBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == "PlayerCharacterData")
                return typeof(PlayerCharacterData);
            if (typeName == "CharacterAttribute")
                return typeof(CharacterAttribute);
            if (typeName == "CharacterBuff")
                return typeof(CharacterBuff);
            if (typeName == "CharacterHotkey")
                return typeof(CharacterHotkey);
            if (typeName == "CharacterItem")
                return typeof(CharacterItem);
            if (typeName == "CharacterQuest")
                return typeof(CharacterQuest);
            if (typeName == "CharacterSkill")
                return typeof(CharacterSkill);
            if (typeName == "CharacterSkillUsage")
                return typeof(CharacterSkillUsage);
            if (typeName == "CharacterSummon")
                return typeof(CharacterSummon);
            if (typeName == "EquipWeapons")
                return typeof(EquipWeapons);
            if (typeName == "StorageCharacterItem")
                return typeof(StorageCharacterItem);
            if (typeName == "StorageSaveData")
                return typeof(StorageSaveData);
            if (typeName == "BuildingSaveData")
                return typeof(BuildingSaveData);
            if (typeName == "WorldSaveData")
                return typeof(WorldSaveData);
            if (typeName == "SummonBuffsSaveData")
                return typeof(SummonBuffsSaveData);
            return null;
        }
    }
}
