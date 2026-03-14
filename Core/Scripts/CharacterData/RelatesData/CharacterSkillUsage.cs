using LiteNetLib.Utils;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterSkillUsage
    {
        public BaseSkill GetSkill()
        {
            if (type == SkillUsageType.Skill && GameInstance.Skills.TryGetValue(dataId, out BaseSkill result))
                return result;
            return null;
        }

        public GuildSkill GetGuildSkill()
        {
            if (type == SkillUsageType.GuildSkill && GameInstance.GuildSkills.TryGetValue(dataId, out GuildSkill result))
                return result;
            return null;
        }

        public IUsableItem GetUsableItem()
        {
            if (type == SkillUsageType.UsableItem && GameInstance.Items.TryGetValue(dataId, out BaseItem item) && item.IsUsable())
                return item as IUsableItem;
            return null;
        }

        public void Use(BaseCharacterEntity character, int level)
        {
            coolDownRemainsDuration = 0f;
            switch (type)
            {
                case SkillUsageType.UsableItem:
                    if (GetUsableItem() != null)
                    {
                        coolDownRemainsDuration = GetUsableItem().UseItemCooldown;
                    }
                    break;
                case SkillUsageType.GuildSkill:
                    if (GetGuildSkill() != null)
                    {
                        coolDownRemainsDuration = GetGuildSkill().GetCoolDownDuration(level);
                    }
                    break;
                case SkillUsageType.Skill:
                    if (GetSkill() != null)
                    {
                        coolDownRemainsDuration = GetSkill().GetCoolDownDuration(level);
                        int tempAmount;
                        // Consume HP
                        tempAmount = GetSkill().GetTotalConsumeHp(level, character);
                        if (tempAmount < 0)
                            tempAmount = 0;
                        character.CurrentHp -= tempAmount;
                        // Consume MP
                        tempAmount = GetSkill().GetTotalConsumeMp(level, character);
                        if (tempAmount < 0)
                            tempAmount = 0;
                        character.CurrentMp -= tempAmount;
                        // Consume Stamina
                        tempAmount = GetSkill().GetTotalConsumeStamina(level, character);
                        if (tempAmount < 0)
                            tempAmount = 0;
                        character.CurrentStamina -= tempAmount;
                        character.ValidateRecovery(character.GetInfo());
                    }
                    break;
            }
        }

        public bool ShouldRemove()
        {
            return coolDownRemainsDuration <= 0f;
        }

        public void Update(float deltaTime)
        {
            coolDownRemainsDuration -= deltaTime;
        }
    }

    [System.Serializable]
    public sealed class SyncListCharacterSkillUsage : LiteNetLibSyncList<CharacterSkillUsage>
    {
        protected override CharacterSkillUsage DeserializeValueForSetOrDirty(int index, NetDataReader reader)
        {
            CharacterSkillUsage result = this[index];
            result.coolDownRemainsDuration = reader.GetFloat();
            return result;
        }

        protected override void SerializeValueForSetOrDirty(int index, NetDataWriter writer, CharacterSkillUsage value)
        {
            writer.Put(value.coolDownRemainsDuration);
        }
    }
}
