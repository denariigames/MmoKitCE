using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIGuildIconEditing : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIGuildIcon uiPrefab;
        public Transform uiContainer;
        public UIGuildIcon[] selectedIcons;

        [Header("Options")]
        [FormerlySerializedAs("updateGuildOptionsOnSelectIcon")]
        public bool updateGuildOptionsOnSelect = false;

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

        private UIGuildIconSelectionManager _cacheSelectionManager;
        public UIGuildIconSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIGuildIconSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _cacheSelectionManager;
            }
        }

        private UIGuildIconUpdater _guildIconUpdater;
        public UIGuildIconUpdater GuildIconUpdater
        {
            get
            {
                if (_guildIconUpdater == null)
                    _guildIconUpdater = gameObject.GetOrAddComponent<UIGuildIconUpdater>();
                return _guildIconUpdater;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            listEmptyObject = null;
            uiPrefab = null;
            uiContainer = null;
            selectedIcons.Nullify();
            _cacheList = null;
            _cacheSelectionManager = null;
            _guildIconUpdater = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            if (GameInstance.JoinedGuild != null)
            {
                // Get current guild options before modify and save
                GuildOptions options = new GuildOptions();
                if (!string.IsNullOrEmpty(GameInstance.JoinedGuild.options))
                    options = JsonConvert.DeserializeObject<GuildOptions>(GameInstance.JoinedGuild.options);
                UpdateData(options.iconDataId);
            }
            else
            {
                UpdateData();
            }
        }

        protected virtual void OnSelect(UIGuildIcon ui)
        {
            UpdateSelectedIcons();
            if (updateGuildOptionsOnSelect)
                UpdateGuildOptions();
        }

        public void UpdateData()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.DataId : 0;
            UpdateData(selectedDataId);
        }

        public virtual void UpdateData(int selectedDataId)
        {
            CacheSelectionManager.Clear();

            List<GuildIcon> list = new List<GuildIcon>(GameInstance.GuildIcons.Values);
            if (list.Count == 0)
            {
                CacheSelectionManager.DeselectSelectedUI();
                CacheList.HideAll();
                if (listEmptyObject != null)
                    listEmptyObject.SetActive(true);
                return;
            }

            if (listEmptyObject != null)
                listEmptyObject.SetActive(false);

            UIGuildIcon selectedUI = null;
            UIGuildIcon tempUI;
            CacheList.Generate(list, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIGuildIcon>();
                tempUI.Data = data;
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if (index == 0 || selectedDataId == data.DataId)
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

        public virtual void UpdateSelectedIcons()
        {
            GuildIcon guildIcon = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            if (selectedIcons != null && selectedIcons.Length > 0)
            {
                foreach (UIGuildIcon selectedIcon in selectedIcons)
                {
                    selectedIcon.Data = guildIcon;
                }
            }
        }

        public virtual void UpdateGuildOptions()
        {
            if (GameInstance.JoinedGuild == null)
            {
                // No joined guild data, so it can't update guild data
                return;
            }
            GuildIconUpdater.UpdateData(CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null);
        }
    }
}
