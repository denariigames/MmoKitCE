namespace MultiplayerARPG
{
    public interface IItemWithStatusEffectApplyings
    {
        /// <summary>
        /// Status effects that can be applied to the attacker when attacking
        /// </summary>
        StatusEffectApplying[] SelfStatusEffectsWhenAttacking { get; }
        /// <summary>
        /// Status effects that can be applied to the enemy when attacking
        /// </summary>
        StatusEffectApplying[] EnemyStatusEffectsWhenAttacking { get; }
        /// <summary>
        /// Status effects that can be applied to the attacker when attacked
        /// </summary>
        StatusEffectApplying[] SelfStatusEffectsWhenAttacked { get; }
        /// <summary>
        /// Status effects that can be applied to the enemy when attacked
        /// </summary>
        StatusEffectApplying[] EnemyStatusEffectsWhenAttacked { get; }
    }
}