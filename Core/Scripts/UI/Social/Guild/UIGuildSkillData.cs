namespace MultiplayerARPG
{
    public struct UIGuildSkillData
    {
        public GuildSkill guildSkill;
        public int targetLevel;
        public UIGuildSkillData(GuildSkill guildSkill, int targetLevel)
        {
            this.guildSkill = guildSkill;
            this.targetLevel = targetLevel;
        }
    }
}
