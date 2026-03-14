using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIPlayerFrameEditing : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIPlayerFrame uiPrefab;
        public Transform uiContainer;
        public UIPlayerFrame[] selectedFrames = new UIPlayerFrame[0];
        public GameObject[] selectedSignObjects = new GameObject[0];
        public GameObject[] notSelectedSignObjects = new GameObject[0];
        public Button uiButtonConfirm;

        [Header("Options")]
        public bool updateSelectedFrameOnSelect;

        private Dictionary<int, UnlockableContent> _availableFrameIds = new Dictionary<int, UnlockableContent>();
        private List<PlayerFrame> _list = new List<PlayerFrame>();

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

        private UIPlayerFrameSelectionManager _cacheSelectionManager;
        public UIPlayerFrameSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIPlayerFrameSelectionManager>();
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
            selectedFrames.Nullify();
            _availableFrameIds?.Clear();
            _list.Nullify();
            _list?.Clear();
            _cacheList = null;
            _cacheSelectionManager = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            LoadAvailableFrames();
        }

        public virtual void LoadAvailableFrames()
        {
            GameInstance.ClientUserContentHandlers.RequestAvailableContents(new RequestAvailableContentsMessage()
            {
                type = UnlockableContentType.Frame,
            }, ResponseAvailableContents);
        }

        protected virtual void ResponseAvailableContents(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAvailableContentsMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _availableFrameIds.Clear();
            for (int i = 0; i < response.contents.Length; ++i)
            {
                if (response.contents[i].unlocked)
                    _availableFrameIds.Add(response.contents[i].dataId, response.contents[i]);
            }
            _list.Clear();
            List<PlayerFrame> availableFrames = new List<PlayerFrame>();
            List<PlayerFrame> unavailableFrames = new List<PlayerFrame>();
            foreach (PlayerFrame frame in GameInstance.PlayerFrames.Values)
            {
                if (_availableFrameIds.ContainsKey(frame.DataId))
                    availableFrames.Add(frame);
                else
                    unavailableFrames.Add(frame);
            }
            _list.AddRange(availableFrames);
            _list.AddRange(unavailableFrames);
            UpdateData(GameInstance.PlayingCharacter.FrameDataId);
        }

        protected virtual void OnSelect(UIPlayerFrame ui)
        {
            UpdateSelectedFrames();
            if (updateSelectedFrameOnSelect)
                UpdateSelectedFrame();

            if (uiButtonConfirm != null)
            {
                uiButtonConfirm.interactable = _availableFrameIds.ContainsKey(ui.Data.DataId);
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

            UIPlayerFrame selectedUI = null;
            UIPlayerFrame tempUI;
            CacheList.Generate(_list, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UIPlayerFrame>();
                tempUI.Data = data;
                tempUI.SetIsLocked(!_availableFrameIds.ContainsKey(data.DataId));
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if ((selectedDataId == 0 && _availableFrameIds.ContainsKey(data.DataId)) || selectedDataId == data.DataId)
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

        public virtual void UpdateSelectedFrames()
        {
            PlayerFrame playerFrame = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            for (int i = 0; i < selectedFrames.Length; ++i)
            {
                selectedFrames[i].Data = playerFrame;
            }
            bool selected = playerFrame != null && _selectedDataId == playerFrame.DataId;
            for (int i = 0; i < selectedSignObjects.Length; ++i)
            {
                selectedSignObjects[i].SetActive(selected);
            }
            for (int i = 0; i < notSelectedSignObjects.Length; ++i)
            {
                notSelectedSignObjects[i].SetActive(!selected);
            }
        }

        public virtual void UpdateSelectedFrame()
        {
            PlayerFrame playerFrame = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data : null;
            GameInstance.ClientCharacterHandlers.RequestSetFrame(new RequestSetFrameMessage()
            {
                dataId = playerFrame.DataId,
            }, ResponseSelectedFrame);
        }

        protected virtual void ResponseSelectedFrame(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSetFrameMessage response)
        {
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            _selectedDataId = response.dataId;
            UpdateSelectedFrames();
        }
    }
}
