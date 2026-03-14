using Cysharp.Text;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.CASH_SHOP_ITEM_FILE, menuName = GameDataMenuConsts.CASH_SHOP_ITEM_MENU, order = GameDataMenuConsts.CASH_SHOP_ITEM_ORDER)]
    public partial class CashShopItem : BaseGameData
    {
        [Category("Cash Shop Item Settings")]
        [SerializeField]
        private string externalIconUrl = string.Empty;
        public string ExternalIconUrl { get { return externalIconUrl; } }

        [FormerlySerializedAs("sellPrice")]
        [SerializeField]
        private int sellPriceCash = 0;
        public int SellPriceCash { get { return sellPriceCash; } }

        [SerializeField]
        private int sellPriceGold = 0;
        public int SellPriceGold { get { return sellPriceGold; } }

        [Tooltip("Gold which character will receives")]
        [SerializeField]
        private int receiveGold = 0;
        public int ReceiveGold { get { return receiveGold; } }

        [SerializeField]
        [ArrayElementTitle("currency")]
        private CurrencyAmount[] receiveCurrencies = new CurrencyAmount[0];
        public CurrencyAmount[] ReceiveCurrencies { get { return receiveCurrencies; } }

        [SerializeField]
        [ArrayElementTitle("item")]
        private ItemAmount[] receiveItems = new ItemAmount[0];
        public ItemAmount[] ReceiveItems { get { return receiveItems; } }

        public override bool Validate()
        {
            bool hasChanges = false;
            for (int i = 0; i < receiveCurrencies.Length; ++i)
            {
                if (receiveCurrencies[i].amount <= 0)
                {
                    Debug.LogWarning("[CashShopItem] Receive Currencies [" + i + "], amount is " + receiveCurrencies[i].amount + " will be changed to 1 (Minimum Value)");
                    hasChanges = true;
                    CurrencyAmount receive = receiveCurrencies[i];
                    receive.amount = 1;
                    receiveCurrencies[i] = receive;
                }
            }
            for (int i = 0; i < receiveItems.Length; ++i)
            {
                if (receiveItems[i].amount <= 0)
                {
                    Debug.LogWarning("[CashShopItem] Receive Items [" + i + "], amount is " + receiveItems[i].amount + " will be changed to 1 (Minimum Value)");
                    hasChanges = true;
                    ItemAmount receive = receiveItems[i];
                    receive.amount = 1;
                    receiveItems[i] = receive;
                }
            }
            return hasChanges || base.Validate();
        }

        public CashShopItem GenerateByItem(BaseItem item, CashShopItemGeneratingData generatingData)
        {
            List<string> languageKeys = new List<string>(LanguageManager.Languages.Keys);
            List<LanguageData> titleLanguageDataList = new List<LanguageData>();
            List<LanguageData> descriptionLanguageDataList = new List<LanguageData>();
            defaultTitle = ZString.Format(LanguageManager.GetText(UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_TITLE.ToString()), item.DefaultTitle, generatingData.amount);
            defaultDescription = ZString.Format(LanguageManager.GetText(UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_DESCRIPTION.ToString()), item.DefaultTitle, generatingData.amount, defaultDescription);
            foreach (string languageKey in languageKeys)
            {
                titleLanguageDataList.Add(new LanguageData()
                {
                    key = languageKey,
                    value = ZString.Format(LanguageManager.GetTextByLanguage(languageKey, UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_TITLE.ToString()), Language.GetTextByLanguageKey(item.LanguageSpecificTitles, languageKey, item.DefaultTitle), generatingData.amount),
                });
                descriptionLanguageDataList.Add(new LanguageData()
                {
                    key = languageKey,
                    value = ZString.Format(LanguageManager.GetTextByLanguage(languageKey, UIFormatKeys.UI_FORMAT_GENERATE_CAST_SHOP_ITEM_DESCRIPTION.ToString()), Language.GetTextByLanguageKey(item.LanguageSpecificTitles, languageKey, item.DefaultTitle), generatingData.amount, Language.GetTextByLanguageKey(item.LanguageSpecificDescriptions, languageKey, item.DefaultDescription)),
                });
            }
            languageSpecificTitles = titleLanguageDataList.ToArray();
            languageSpecificDescriptions = descriptionLanguageDataList.ToArray();
            category = item.Category;
#if UNITY_EDITOR || !UNITY_SERVER
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            icon = item.Icon;
#endif
#if !DISABLE_ADDRESSABLES
            addressableIcon = item.AddressableIcon;
#endif
#endif
            sellPriceCash = generatingData.sellPriceCash;
            sellPriceGold = generatingData.sellPriceGold;
            receiveItems = new ItemAmount[]
            {
                    new ItemAmount()
                    {
                        item = item,
                        amount = generatingData.amount,
                    }
            };
            return this;
        }
    }
}
