namespace MultiplayerARPG
{
    [System.Flags]
    public enum DirectionType2D : byte
    {
        Down = 1 << 0,
        Up = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        DownLeft = Down | Left,
        DownRight = Down | Right,
        UpLeft = Up | Left,
        UpRight = Up | Right,
    }
}
