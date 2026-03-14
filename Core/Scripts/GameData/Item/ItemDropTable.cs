using Insthync.UnityEditorUtils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.ITEM_DROP_TABLE_FILE, menuName = GameDataMenuConsts.ITEM_DROP_TABLE_MENU, order = GameDataMenuConsts.ITEM_DROP_TABLE_ORDER)]
    public class ItemDropTable : ScriptableObject
    {
        [ArrayElementTitle("item")]
        public ItemDrop[] randomItems;
        [ArrayElementTitle("currency")]
        public CurrencyRandomAmount[] randomCurrencies;

#if UNITY_EDITOR
        [ContextMenu("Set ammo drop amount to max stack")]
        public void SetAmmoDropAmountToMaxStack()
        {
            for (int i = 0; i < randomItems.Length; ++i)
            {
                ItemDrop randomItem = randomItems[i];
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
#endif
    }
}
