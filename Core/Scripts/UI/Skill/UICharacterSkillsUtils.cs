using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class UICharacterSkillsUtils
    {
        public static Dictionary<BaseSkill, int> GetFilteredList(Dictionary<BaseSkill, int> list, List<string> filterCategories, List<SkillType> filterSkillTypes)
        {
            // Prepare result
            Dictionary<BaseSkill, int> result = new Dictionary<BaseSkill, int>();
            // Trim filter categories
            for (int i = 0; i < filterCategories.Count; ++i)
            {
                filterCategories[i] = filterCategories[i].Trim().ToLower();
            }
            foreach (KeyValuePair<BaseSkill, int> kvp in list)
            {
                if (kvp.Key == null)
                {
                    // Skip empty data
                    continue;
                }
                string category = (string.IsNullOrEmpty(kvp.Key.Category) ? string.Empty : kvp.Key.Category).Trim().ToLower();
                if (filterCategories.Count > 0 && !filterCategories.Contains(category))
                {
                    // Category filtering
                    continue;
                }
                if (filterSkillTypes.Count > 0 && !filterSkillTypes.Contains(kvp.Key.SkillType))
                {
                    // Skill type filtering
                    continue;
                }
                result.Add(kvp.Key, kvp.Value);
            }
            return result;
        }
    }
}
