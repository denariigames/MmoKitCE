namespace MultiplayerARPG
{
    public static class UITextKeysExtensions
    {
        public static bool IsError(this UITextKeys uiTextKeys)
        {
            return uiTextKeys.ToString().StartsWith("UI_ERROR");
        }
    }
}
