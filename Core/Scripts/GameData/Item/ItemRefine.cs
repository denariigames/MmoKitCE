using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.ITEM_REFINE_FILE, menuName = GameDataMenuConsts.ITEM_REFINE_MENU, order = GameDataMenuConsts.ITEM_REFINE_ORDER)]
    public partial class ItemRefine : BaseGameData
    {
        [Category("Item Refine Settings")]
        [SerializeField]
        private Color titleColor = Color.clear;
        public Color TitleColor { get { return titleColor; } }

        [SerializeField]
        [Tooltip("This is refine level, each level have difference success rate, required items, required gold")]
        private ItemRefineLevel[] levels = new ItemRefineLevel[0];
        public ItemRefineLevel[] Levels { get { return levels; } }

        [SerializeField]
        [Tooltip("This is repair prices, should order from high to low durability rate")]
        private ItemRepairPrice[] repairPrices = new ItemRepairPrice[0];
        public ItemRepairPrice[] RepairPrices { get { return repairPrices; } }

        public Color GetTitleColor(int level)
        {
            int index = level - 2;
            if (index > 0 && index < Levels.Length && Levels[index].TitleColor.a != 0)
                return Levels[index].TitleColor;
            if (index >= Levels.Length && Levels[Levels.Length - 1].TitleColor.a != 0)
                return Levels[Levels.Length - 1].TitleColor;
            return TitleColor;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (Levels != null && Levels.Length > 0)
            {
                foreach (ItemRefineLevel entry in Levels)
                {
                    GameInstance.AddItems(entry.RequireItems);
                    GameInstance.AddCurrencies(entry.RequireCurrencies);
                }
            }
            if (RepairPrices != null && RepairPrices.Length > 0)
            {
                foreach (ItemRepairPrice entry in RepairPrices)
                {
                    GameInstance.AddItems(entry.RequireItems);
                    GameInstance.AddCurrencies(entry.RequireCurrencies);
                }
            }
        }
    }

    [System.Serializable]
    public partial class ItemRefineEnhancer
    {
        public BaseItem item;
        [Range(0f, 1f)]
        public float increaseSuccessRate = 0f;
        [Range(0f, 1f)]
        public float decreaseRequireGoldRate = 0f;
        [Range(0f, 1f)]
        public float chanceToNotDecreaseLevels = 0f;
        [Range(0f, 1f)]
        public float chanceToNotDestroyItem = 0f;
    }

    [System.Serializable]
    public partial class ItemRefineFailReturning
    {
        public int randomWeight = 0;
        public ItemAmount[] returnItems = new ItemAmount[0];
        public CurrencyAmount[] returnCurrencies = new CurrencyAmount[0];
        public int returnGold = 0;
    }

    [System.Serializable]
    public partial class ItemRefineLevel
    {
        [SerializeField]
        public Color titleColor = Color.clear;
        public Color TitleColor { get { return titleColor; } }
        
        [Range(0.01f, 1f)]
        [SerializeField]
        private float successRate = 0.01f;
        public float SuccessRate { get { return successRate; } }

        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemAmount[] requireItems = new ItemAmount[0];
        public ItemAmount[] RequireItems { get { return requireItems; } }

        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] requireCurrencies = new CurrencyAmount[0];
        public CurrencyAmount[] RequireCurrencies { get { return requireCurrencies; } }

        [SerializeField]
        private int requireGold = 0;
        public int RequireGold { get { return requireGold; } }

        [Tooltip("How many levels it will be decreased if refining failed")]
        [SerializeField]
        private int refineFailDecreaseLevels = 0;
        public int RefineFailDecreaseLevels { get { return refineFailDecreaseLevels; } }

        [Tooltip("It will be destroyed if this value is TRUE and refining failed")]
        [SerializeField]
        private bool refineFailDestroyItem = false;
        public bool RefineFailDestroyItem { get { return refineFailDestroyItem; } }

        [Tooltip("Materials for item refinement enhancing")]
        [SerializeField]
        private ItemRefineEnhancer[] availableEnhancers = new ItemRefineEnhancer[0];
        public ItemRefineEnhancer[] AvailableEnhancers { get { return availableEnhancers; } }

        [Tooltip("Items/currencies which will be returned when refining failed")]
        [SerializeField]
        private ItemRefineFailReturning[] failReturnings = new ItemRefineFailReturning[0];
        public ItemRefineFailReturning[] FailReturnings { get { return failReturnings; } }

        public bool CanRefine(IPlayerCharacterData character, int[] materialDataIds)
        {
            return CanRefine(character, materialDataIds, out _);
        }

        public bool CanRefine(IPlayerCharacterData character, int[] materialDataIds, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            float decreaseRequireGoldRate = 0f;
            for (int i = 0; i < materialDataIds.Length; ++i)
            {
                int materialDataId = materialDataIds[i];
                int indexOfMaterial = character.IndexOfNonEquipItem(materialDataId);
                if (indexOfMaterial >= 0 && !character.NonEquipItems[indexOfMaterial].IsEmptySlot())
                {
                    for (int j = 0; j < AvailableEnhancers.Length; ++j)
                    {
                        if (AvailableEnhancers[j].item != null && AvailableEnhancers[j].item.DataId == materialDataId)
                        {
                            decreaseRequireGoldRate += AvailableEnhancers[j].decreaseRequireGoldRate;
                            break;
                        }
                    }
                }
            }
            if (!GameInstance.Singleton.GameplayRule.CurrenciesEnoughToRefineItem(character, this, decreaseRequireGoldRate))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                return false;
            }
            if (requireItems == null || requireItems.Length == 0)
                return true;
            // Count required items
            foreach (ItemAmount requireItem in requireItems)
            {
                if (requireItem.item != null && character.CountNonEquipItems(requireItem.item.DataId) < requireItem.amount)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }
    }

    [System.Serializable]
    public partial class ItemRepairPrice : System.IComparable<ItemRepairPrice>
    {
        [Range(0.01f, 1f)]
        [SerializeField]
        private float durabilityRate;
        public float DurabilityRate { get { return durabilityRate; } }

        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemAmount[] requireItems = new ItemAmount[0];
        public ItemAmount[] RequireItems { get { return requireItems; } }

        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] requireCurrencies = new CurrencyAmount[0];
        public CurrencyAmount[] RequireCurrencies { get { return requireCurrencies; } }

        [SerializeField]
        private int requireGold = 0;
        public int RequireGold { get { return requireGold; } }

        public ItemRepairPrice(
            float durabilityRate,
            ItemAmount[] requireItems,
            CurrencyAmount[] requireCurrencies,
            int requireGold)
        {
            this.durabilityRate = durabilityRate;
            this.requireItems = requireItems;
            this.requireCurrencies = requireCurrencies;
            this.requireGold = requireGold;
        }

        public bool CanRepair(IPlayerCharacterData character)
        {
            return CanRepair(character, out _);
        }

        public bool CanRepair(IPlayerCharacterData character, out UITextKeys gameMessage)
        {
            gameMessage = UITextKeys.NONE;
            if (!GameInstance.Singleton.GameplayRule.CurrenciesEnoughToRepairItem(character, this))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                return false;
            }
            if (requireItems == null || requireItems.Length == 0)
                return true;
            // Count required items
            foreach (ItemAmount requireItem in requireItems)
            {
                if (requireItem.item != null && character.CountNonEquipItems(requireItem.item.DataId) < requireItem.amount)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }
            }
            return true;
        }

        public int CompareTo(ItemRepairPrice other)
        {
            return durabilityRate.CompareTo(other.durabilityRate);
        }
    }
}
