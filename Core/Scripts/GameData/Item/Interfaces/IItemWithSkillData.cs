namespace MultiplayerARPG
{
    public interface IItemWithSkillData : IItem
    {
        /// <summary>
        /// Skill data for this item
        /// </summary>
        BaseSkill SkillData { get; }

        /// <summary>
        /// Skill's level for this item
        /// </summary>
        int SkillLevel { get; }
    }
}