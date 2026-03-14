namespace MultiplayerARPG
{
    [System.Flags]
    public enum WeaponHandlingState : byte
    {
        None = 0,
        IsLeftHand = 1 << 0, // 1
        IsAiming = 1 << 1, // 2
        IsFpsView = 1 << 2, // 4
        IsShoulderView = 1 << 3, // 8
    }

    public static class WeaponHandlingStateExtensions
    {
        public static bool Has(this WeaponHandlingState self, WeaponHandlingState flag)
        {
            return (self & flag) == flag;
        }
    }
}
