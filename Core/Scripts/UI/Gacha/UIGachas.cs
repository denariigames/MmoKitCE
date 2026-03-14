using System.Collections.Generic;
using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGachas : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Cash Amount}")]
        public UILocaleKeySetting formatKeyCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CASH);

        [Header("Filter")]
        public List<string> filterCategories = new List<string>();

        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIGacha uiDialog;
        public UIGacha uiPrefab;
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

        private UIGachaSelectionManager _cacheSelectionManager;
        public UIGachaSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIGachaSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _cacheSelectionManager;
            }
        }

        public List<Gacha> LoadedList { get; private set; } = new List<Gacha>();

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
            // Load gacha list
            GameInstance.ClientGachaHandlers.RequestGachaInfo(ResponseInfo);
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

        protected virtual void OnSelect(UIGacha ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                uiDialog.uiGachas = this;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UIGacha ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public void Buy(int dataId, GachaOpenMode openMode)
        {
            GameInstance.ClientGachaHandlers.RequestOpenGacha(new RequestOpenGachaMessage()
            {
                dataId = dataId,
                openMode = openMode,
            }, ResponseOpenGacha);
        }

        protected virtual void ResponseInfo(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseGachaInfoMessage response)
        {
            ClientGachaActions.ResponseGachaInfo(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;

            if (uiTextCash != null)
            {
                uiTextCash.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCash),
                    response.cash.ToString("N0"));
            }

            LoadedList.Clear();
            foreach (int gachaId in response.gachaIds)
            {
                Gacha gacha;
                if (GameInstance.Gachas.TryGetValue(gachaId, out gacha))
                    LoadedList.Add(gacha);
            }

            GenerateList();
        }

        protected virtual void ResponseOpenGacha(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseOpenGachaMessage response)
        {
            ClientGachaActions.ResponseOpenGacha(requestHandler, responseCode, response);
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
                    rewardItems = response.rewardItems,
                };
                uiRewarding.Show();
            }
            else
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_SUCCESS.ToString()), LanguageManager.GetText(UITextKeys.UI_GACHA_OPENED.ToString()));
            }
            Refresh();
        }

        public virtual void GenerateList()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.DataId : 0;
            CacheSelectionManager.Clear();

            List<Gacha> filteredList = UIGachasUtils.GetFilteredList(LoadedList, filterCategories);
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

            UIGacha selectedUI = null;
            UIGacha tempUI;
            CacheList.Generate(filteredList, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIGacha>();
                tempUI.uiGachas = this;
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
