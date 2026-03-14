using System.Collections.Generic;
using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICashPackages : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Cash Amount}")]
        public UILocaleKeySetting formatKeyCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CASH);

        [Header("Filter")]
        public List<string> filterCategories = new List<string>();

        [Header("UI Elements")]
        public GameObject listEmptyObject;
        [FormerlySerializedAs("uiCashPackageDialog")]
        public UICashPackage uiDialog;
        [FormerlySerializedAs("uiCashPackagePrefab")]
        public UICashPackage uiPrefab;
        [FormerlySerializedAs("uiCashPackageContainer")]
        public Transform uiContainer;
        public TextWrapper uiTextCash;
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

        private UICashPackageSelectionManager _cacheSelectionManager;
        public UICashPackageSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UICashPackageSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        public List<CashPackage> LoadedList { get; private set; } = new List<CashPackage>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            filterCategories?.Clear();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            uiTextCash = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            LoadedList.Nullify();
            LoadedList?.Clear();
        }

        public void Refresh()
        {
            // Load cash shop item list
            GameInstance.ClientCashShopHandlers.RequestCashPackageInfo(ResponseInfo);
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

        protected virtual void OnSelect(UICashPackage ui)
        {
            if (uiDialog != null && ui.Data != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                uiDialog.uiCashPackages = this;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UICashPackage ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public void Buy(string productId)
        {
            GameInstance.PurchaseCallback = ResponsePurchase;
            GameInstance.Singleton.Purchase(productId);
        }

        private void ResponsePurchase(bool success, string errorMessage)
        {
            if (!success)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), errorMessage);
                return;
            }
            Refresh();
        }

        protected virtual void ResponseInfo(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashPackageInfoMessage response)
        {
            ClientCashShopActions.ResponseCashPackageInfo(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;

            if (uiTextCash != null)
            {
                uiTextCash.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCash),
                    response.cash.ToString("N0"));
            }

            LoadedList.Clear();
            foreach (int cashPackageId in response.cashPackageIds)
            {
                CashPackage cashPackage;
                if (GameInstance.CashPackages.TryGetValue(cashPackageId, out cashPackage))
                {
                    LoadedList.Add(cashPackage);
                }
            }

            GenerateList();
        }

        public virtual void GenerateList()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.DataId : 0;
            CacheSelectionManager.Clear();

            List<CashPackage> filteredList = UICashPackagesUtils.GetFilteredList(LoadedList, filterCategories);
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

            UICashPackage selectedUI = null;
            UICashPackage tempUI;
            CacheList.Generate(filteredList, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICashPackage>();
                tempUI.uiCashPackages = this;
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
