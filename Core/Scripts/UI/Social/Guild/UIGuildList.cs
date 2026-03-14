using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public class UIGuildList : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIGuildListEntry uiDialog;
        public UIGuildListEntry uiPrefab;
        public Transform uiContainer;
        [FormerlySerializedAs("inputGuildName")]
        public InputFieldWrapper inputFind;

        [Header("Options")]
        public GuildListFieldOptions fieldOptions = GuildListFieldOptions.All;

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

        private UIGuildListSelectionManager _cacheSelectionManager;
        public UIGuildListSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIGuildListSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
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
            inputFind = null;
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
            if (inputFind)
                inputFind.text = string.Empty;
            OnClickFind();
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

        protected virtual void OnSelect(UIGuildListEntry ui)
        {
            if (uiDialog != null)
            {
                uiDialog.Data = ui.Data;
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UIGuildListEntry ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        protected virtual void UpdateFoundGuildsUIs(List<GuildListEntry> foundGuilds)
        {
            if (foundGuilds == null)
                return;

            int selectedId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.Id : 0;
            CacheSelectionManager.Clear();

            UIGuildListEntry selectedUI = null;
            UIGuildListEntry tempUI;
            CacheList.Generate(foundGuilds, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIGuildListEntry>();
                tempUI.Data = data;
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if (selectedId == data.Id)
                    selectedUI = tempUI;
            });

            if (listEmptyObject != null)
                listEmptyObject.SetActive(foundGuilds.Count == 0);

            if (selectedUI == null)
            {
                CacheSelectionManager.DeselectSelectedUI();
            }
            else
            {
                selectedUI.SelectByManager();
            }
        }

        public void OnClickFind()
        {
            string guildName = string.Empty;
            if (inputFind != null)
                guildName = inputFind.text;
            RequestFindGuilds(guildName);
        }

        public void OnClickRefresh()
        {
            if (inputFind != null)
                inputFind.text = string.Empty;
            OnClickFind();
        }

        private void RequestFindGuilds(string guildName)
        {
            GameInstance.ClientGuildHandlers.RequestFindGuilds(new RequestFindGuildsMessage()
            {
                fieldOptions = fieldOptions,
                guildName = guildName,
                skip = 0,
                limit = 50,
            }, FindGuildsCallback);
        }

        private void FindGuildsCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseFindGuildsMessage response)
        {
            ClientGuildActions.ResponseFindGuilds(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            UpdateFoundGuildsUIs(response.guilds);
        }
    }
}
