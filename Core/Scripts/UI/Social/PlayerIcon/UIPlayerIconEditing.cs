using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIPlayerIconEditing : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIPlayerIcon uiPrefab;
        public Transform uiContainer;
        public UIPlayerIcon[] selectedIcons = new UIPlayerIcon[0];
        public GameObject[] selectedSignObjects = new GameObject[0];
        public GameObject[] notSelectedSignObjects = new GameObject[0];
        public Button uiButtonConfirm;

        [Header("Options")]
        public bool updateSelectedIconOnSelect;

        private Dictionary<int, UnlockableContent> _availableIconIds = new Dictionary<int, UnlockableContent>();
        private List<PlayerIcon> _list = new List<PlayerIcon>();

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

        private UIPlayerIconSelectionManager _cacheSelectionManager;
        public UIPlayerIconSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIPlayerIconSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _cacheSelectionManager;
            }
        }

        private int _selectedDataId;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            listEmptyObject = null;
            uiPrefab = null;
            uiContainer = null;
            selectedIcons.Nullify();
            _availableIconIds?.Clear();
            _list.Nullify();
            _list?.Clear();
            _cacheList = null;
            _cacheSelectionManager = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            LoadAvailableIcons();
        }

        public virtual void LoadAvailableIcons()
        {
            GameInstance.ClientUserContentHandlers.RequestAvailableContents(new RequestAvailableContentsMessage()
            {
                type = UnlockableContentType.Icon,
            }, ResponseAvailableContents);
        }

        protected virtual void ResponseAvailableContents(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAvailableContentsMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _availableIconIds.Clear();
            for (int i = 0; i < response.contents.Length; ++i)
            {
                if (response.contents[i].unlocked)
                    _availableIconIds.Add(response.contents[i].dataId, response.contents[i]);
            }
            _list.Clear();
            List<PlayerIcon> availableIcons = new List<PlayerIcon>();
            List<PlayerIcon> unavailableIcons = new List<PlayerIcon>();
            foreach (PlayerIcon icon in GameInstance.PlayerIcons.Values)
            {
                if (_availableIconIds.ContainsKey(icon.DataId))
                    availableIcons.Add(icon);
                else
                    unavailableIcons.Add(icon);
            }
            _list.AddRange(availableIcons);
            _list.AddRange(unavailableIcons);
            UpdateData(GameInstance.PlayingCharacter.IconDataId);
        }

        protected virtual void OnSelect(UIPlayerIcon ui)
        {
            UpdateSelectedIcons();
            if (updateSelectedIconOnSelect)
                UpdateSelectedIcon();

            if (uiButtonConfirm != null)
            {
                uiButtonConfirm.interactable = _availableIconIds.ContainsKey(ui.Data.DataId);
            }
        }

        public void UpdateData()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.DataId : 0;
            UpdateData(selectedDataId);
        }

        public virtual void UpdateData(int selectedDataId)
        {
            _selectedDataId = selectedDataId;
            CacheSelectionManager.Clear();

            if (_list.Count == 0)
            {
                CacheSelectionManager.DeselectSelectedUI();
                CacheList.HideAll();
                if (listEmptyObject != null)
                    listEmptyObject.SetActive(true);
                return;
            }

            if (listEmptyObject != null)
                listEmptyObject.SetActive(false);

            UIPlayerIcon selectedUI = null;
            UIPlayerIcon tempUI;
            CacheList.Generate(_list, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIPlayerIcon>();
                tempUI.Data = data;
                tempUI.SetIsLocked(!_availableIconIds.ContainsKey(data.DataId));
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if ((selectedDataId == 0 && _availableIconIds.ContainsKey(data.DataId)) || selectedDataId == data.DataId)
                {
                    selectedDataId = data.DataId;
                    selectedUI = tempUI;
                }
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
            PlayerIcon playerIcon = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            for (int i = 0; i < selectedIcons.Length; ++i)
            {
                selectedIcons[i].Data = playerIcon;
            }
            bool selected = playerIcon != null && _selectedDataId == playerIcon.DataId;
            for (int i = 0; i < selectedSignObjects.Length; ++i)
            {
                selectedSignObjects[i].SetActive(selected);
            }
            for (int i = 0; i < notSelectedSignObjects.Length; ++i)
            {
                notSelectedSignObjects[i].SetActive(!selected);
            }
        }

        public virtual void UpdateSelectedIcon()
        {
            PlayerIcon playerIcon = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            GameInstance.ClientCharacterHandlers.RequestSetIcon(new RequestSetIconMessage()
            {
                dataId = playerIcon.DataId,
            }, ResponseSelectedIcon);
        }

        protected virtual void ResponseSelectedIcon(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSetIconMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _selectedDataId = response.dataId;
            UpdateSelectedIcons();
        }
    }
}
