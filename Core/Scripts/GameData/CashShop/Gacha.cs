using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.GACHA_FILE, menuName = GameDataMenuConsts.GACHA_MENU, order = GameDataMenuConsts.GACHA_ORDER)]
    public partial class Gacha : BaseGameData
    {
        [Category("Gacha Settings")]
        [SerializeField]
        private string externalIconUrl = string.Empty;
        public string ExternalIconUrl { get { return externalIconUrl; } }

        [SerializeField]
        private int singleModeOpenPrice = 10;
        public int SingleModeOpenPrice
        {
            get { return singleModeOpenPrice; }
        }

        [SerializeField]
        private int multipleModeOpenPrice = 100;
        public int MultipleModeOpenPrice
        {
            get { return multipleModeOpenPrice; }
        }

        [SerializeField]
        private int multipleModeOpenCount = 11;
        public int MultipleModeOpenCount
        {
            get { return multipleModeOpenCount; }
        }

        [Tooltip("An empty items won't be used for item randoming")]
        [ArrayElementTitle("item")]
        [SerializeField]
        private ItemRandomByWeight[] randomItems = new ItemRandomByWeight[0];
        public ItemRandomByWeight[] RandomItems
        {
            get { return randomItems; }
        }

        [System.NonSerialized]
        private List<WeightedRandomizerItem<ItemRandomByWeight>> _cacheRandomItems;
        public List<WeightedRandomizerItem<ItemRandomByWeight>> CacheRandomItems
        {
            get
            {
                if (_cacheRandomItems == null)
                {
                    _cacheRandomItems = new List<WeightedRandomizerItem<ItemRandomByWeight>>();
                    foreach (ItemRandomByWeight item in randomItems)
                    {
                        if (item.item == null || item.maxAmount <= 0 || item.randomWeight <= 0)
                            continue;
                        _cacheRandomItems.Add(new WeightedRandomizerItem<ItemRandomByWeight>()
                        {
                            item = item,
                            weight = item.randomWeight,
                        });
                    }
                }
                return _cacheRandomItems;
            }
        }

        public List<RewardedItem> GetRandomedItems(int count)
        {
            List<RewardedItem> rewardItems = new List<RewardedItem>();
            for (int i = 0; i < count; ++i)
            {
                ItemRandomByWeight randomedItem = WeightedRandomizer.From(CacheRandomItems).TakeOne();
                if (randomedItem.item == null)
                    continue;
                rewardItems.Add(new RewardedItem()
                {
                    item = randomedItem.item,
                    level = randomedItem.GetRandomedLevel(),
                    amount = randomedItem.GetRandomedAmount(),
                    randomSeed = Random.Range(int.MinValue, int.MaxValue),
                });
            }
            return rewardItems;
        }
    }
}
