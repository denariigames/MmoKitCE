using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class UICharacterBuffsUtils
    {
        public static List<CharacterBuff> GetFilteredList(IList<CharacterBuff> list)
        {
            // Prepare result
            List<CharacterBuff> result = new List<CharacterBuff>();
            CharacterBuff entry;
            for (int i = 0; i < list.Count; ++i)
            {
                entry = list[i];
                if (entry.buffRemainsDuration <= 0)
                {
                    // Skip empty data
                    continue;
                }
                result.Add(entry);
            }
            return result;
        }
    }
}
