using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIPlayerBackgroundEditing : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIPlayerBackground uiPrefab;
        public Transform uiContainer;
        public UIPlayerBackground[] selectedBackgrounds = new UIPlayerBackground[0];
        public GameObject[] selectedSignObjects = new GameObject[0];
        public GameObject[] notSelectedSignObjects = new GameObject[0];
        public Button uiButtonConfirm;

        [Header("Options")]
        public bool updateSelectedBackgroundOnSelect;

        private Dictionary<int, UnlockableContent> _availableBackgroundIds = new Dictionary<int, UnlockableContent>();
        private List<PlayerBackground> _list = new List<PlayerBackground>();

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

        private UIPlayerBackgroundSelectionManager _cacheSelectionManager;
        public UIPlayerBackgroundSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIPlayerBackgroundSelectionManager>();
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
            selectedBackgrounds.Nullify();
            _availableBackgroundIds?.Clear();
            _list.Nullify();
            _list?.Clear();
            _cacheList = null;
            _cacheSelectionManager = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            LoadAvailableBackgrounds();
        }

        public virtual void LoadAvailableBackgrounds()
        {
            GameInstance.ClientUserContentHandlers.RequestAvailableContents(new RequestAvailableContentsMessage()
            {
                type = UnlockableContentType.Background,
            }, ResponseAvailableContents);
        }

        protected virtual void ResponseAvailableContents(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAvailableContentsMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _availableBackgroundIds.Clear();
            for (int i = 0; i < response.contents.Length; ++i)
            {
                if (response.contents[i].unlocked)
                    _availableBackgroundIds.Add(response.contents[i].dataId, response.contents[i]);
            }
            _list.Clear();
            List<PlayerBackground> available = new List<PlayerBackground>();
            List<PlayerBackground> unavailable = new List<PlayerBackground>();
            foreach (PlayerBackground background in GameInstance.PlayerBackgrounds.Values)
            {
                if (_availableBackgroundIds.ContainsKey(background.DataId))
                    available.Add(background);
                else
                    unavailable.Add(background);
            }
            _list.AddRange(available);
            _list.AddRange(unavailable);
            UpdateData(GameInstance.PlayingCharacter.BackgroundDataId);
        }

        protected virtual void OnSelect(UIPlayerBackground ui)
        {
            UpdateSelectedBackgrounds();
            if (updateSelectedBackgroundOnSelect)
                UpdateSelectedBackground();

            if (uiButtonConfirm != null)
            {
                uiButtonConfirm.interactable = _availableBackgroundIds.ContainsKey(ui.Data.DataId);
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

            UIPlayerBackground selectedUI = null;
            UIPlayerBackground tempUI;
            CacheList.Generate(_list, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIPlayerBackground>();
                tempUI.Data = data;
                tempUI.SetIsLocked(!_availableBackgroundIds.ContainsKey(data.DataId));
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if ((selectedDataId == 0 && _availableBackgroundIds.ContainsKey(data.DataId)) || selectedDataId == data.DataId)
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

        public virtual void UpdateSelectedBackgrounds()
        {
            PlayerBackground playerBackground = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            for (int i = 0; i < selectedBackgrounds.Length; ++i)
            {
                selectedBackgrounds[i].Data = playerBackground;
            }
            bool selected = playerBackground != null && _selectedDataId == playerBackground.DataId;
            for (int i = 0; i < selectedSignObjects.Length; ++i)
            {
                selectedSignObjects[i].SetActive(selected);
            }
            for (int i = 0; i < notSelectedSignObjects.Length; ++i)
            {
                notSelectedSignObjects[i].SetActive(!selected);
            }
        }

        public virtual void UpdateSelectedBackground()
        {
            PlayerBackground playerBackground = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            GameInstance.ClientCharacterHandlers.RequestSetBackground(new RequestSetBackgroundMessage()
            {
                dataId = playerBackground.DataId,
            }, ResponseSelectedBackground);
        }

        protected virtual void ResponseSelectedBackground(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSetBackgroundMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _selectedDataId = response.dataId;
            UpdateSelectedBackgrounds();
        }
    }
}
