using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(GuildSkill))]
    [CanEditMultipleObjects]
    public class GuildSkillEditor : BaseGameDataEditor
    {
        protected override void SetFieldCondition()
        {
            GuildSkill guildSkill = CreateInstance<GuildSkill>();
            // Passive skill
            ShowOnEnum("skillType", nameof(SkillType.Passive), "increaseMaxMember");
            ShowOnEnum("skillType", nameof(SkillType.Passive), "increaseExpGainPercentage");
            ShowOnEnum("skillType", nameof(SkillType.Passive), "increaseGoldGainPercentage");
            ShowOnEnum("skillType", nameof(SkillType.Passive), "increaseShareExpGainPercentage");
            ShowOnEnum("skillType", nameof(SkillType.Passive), "increaseShareGoldGainPercentage");
            ShowOnEnum("skillType", nameof(SkillType.Passive), "decreaseExpLostPercentage");
            // Active skill
            ShowOnEnum("skillType", nameof(SkillType.Active), "coolDownDuration");
            ShowOnEnum("skillType", nameof(SkillType.Active), "buff");
        }
    }
}
