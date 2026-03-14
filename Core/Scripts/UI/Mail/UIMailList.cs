using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class UIMailList : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIMail uiDialog;
        public UIMailListEntry uiPrefab;
        public Transform uiContainer;

        [Header("Events")]
        public UnityEvent onRefresh = new UnityEvent();
        public UnityEvent onClaimAllMailsItems = new UnityEvent();
        public UnityEvent onDeleteAllMails = new UnityEvent();

        [Header("Options")]
        public bool onlyNewMails = false;

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

        private UIMailListSelectionManager _cacheSelectionManager;
        public UIMailListSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIMailListSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _cacheSelectionManager;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            onRefresh?.RemoveAllListeners();
            onRefresh = null;
            onClaimAllMailsItems?.RemoveAllListeners();
            onClaimAllMailsItems = null;
            onDeleteAllMails?.RemoveAllListeners();
            onDeleteAllMails = null;
            _cacheList = null;
            _cacheSelectionManager = null;
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

        protected virtual void OnSelect(UIMailListEntry ui)
        {
            if (uiDialog != null)
            {
                MailListEntry data = ui.Data;
                uiDialog.uiMailList = this;
                uiDialog.MailId = data.Id;
                uiDialog.Show();
                data.IsRead = true;
                ui.Data = data;
                ui.ForceUpdate();
            }
        }

        protected virtual void OnDeselect(UIMailListEntry ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public void Refresh()
        {
            GameInstance.ClientMailHandlers.RequestMailList(new RequestMailListMessage()
            {
                onlyNewMails = onlyNewMails,
            }, MailListCallback);
            // Update notification count
            UIMailNotification[] notifications = FindObjectsOfType<UIMailNotification>();
            for (int i = 0; i < notifications.Length; ++i)
            {
                notifications[i].Refresh();
            }
        }

        protected virtual void MailListCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseMailListMessage response)
        {
            ClientMailActions.ResponseMailList(requestHandler, responseCode, response);
            string selectedId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.Id : string.Empty;
            CacheSelectionManager.DeselectSelectedUI();
            CacheSelectionManager.Clear();
            if (listEmptyObject != null)
                listEmptyObject.SetActive(true);
            if (responseCode == AckResponseCode.Exception ||
                responseCode == AckResponseCode.Unimplemented ||
                responseCode == AckResponseCode.Timeout)
                return;
            UIMailListEntry tempUI;
            CacheList.Generate(response.mails, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIMailListEntry>();
                tempUI.Data = data;
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if (index == 0 || selectedId.Equals(data.Id))
                    tempUI.SelectByManager();
            });
            if (listEmptyObject != null)
                listEmptyObject.SetActive(response.mails.Count == 0);
            onRefresh.Invoke();
        }

        public void OnClickClaimAll()
        {
            GameInstance.ClientMailHandlers.RequestClaimAllMailsItems(ClaimAllCallback);
        }

        protected virtual void ClaimAllCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseClaimAllMailsItemsMessage response)
        {
            ClientMailActions.ResponseClaimAllMailsItems(requestHandler, responseCode, response);
            CacheSelectionManager.DeselectSelectedUI();
            CacheSelectionManager.Clear();
            Refresh();
            onClaimAllMailsItems.Invoke();
        }

        public void OnClickDeleteAll()
        {
            GameInstance.ClientMailHandlers.RequestDeleteAllMails(DeleteAllCallback);
        }

        protected virtual void DeleteAllCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDeleteAllMailsMessage response)
        {
            ClientMailActions.ResponseDeleteAllMails(requestHandler, responseCode, response);
            CacheSelectionManager.DeselectSelectedUI();
            CacheSelectionManager.Clear();
            Refresh();
            onDeleteAllMails.Invoke();
        }
    }
}
