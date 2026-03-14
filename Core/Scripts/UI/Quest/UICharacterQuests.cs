using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacterQuests : UIBase
    {
        [Header("Filter")]
        public List<string> filterCategories = new List<string>();

        public GameObject listEmptyObject;
        [FormerlySerializedAs("uiQuestDialog")]
        public UICharacterQuest uiDialog;
        [FormerlySerializedAs("uiCharacterQuestPrefab")]
        public UICharacterQuest uiPrefab;
        [FormerlySerializedAs("uiCharacterQuestContainer")]
        public Transform uiContainer;

        [SerializeField]
        private bool hideCompleteQuest;
        public bool HideCompleteQuest
        {
            get { return hideCompleteQuest; }
            set
            {
                if (hideCompleteQuest != value)
                {
                    hideCompleteQuest = value;
                    UpdateData();
                }
            }
        }

        [SerializeField]
        private bool showOnlyTrackingQuests;
        public bool ShowOnlyTrackingQuests
        {
            get { return showOnlyTrackingQuests; }
            set
            {
                if (showOnlyTrackingQuests != value)
                {
                    showOnlyTrackingQuests = value;
                    UpdateData();
                }
            }
        }

        [SerializeField]
        private bool showAllWhenNoTrackedQuests;
        public bool ShowAllWhenNoTrackedQuests
        {
            get { return showAllWhenNoTrackedQuests; }
            set
            {
                if (showAllWhenNoTrackedQuests != value)
                {
                    showAllWhenNoTrackedQuests = value;
                    UpdateData();
                }
            }
        }

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

        private UICharacterQuestSelectionManager _cacheSelectionManager;
        public UICharacterQuestSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UICharacterQuestSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _cacheSelectionManager;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            filterCategories?.Clear();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelect.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelect.AddListener(OnSelect);
            CacheSelectionManager.eventOnDeselect.RemoveListener(OnDeselect);
            CacheSelectionManager.eventOnDeselect.AddListener(OnDeselect);
            UpdateData();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onQuestsOperation += OnQuestsOperation;
        }

        protected virtual void OnDisable()
        {
            CacheSelectionManager.DeselectSelectedUI();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onQuestsOperation -= OnQuestsOperation;
        }

        private void OnQuestsOperation(LiteNetLibSyncListOp operation, int index, CharacterQuest oldItem, CharacterQuest newItem)
        {
            UpdateData();
        }

        protected virtual void OnSelect(UICharacterQuest ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UICharacterQuest ui)
        {
            if (uiDialog != null)
                uiDialog.Hide();
        }

        public void UpdateData()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Data.dataId : 0;
            CacheSelectionManager.DeselectSelectedUI();
            CacheSelectionManager.Clear();

            List<CharacterQuest> filteredList = UICharacterQuestsUtils.GetFilteredList(GameInstance.PlayingCharacter.Quests, ShowOnlyTrackingQuests, ShowAllWhenNoTrackedQuests, HideCompleteQuest, filterCategories);
            if (filteredList.Count == 0)
            {
                if (uiDialog != null)
                    uiDialog.Hide();
                CacheList.HideAll();
                if (listEmptyObject != null)
                    listEmptyObject.SetActive(true);
                return;
            }

            if (listEmptyObject != null)
                listEmptyObject.SetActive(false);

            UICharacterQuest selectedUI = null;
            UICharacterQuest tempUI;
            CacheList.Generate(filteredList, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICharacterQuest>();
                tempUI.Setup(data, GameInstance.PlayingCharacter, index);
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
                if (index == 0 || selectedDataId == data.dataId)
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
