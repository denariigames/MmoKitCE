using System.Collections.Generic;
using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICashShop : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Cash Amount}")]
        public UILocaleKeySetting formatKeyCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CASH);

        [Header("Filter")]
        public List<string> filterCategories = new List<string>();

        [Header("UI Elements")]
        public GameObject listEmptyObject;
        [FormerlySerializedAs("uiCashShopItemDialog")]
        public UICashShopItem uiDialog;
        [FormerlySerializedAs("uiCashShopItemPrefab")]
        public UICashShopItem uiPrefab;
        [FormerlySerializedAs("uiCashShopItemContainer")]
        public Transform uiContainer;
        public TextWrapper uiTextCash;
        public UIRewarding uiRewarding;
        public bool selectFirstEntryByDefault;

        private UIList _cacheList;
        public UIList CacheList
        {
            get
            {
                if (_cacheList == null)
                {
                    _cacheList = gameObject.AddComponent<UIList>();
                    _cacheList.uiPrefab = uiPrefab.gameObject;
                    _cacheList.uiContainer = uiContainer;
                }
                return _cacheList;
            }
        }

        private UICashShopSelectionManager _cacheSelectionManager;
        public UICashShopSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UICashShopSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        public List<CashShopItem> LoadedList { get; private set; } = new List<CashShopItem>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            filterCategories?.Clear();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            uiTextCash = null;
            uiRewarding = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            LoadedList.Nullify();
            LoadedList?.Clear();
        }

        public void Refresh()
        {
            // Load cash shop item list
            GameInstance.ClientCashShopHandlers.RequestCashShopInfo(ResponseInfo);
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelect.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelect.AddListener(OnSelect);
            CacheSelectionManager.eventOnDeselect.RemoveListener(OnDeselect);
            CacheSelectionManager.eventOnDeselect.AddListener(OnDeselect);
            if (uiDialog != null)
                uiDialog.onHide.AddListener(OnDialogHide);
            Refresh();
        }

        protected virtual void OnDisable()
        {
            if (uiDialog != null)
                uiDialog.onHide.RemoveListener(OnDialogHide);
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnDialogHide()
        {
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnSelect(UICashShopItem ui)
        {
            if (uiDialog != null && ui.Data != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                uiDialog.uiCashShop = this;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UICashShopItem ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public void Buy(int dataId, CashShopItemCurrencyType currencyType, int amount)
        {
            GameInstance.ClientCashShopHandlers.RequestCashShopBuy(new RequestCashShopBuyMessage()
            {
                dataId = dataId,
                currencyType = currencyType,
                amount = amount,
            }, ResponseBuy);
        }

        protected virtual void ResponseInfo(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashShopInfoMessage response)
        {
            ClientCashShopActions.ResponseCashShopInfo(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;

            if (uiTextCash != null)
            {
                uiTextCash.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCash),
                    response.cash.ToString("N0"));
            }

            LoadedList.Clear();
            foreach (int cashShopItemId in response.cashShopItemIds)
            {
                CashShopItem cashShopItem;
                if (GameInstance.CashShopItems.TryGetValue(cashShopItemId, out cashShopItem))
                    LoadedList.Add(cashShopItem);
            }

            GenerateList();
        }

        protected virtual void ResponseBuy(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashShopBuyMessage response)
        {
            ClientCashShopActions.ResponseCashShopBuy(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            if (uiRewarding == null)
            {
                // Try use ref from global one
                uiRewarding = UISceneGlobal.Singleton.uiRewarding;
            }
            if (uiRewarding != null)
            {
                uiRewarding.Data = new UIRewardingData()
                {
                    rewardGold = response.rewardGold,
                    rewardItems = response.rewardItems,
                };
                uiRewarding.Show();
            }
            else
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_SUCCESS.ToString()), LanguageManager.GetText(UITextKeys.UI_CASH_SHOP_ITEM_BOUGHT.ToString()));
            }
            Refresh();
        }

        public virtual void GenerateList()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.DataId : 0;
            CacheSelectionManager.Clear();

            List<CashShopItem> filteredList = UICashShopUtils.GetFilteredList(LoadedList, filterCategories);
            if (filteredList.Count == 0)
            {
                CacheSelectionManager.DeselectSelectedUI();
                if (uiDialog != null)
                    uiDialog.Hide();
                CacheList.HideAll();
                if (listEmptyObject != null)
                    listEmptyObject.SetActive(true);
                return;
            }

            if (listEmptyObject != null)
                listEmptyObject.SetActive(false);

            UICashShopItem selectedUI = null;
            UICashShopItem tempUI;
            CacheList.Generate(filteredList, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICashShopItem>();
                tempUI.uiCashShop = this;
                tempUI.Data = data;
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if ((selectFirstEntryByDefault && index == 0) || selectedDataId == data.DataId)
                    selectedUI = tempUI;
            });

            if (selectedUI == null)
            {
                CacheSelectionManager.DeselectSelectedUI();
            }
            else
            {
                selectedUI.SelectByManager();
            }
        }
    }
}
