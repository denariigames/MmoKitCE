namespace MultiplayerARPG
{
    public struct UICharacterSkillData
    {
        public CharacterSkill characterSkill;
        public int targetLevel;
        public UICharacterSkillData(CharacterSkill characterSkill, int targetLevel)
        {
            this.characterSkill = characterSkill;
            this.targetLevel = targetLevel;
        }
        public UICharacterSkillData(CharacterSkill characterSkill) : this(characterSkill, characterSkill.level)
        {
        }
        public UICharacterSkillData(BaseSkill skill, int targetLevel) : this(CharacterSkill.Create(skill, targetLevel), targetLevel)
        {
        }
    }
}
