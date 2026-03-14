namespace MultiplayerARPG
{
    public enum BuffRemoveReasons
    {
        Timeout,
        CharacterDead,
        FullStack,
        RemoveByOtherBuffs,
        RemoveByAttackRemoveChance,
        RemoveByAttackedRemoveChance,
        RemoveByUseSkillRemoveChance,
        RemoveByUseItemRemoveChance,
        RemoveByPickupChance,
        RemoveByToggle,
    }
}