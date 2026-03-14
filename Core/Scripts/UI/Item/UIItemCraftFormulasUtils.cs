using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class UIItemCraftFormulasUtils
    {
        public static List<ItemCraftFormula> GetFilteredList(List<ItemCraftFormula> list, List<string> filterCategories)
        {
            // Prepare result
            List<ItemCraftFormula> result = new List<ItemCraftFormula>();
            // Trim filter categories
            for (int i = 0; i < filterCategories.Count; ++i)
            {
                filterCategories[i] = filterCategories[i].Trim().ToLower();
            }
            ItemCraftFormula entry;
            for (int i = 0; i < list.Count; ++i)
            {
                entry = list[i];
                if (entry == null || entry.ItemCraft.CraftingItem == null)
                {
                    // Skip empty data
                    continue;
                }
                string formulaCategory = (string.IsNullOrEmpty(entry.Category) ? string.Empty : entry.Category).Trim().ToLower();
                string itemCraftCategory = (string.IsNullOrEmpty(entry.ItemCraft.CraftingItem.Category) ? string.Empty : entry.ItemCraft.CraftingItem.Category).Trim().ToLower();
                if (filterCategories.Count > 0 && !filterCategories.Contains(formulaCategory) && !filterCategories.Contains(itemCraftCategory))
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