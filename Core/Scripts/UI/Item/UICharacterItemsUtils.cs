using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class UICharacterItemsUtils
    {
        public delegate List<KeyValuePair<int, CharacterItem>> GetFilteredListDelegate(List<CharacterItem> list, List<string> filterCategories, List<ItemType> filterItemTypes, List<SocketEnhancerType> filterSocketEnhancerTypes, bool doNotShowEmptySlots);

        public static List<KeyValuePair<int, CharacterItem>> GetFilteredList(List<CharacterItem> list, List<string> filterCategories, List<ItemType> filterItemTypes, List<SocketEnhancerType> filterSocketEnhancerTypes, bool doNotShowEmptySlots)
        {
            // Prepare result
            List<KeyValuePair<int, CharacterItem>> result = new List<KeyValuePair<int, CharacterItem>>();
            // Trim filter categories
            for (int i = 0; i < filterCategories.Count; ++i)
            {
                filterCategories[i] = filterCategories[i].Trim().ToLower();
            }
            CharacterItem entry;
            BaseItem tempItem;
            for (int i = 0; i < list.Count; ++i)
            {
                entry = list[i];
                if (entry.IsEmptySlot() && (!GameInstance.Singleton.IsLimitInventorySlot || doNotShowEmptySlots ||
                    filterCategories?.Count > 0 || filterItemTypes?.Count > 0 || filterSocketEnhancerTypes?.Count > 0))
                {
                    // Hide empty slot
                    continue;
                }
                tempItem = entry.GetItem();
                if (tempItem == null)
                {
                    // Add empty slots
                    result.Add(new KeyValuePair<int, CharacterItem>(i, entry));
                    continue;
                }
                string category = (string.IsNullOrEmpty(tempItem.Category) ? string.Empty : tempItem.Category).Trim().ToLower();
                if (filterCategories?.Count > 0 && !filterCategories.Contains(category))
                {
                    // Category filtering
                    continue;
                }
                if (filterItemTypes?.Count > 0 && !filterItemTypes.Contains(tempItem.ItemType))
                {
                    // Item type filtering
                    continue;
                }
                if (tempItem.IsSocketEnhancer())
                {
                    ISocketEnhancerItem socketEnhancerItem = tempItem as ISocketEnhancerItem;
                    if (filterSocketEnhancerTypes?.Count > 0 && !filterSocketEnhancerTypes.Contains(socketEnhancerItem.SocketEnhancerType))
                    {
                        // Socket Enhancer Type filtering
                        continue;
                    }
                }
                result.Add(new KeyValuePair<int, CharacterItem>(i, entry));
            }
            return result;
        }
    }
}
