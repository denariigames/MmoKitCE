using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterMount
    {
        public BaseSkill GetSkill()
        {
            if (type != MountType.Skill)
                return null;
            if (GameInstance.Skills.TryGetValue(BaseGameData.MakeDataId(sourceId), out BaseSkill skill))
                return skill;
            return null;
        }

        public IMountItem GetMountItem(ICharacterData characterData)
        {
            if (type != MountType.MountItem)
                return null;
            int tempIndexOfData = characterData.IndexOfNonEquipItem(sourceId);
            if (tempIndexOfData < 0)
                return null;
            BaseItem tempItem = characterData.NonEquipItems[tempIndexOfData].GetItem();
            if (tempItem.IsMount())
                return tempItem as IMountItem;
            return null;
        }

        public BuffMount GetBuffMount(ICharacterData characterData)
        {
            if (type != MountType.Buff)
                return null;
            int tempIndexOfData = characterData.IndexOfBuff(sourceId);
            if (tempIndexOfData < 0)
                return null;
            CalculatedBuff tempCalculatedBuff = characterData.Buffs[tempIndexOfData].GetBuff();
            if (tempCalculatedBuff != null && tempCalculatedBuff.TryGetMount(out BuffMount tempBuffMount))
                return tempBuffMount;
            return null;
        }

        public bool ShouldRemove(ICharacterData characterData)
        {
            switch (type)
            {
                case MountType.Skill:
                    BaseSkill skill = GetSkill();
                    if (skill == null || !skill.TryGetMount(out SkillMount mount))
                        return true;
                    if (mount.NoDuration)
                        return false;
                    return mountRemainsDuration <= 0f;
                case MountType.MountItem:
                    IMountItem mountItem = GetMountItem(characterData);
                    if (mountItem == null)
                        return true;
                    if (mountItem.NoMountDuration)
                        return false;
                    return mountRemainsDuration <= 0f;
                case MountType.Buff:
                    BuffMount buffMount = GetBuffMount(characterData);
                    if (buffMount == null)
                        return true;
                    return false;
                case MountType.Custom:
                    // TODO: Implement this
                    return false;
            }
            return false;
        }

        public void Update(IVehicleEntity vehicleEntity, float deltaTime)
        {
            if (mountRemainsDuration > 0f)
            {
                mountRemainsDuration -= deltaTime;
                if (mountRemainsDuration < 0f)
                    mountRemainsDuration = 0f;
            }
            currentHp = vehicleEntity.CurrentHp;
        }
    }

    [System.Serializable]
    public class SyncFieldCharacterMount : LiteNetLibSyncField<CharacterMount>
    {
        protected override bool IsValueChanged(CharacterMount oldValue, CharacterMount newValue)
        {
            return oldValue.type != newValue.type ||
                oldValue.sourceId != newValue.sourceId ||
                oldValue.level != newValue.level ||
                oldValue.mountRemainsDuration != newValue.mountRemainsDuration ||
                oldValue.currentHp != newValue.currentHp;
        }
    }


    [System.Serializable]
    public class SyncListCharacterMount : LiteNetLibSyncList<CharacterMount>
    {
    }
}
