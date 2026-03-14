using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.ITEM_RANDOM_BY_WEIGHT_TABLE_FILE, menuName = GameDataMenuConsts.ITEM_RANDOM_BY_WEIGHT_TABLE_MENU, order = GameDataMenuConsts.ITEM_RANDOM_BY_WEIGHT_TABLE_ORDER)]
    public class ItemRandomByWeightTable : ScriptableObject
    {
        [Tooltip("Can set empty item as a chance to not drop any items")]
        [ArrayElementTitle("item")]
        public ItemRandomByWeight[] randomItems = new ItemRandomByWeight[0];
        public float noDropWeight = 0f;

        [Header("Drop Test Tool")]
        public int dropTestRound = 100;
        [InspectorButton(nameof(ProceedDropTest), "Proceed Drop Test")]
        public bool btnProceedDropTest;

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
                        if (item.randomWeight <= 0)
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

        public virtual void PrepareRelatesData()
        {
            GameInstance.AddItems(randomItems);
        }

        public void RandomItem(OnDropItemDelegate onRandomItem, int seed = 0, HashSet<int> excludeItemDataIds = null, System.Action onFailed = null)
        {
            ItemRandomByWeight randomedItem;
            if (CacheRandomItems.Count > 1 && excludeItemDataIds != null && excludeItemDataIds.Count > 0)
            {
                using (CollectionPool<List<WeightedRandomizerItem<ItemRandomByWeight>>, WeightedRandomizerItem<ItemRandomByWeight>>.Get(out List<WeightedRandomizerItem<ItemRandomByWeight>> randomItems))
                {
                    foreach (var kv in CacheRandomItems)
                    {
                    	if (!kv.item.item || excludeItemDataIds.Contains(kv.item.item.DataId))
                        	continue;
                        randomItems.Add(new WeightedRandomizerItem<ItemRandomByWeight>()
                        {
                            item = kv.item,
                            weight = kv.weight,
                        });
                    }
                    randomedItem = WeightedRandomizer.From(randomItems, noDropWeight).TakeOne(seed);
                }
            }
            else
            {
                randomedItem = WeightedRandomizer.From(CacheRandomItems, noDropWeight).TakeOne(seed);
            }
            if (randomedItem.item == null)
            {
                onFailed?.Invoke();
                return;
            }
            onRandomItem?.Invoke(randomedItem.item, randomedItem.GetRandomedLevel(), randomedItem.GetRandomedAmount());
        }

        public void ProceedDropTest()
        {
            ProceedDropTestBySpecificRounds(dropTestRound);
        }

        public void ProceedDropTestBySpecificRounds(int dropTestRound)
        {
            _cacheRandomItems = null;
            Dictionary<BaseItem, int> itemAmounts = new Dictionary<BaseItem, int>();
            Debug.Log("== Start Drop Test == ");
            for (int i = 0; i < dropTestRound; ++i)
            {
                Debug.Log($"=== Drop Test Round {i + 1} ===");
                int j = 0;
                RandomItem((BaseItem item, int level, int amount) =>
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

#if UNITY_EDITOR
        [ContextMenu("Set ammo drop amount to max stack")]
        public void SetAmmoDropAmountToMaxStack()
        {
            for (int i = 0; i < randomItems.Length; ++i)
            {
                ItemRandomByWeight randomItem = randomItems[i];
                if (randomItem.item == null)
                    continue;
                if (randomItem.item is IAmmoItem ammoItem)
                {
                    randomItem.minAmount = ammoItem.MaxStack;
                    randomItem.maxAmount = ammoItem.MaxStack;
                    randomItems[i] = randomItem;
                }
            }
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Show Total Weight")]
        public void ShowTotalWeight()
        {
            float totalWeight = 0;
            foreach (var item in randomItems)
            {
                totalWeight += item.randomWeight;
            }
            totalWeight += noDropWeight;
            Debug.Log($"Total Weight: {totalWeight}");
        }
#endif
    }
}
