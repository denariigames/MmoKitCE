namespace MultiplayerARPG
{
    public static class DamageableEntityExtensions
    {
        public static bool IsDead(this IDamageableEntity damageableEntity)
        {
            return damageableEntity.IsNull() || damageableEntity.CurrentHp <= 0;
        }

        public static bool IsDeadOrHideFrom(this IDamageableEntity damageableEntity, IGameEntity watcherEntity)
        {
            return damageableEntity.IsDead() || damageableEntity.IsHideFrom(watcherEntity);
        }
    }
}
