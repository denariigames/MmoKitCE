using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class UIFormatKeysExtensions
    {
        public static readonly Dictionary<UIFormatKeys, string> UIFormatKeyStrings = new Dictionary<UIFormatKeys, string>();

        public static string GetCachedString(this UIFormatKeys key)
        {
            if (!UIFormatKeyStrings.TryGetValue(key, out string value))
            {
                value = key.ToString();
                UIFormatKeyStrings[key] = value;
            }
            return value;
        }
    }
}
