namespace MultiplayerARPG
{
    public interface IWeaponAbilityController : IShooterWeaponController
    {
        BaseWeaponAbility WeaponAbility { get; }
        WeaponAbilityState WeaponAbilityState { get; }
    }
}
