using Cysharp.Text;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class UserDataExtensions
    {
        public static List<UnlockableContent> ReadUnlockableContents(this string unlockableContentsString)
        {
            List<UnlockableContent> unlockableContents = new List<UnlockableContent>();
            string[] splitSets = unlockableContentsString.Split(';');
            foreach (string set in splitSets)
            {
                if (string.IsNullOrEmpty(set))
                    continue;

                string[] splitData = set.Split(':');
                if (splitData.Length != 4)
                    continue;

                if (!byte.TryParse(splitData[0], out byte contentType))
                    continue;

                if (!int.TryParse(splitData[1], out int dataId))
                    dataId = splitData[1].GenerateHashId();

                if (!int.TryParse(splitData[2], out int progression))
                    progression = 0;

                if (!bool.TryParse(splitData[3], out bool unlocked))
                    unlocked = false;

                UnlockableContent unlockableContent = new UnlockableContent();
                unlockableContent.dataId = dataId;
                unlockableContent.type = (UnlockableContentType)contentType;
                unlockableContent.progression = progression;
                unlockableContent.unlocked = unlocked;
                unlockableContents.Add(unlockableContent);
            }
            return unlockableContents;
        }

        public static string WriteUnlockableContents(this List<UnlockableContent> unlockableContents)
        {
            if (unlockableContents == null || unlockableContents.Count == 0)
                return string.Empty;

            using (Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder(false))
            {
                foreach (UnlockableContent unlockableContent in unlockableContents)
                {
                    stringBuilder.Append((byte)unlockableContent.type);
                    stringBuilder.Append(':');
                    stringBuilder.Append(unlockableContent.dataId);
                    stringBuilder.Append(':');
                    stringBuilder.Append(unlockableContent.progression);
                    stringBuilder.Append(':');
                    stringBuilder.Append(unlockableContent.unlocked);
                    stringBuilder.Append(';');
                }
                return stringBuilder.ToString();
            }
        }
    }
}
