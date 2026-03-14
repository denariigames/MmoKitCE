using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIPlayerTitleEditing : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIPlayerTitle uiPrefab;
        public Transform uiContainer;
        public UIPlayerTitle[] selectedTitles = new UIPlayerTitle[0];
        public GameObject[] selectedSignObjects = new GameObject[0];
        public GameObject[] notSelectedSignObjects = new GameObject[0];
        public Button uiButtonConfirm;

        [Header("Options")]
        public bool updateSelectedTitleOnSelect;

        private Dictionary<int, UnlockableContent> _availableTitleIds = new Dictionary<int, UnlockableContent>();
        private List<PlayerTitle> _list = new List<PlayerTitle>();

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

        private UIPlayerTitleSelectionManager _cacheSelectionManager;
        public UIPlayerTitleSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIPlayerTitleSelectionManager>();
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
            selectedTitles.Nullify();
            _availableTitleIds?.Clear();
            _list.Nullify();
            _list?.Clear();
            _cacheList = null;
            _cacheSelectionManager = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            LoadAvailableTitles();
        }

        public virtual void LoadAvailableTitles()
        {
            GameInstance.ClientUserContentHandlers.RequestAvailableContents(new RequestAvailableContentsMessage()
            {
                type = UnlockableContentType.Title,
            }, ResponseAvailableContents);
        }

        protected virtual void ResponseAvailableContents(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAvailableContentsMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _availableTitleIds.Clear();
            for (int i = 0; i < response.contents.Length; ++i)
            {
                if (response.contents[i].unlocked)
                    _availableTitleIds.Add(response.contents[i].dataId, response.contents[i]);
            }
            _list.Clear();
            List<PlayerTitle> availableTitles = new List<PlayerTitle>();
            List<PlayerTitle> unavailableTitles = new List<PlayerTitle>();
            foreach (PlayerTitle title in GameInstance.PlayerTitles.Values)
            {
                if (_availableTitleIds.ContainsKey(title.DataId))
                    availableTitles.Add(title);
                else
                    unavailableTitles.Add(title);
            }
            _list.AddRange(availableTitles);
            _list.AddRange(unavailableTitles);
            UpdateData(GameInstance.PlayingCharacter.TitleDataId);
        }

        protected virtual void OnSelect(UIPlayerTitle ui)
        {
            UpdateSelectedTitles();
            if (updateSelectedTitleOnSelect)
                UpdateSelectedTitle();

            if (uiButtonConfirm != null)
            {
                uiButtonConfirm.interactable = _availableTitleIds.ContainsKey(ui.Data.DataId);
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

            UIPlayerTitle selectedUI = null;
            UIPlayerTitle tempUI;
            CacheList.Generate(_list, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIPlayerTitle>();
                tempUI.Data = data;
                tempUI.SetIsLocked(!_availableTitleIds.ContainsKey(data.DataId));
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if ((selectedDataId == 0 && _availableTitleIds.ContainsKey(data.DataId)) || selectedDataId == data.DataId)
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

        public virtual void UpdateSelectedTitles()
        {
            PlayerTitle playerTitle = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            for (int i = 0; i < selectedTitles.Length; ++i)
            {
                selectedTitles[i].Data = playerTitle;
            }
            bool selected = playerTitle != null && _selectedDataId == playerTitle.DataId;
            for (int i = 0; i < selectedSignObjects.Length; ++i)
            {
                selectedSignObjects[i].SetActive(selected);
            }
            for (int i = 0; i < notSelectedSignObjects.Length; ++i)
            {
                notSelectedSignObjects[i].SetActive(!selected);
            }
        }

        public virtual void UpdateSelectedTitle()
        {
            PlayerTitle playerTitle = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            GameInstance.ClientCharacterHandlers.RequestSetTitle(new RequestSetTitleMessage()
            {
                dataId = playerTitle.DataId,
            }, ResponseSelectedTitle);
        }

        protected virtual void ResponseSelectedTitle(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSetTitleMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _selectedDataId = response.dataId;
            UpdateSelectedTitles();
        }
    }
}
