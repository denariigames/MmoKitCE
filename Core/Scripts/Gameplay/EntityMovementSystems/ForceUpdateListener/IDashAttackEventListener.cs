namespace MultiplayerARPG
{
    public interface IDashAttackEventListener
    {
        float DashMovingTriggerInterval { get; }
        void OnPreDashAttack(BaseCharacterEntity user, int skillLevel);
        void OnDashMovingToAttack(BaseCharacterEntity user, int skillLevel);
        void OnPostDashAttack(BaseCharacterEntity user, int skillLevel);
    }
}
