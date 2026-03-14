namespace MultiplayerARPG
{
    public partial class DamageableEntity
    {
        public override void Clean()
        {
            base.Clean();
            combatTextTransform = null;
            opponentAimTransform = null;
            onNormalDamageHit?.RemoveAllListeners();
            onNormalDamageHit = null;
            onCriticalDamageHit?.RemoveAllListeners();
            onCriticalDamageHit = null;
            onBlockedDamageHit?.RemoveAllListeners();
            onBlockedDamageHit = null;
            onDamageMissed?.RemoveAllListeners();
            onDamageMissed = null;
            onCurrentHpChange = null;
            onReceiveDamage = null;
            onReceivedDamage = null;
            SafeArea = null;
            HitBoxes?.Nullify();
            HitBoxes = null;
        }
    }
}