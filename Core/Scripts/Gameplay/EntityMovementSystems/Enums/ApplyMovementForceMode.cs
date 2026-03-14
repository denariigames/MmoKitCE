namespace MultiplayerARPG
{
    public enum ApplyMovementForceMode : byte
    {
        Default,
        ReplaceMovement,
        Dash,
    }

    public static class ApplyMovementForceModeExtensions
    {
        public static bool IsReplaceMovement(this ApplyMovementForceMode mode)
        {
            switch (mode)
            {
                case ApplyMovementForceMode.ReplaceMovement:
                case ApplyMovementForceMode.Dash:
                    return true;
                default:
                    return false;
            }
        }
    }
}