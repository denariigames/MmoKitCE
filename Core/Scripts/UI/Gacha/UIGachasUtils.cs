using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class UIGachasUtils
    {
        public static List<Gacha> GetFilteredList(List<Gacha> list, List<string> filterCategories)
        {
            // Prepare result
            List<Gacha> result = new List<Gacha>();
            // Trim filter categories
            for (int i = 0; i < filterCategories.Count; ++i)
            {
                filterCategories[i] = filterCategories[i].Trim().ToLower();
            }
            Gacha entry;
            for (int i = 0; i < list.Count; ++i)
            {
                entry = list[i];
                if (entry == null)
                {
                    // Skip empty data
                    continue;
                }
                string category = (string.IsNullOrEmpty(entry.Category) ? string.Empty : entry.Category).Trim().ToLower();
                if (filterCategories.Count > 0 && !filterCategories.Contains(category))
                {
                    // Category filtering
                    continue;
                }
                result.Add(entry);
            }
            return result;
        }
    }
}
