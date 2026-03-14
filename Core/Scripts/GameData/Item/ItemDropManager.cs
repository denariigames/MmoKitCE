using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class ItemDropManager
    {
        [ArrayElementTitle("item")]
        public ItemDrop[] randomItems = new ItemDrop[0];
        public ItemDropTable[] itemDropTables = new ItemDropTable[0];
        public ItemRandomByWeightTable[] itemRandomByWeightTables = new ItemRandomByWeightTable[0];
        public ItemRandomByWeightTable[] certainItemRandomByWeightTables = new ItemRandomByWeightTable[0];
        [Tooltip("Min kind of items that will be dropped in ground")]
        [Min(0)]
        public byte minDropItems = 1;
        [Tooltip("Max kind of items that will be dropped in ground")]
        [Min(1)]
        public byte maxDropItems = 5;
        [Tooltip("If true, certain drop items will be limited by random drop items (Randomed by `minDropItems` and `maxDropItems`)")]
        public bool certainDropLimitedByRandomDropItems = false;

        [System.NonSerialized]
        private List<ItemRandomByWeightTable> _cacheItemRandomByWeightTables = null;
        public List<ItemRandomByWeightTable> CacheItemRandomByWeightTables
        {
            get
            {
                if (_cacheItemRandomByWeightTables == null)
                {
                    _cacheItemRandomByWeightTables = new List<ItemRandomByWeightTable>();
                    if (itemRandomByWeightTables != null)
                        _cacheItemRandomByWeightTables.AddRange(itemRandomByWeightTables);
                }
                return _cacheItemRandomByWeightTables;
            }
        }

        [System.NonSerialized]
        private List<ItemDrop> _certainDropItems = new List<ItemDrop>();
        [System.NonSerialized]
        private List<ItemDrop> _uncertainDropItems = new List<ItemDrop>();

        [System.NonSerialized]
        private List<ItemDrop> _cacheRandomItems = null;
        public List<ItemDrop> CacheRandomItems
        {
            get
            {
                if (_cacheRandomItems == null)
                {
                    int i;
                    _cacheRandomItems = new List<ItemDrop>();
                    if (randomItems != null &&
                        randomItems.Length > 0)
                    {
                        for (i = 0; i < randomItems.Length; ++i)
                        {
                            if (randomItems[i].item == null ||
                                randomItems[i].maxAmount <= 0 ||
                                randomItems[i].dropRate <= 0)
                                continue;
                            _cacheRandomItems.Add(randomItems[i]);
                        }
                    }
                    if (itemDropTables != null &&
                        itemDropTables.Length > 0)
                    {
                        foreach (ItemDropTable itemDropTable in itemDropTables)
                        {
                            if (itemDropTable == null ||
                                itemDropTable.randomItems == null ||
                                itemDropTable.randomItems.Length <= 0)
                            {
                                continue;
                            }

                            for (i = 0; i < itemDropTable.randomItems.Length; ++i)
                            {
                                if (itemDropTable.randomItems[i].item == null ||
                                    itemDropTable.randomItems[i].maxAmount <= 0 ||
                                    itemDropTable.randomItems[i].dropRate <= 0)
                                    continue;
                                _cacheRandomItems.Add(itemDropTable.randomItems[i]);
                            }
                        }
                    }
                    _cacheRandomItems.Sort((a, b) => b.dropRate.CompareTo(a.dropRate));
                    _certainDropItems.Clear();
                    _uncertainDropItems.Clear();
                    for (i = 0; i < _cacheRandomItems.Count; ++i)
                    {
                        if (_cacheRandomItems[i].item == null)
                            continue;
                        if (_cacheRandomItems[i].dropRate >= 1f)
                            _certainDropItems.Add(_cacheRandomItems[i]);
                        else
                            _uncertainDropItems.Add(_cacheRandomItems[i]);
                    }
                }
                return _cacheRandomItems;
            }
        }

        public void PrepareRelatesData()
        {
            GameInstance.AddItems(CacheRandomItems);
            if (itemRandomByWeightTables != null)
            {
                foreach (ItemRandomByWeightTable entry in itemRandomByWeightTables)
                {
                    if (entry == null)
                        continue;
                    GameInstance.AddItems(entry.randomItems);
                }
            }
        }

        public virtual void RandomItems(OnDropItemDelegate onRandomItem, float rate = 1f)
        {
            if (CacheRandomItems.Count == 0 && CacheItemRandomByWeightTables.Count == 0)
                return;
            using (CollectionPool<HashSet<int>, int>.Get(out HashSet<int> excludeItemDataIds))
            {
                int randomDropCount = 0;
                int targetRandomDropCount = Random.Range(minDropItems, maxDropItems + 1);
                int i;
                // Drop certain drop rate items
                for (i = 0; i < certainItemRandomByWeightTables.Length && (!certainDropLimitedByRandomDropItems || randomDropCount < targetRandomDropCount); ++i)
                {
                    certainItemRandomByWeightTables[i].RandomItem((BaseItem item, int level, int amount) =>
                    {
                        onRandomItem?.Invoke(item, level, amount);
                        ++randomDropCount;
                    }, 0, excludeItemDataIds);
                }
                _certainDropItems.Shuffle();
                for (i = 0; i < _certainDropItems.Count && (!certainDropLimitedByRandomDropItems || randomDropCount < targetRandomDropCount); ++i)
                {
                    onRandomItem?.Invoke(_certainDropItems[i].item, _certainDropItems[i].GetRandomedLevel(), _certainDropItems[i].GetRandomedAmount());
                    ++randomDropCount;
                }
                // Reached max drop items?
                if (randomDropCount >= targetRandomDropCount)
                    return;
                // Randoming
                if (_uncertainDropItems.Count == 0 && CacheItemRandomByWeightTables.Count == 0)
                    return;
                do
                {
                    // Drop uncertain drop rate items
                    _uncertainDropItems.Shuffle();
                    for (i = 0; i < _uncertainDropItems.Count && randomDropCount < targetRandomDropCount; ++i)
                    {
                        BaseItem dropItem = _uncertainDropItems[i].item;
                        float dropRate = _uncertainDropItems[i].dropRate * rate;
                        if (Random.value > dropRate)
                            continue;
                        if (BaseGameNetworkManager.CurrentMapInfo != null && BaseGameNetworkManager.CurrentMapInfo.ExcludeItemFromDropping(dropItem))
                            continue;
                        onRandomItem.Invoke(dropItem, _uncertainDropItems[i].GetRandomedLevel(), _uncertainDropItems[i].GetRandomedAmount());
                        ++randomDropCount;
                    }
                    // Reached max drop items?
                    if (randomDropCount >= targetRandomDropCount)
                        return;
                    // Drop items by weighted tables
                    CacheItemRandomByWeightTables.Shuffle();
                    for (i = 0; i < CacheItemRandomByWeightTables.Count && randomDropCount < targetRandomDropCount; ++i)
                    {
                        CacheItemRandomByWeightTables[i].RandomItem((BaseItem item, int level, int amount) =>
                        {
                            onRandomItem?.Invoke(item, level, amount);
                            ++randomDropCount;
                        }, 0, excludeItemDataIds);
                    }
                } while (randomDropCount < targetRandomDropCount);
            }
        }

        public void ProceedDropTest(int dropTestRound)
        {
            _cacheRandomItems = null;
            Dictionary<BaseItem, int> itemAmounts = new Dictionary<BaseItem, int>();
            Debug.Log("== Start Drop Test == ");
            for (int i = 0; i < dropTestRound; ++i)
            {
                Debug.Log($"=== Drop Test Round {i + 1} ===");
                int j = 0;
                RandomItems((BaseItem item, int level, int amount) =>
                {
                    Debug.Log($"==== Drop #{j} - {item}, Lv.{level}, Amt.{amount} ====");
                    if (!itemAmounts.ContainsKey(item))
                        itemAmounts[item] = 0;
                    itemAmounts[item] += amount;
                });
            }
            Debug.Log("== End Drop Test, Summary ==");
            foreach (KeyValuePair<BaseItem, int> itemAmount in itemAmounts)
            {
                Debug.Log($"=== Total Drop {itemAmount.Key}, Amt.{itemAmount.Value} ===");
            }
            itemAmounts.Clear();
        }
    }
}
