using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public partial class UICharacterQuest : UIDataForCharacter<CharacterQuest>
    {
        public CharacterQuest CharacterQuest { get { return Data; } }
        public Quest Quest { get { return CharacterQuest.GetQuest(); } }

        [Header("Generic Info Format")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitleOnGoing = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TITLE_ON_GOING);
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitleTasksComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TITLE_TASKS_COMPLETE);
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitleComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TITLE_COMPLETE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Exp Amount}")]
        public UILocaleKeySetting formatKeyRewardExp = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_EXP);
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyRewardGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_GOLD);
        [Tooltip("Format => {0} = {Stat Points}")]
        public UILocaleKeySetting formatKeyRewardStatPoints = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_STAT_POINTS);
        [Tooltip("Format => {0} = {Skill Points}")]
        public UILocaleKeySetting formatKeyRewardSkillPoints = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_SKILL_POINTS);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public TextWrapper uiTextRewardExp;
        public TextWrapper uiTextRewardGold;
        public TextWrapper uiTextRewardStatPoints;
        public TextWrapper uiTextRewardSkillPoints;
        [Header("Items")]
        public UICharacterItem uiRewardItemDialog;
        public UICharacterItem uiRewardItemPrefab;
        [Header("Reward Items")]
        public bool showRewardItemList;
        public GameObject uiRewardItemRoot;
        public Transform uiRewardItemContainer;
        [Header("Selectable Reward Items")]
        public bool showSelectableRewardItemList;
        public GameObject uiSelectableRewardItemRoot;
        public Transform uiSelectableRewardItemContainer;
        [Header("Random Reward Items")]
        public bool showRandomRewardItemList;
        public GameObject uiRandomRewardItemRoot;
        public Transform uiRandomRewardItemContainer;
        [Header("Reward Currencies")]
        public bool showRewardCurrencies;
        public UICurrencyAmounts uiRewardCurrencies;
        [Header("Quest Tasks")]
        public bool showQuestTaskList;
        public GameObject uiQuestTaskRoot;
        public UIQuestTask uiQuestTaskPrefab;
        public Transform uiQuestTaskContainer;
        [Header("Quest Status")]
        public Toggle toggleQuestTracking;
        [HideInInspector]
        [Tooltip("This is a part of `questOnGoingStatusObjects`, just keep it for backward compatibility.")]
        public GameObject questOnGoingStatusObject;
        public List<GameObject> questOnGoingStatusObjects = new List<GameObject>();
        [HideInInspector]
        [Tooltip("This is a part of `questTasksCompleteStatusObjects`, just keep it for backward compatibility.")]
        public GameObject questTasksCompleteStatusObject;
        public List<GameObject> questTasksCompleteStatusObjects = new List<GameObject>();
        [HideInInspector]
        [Tooltip("This is a part of `questCompleteStatusObjects`, just keep it for backward compatibility.")]
        public GameObject questCompleteStatusObject;
        public List<GameObject> questCompleteStatusObjects = new List<GameObject>();
        [HideInInspector]
        [Tooltip("This is a part of `questIsTrackingObjects`, just keep it for backward compatibility.")]
        public GameObject questIsTrackingObject;
        public List<GameObject> questIsTrackingObjects = new List<GameObject>();
        [HideInInspector]
        [Tooltip("This is a part of `questIsNotTrackingObjects`, just keep it for backward compatibility.")]
        public GameObject questIsNotTrackingObject;
        public List<GameObject> questIsNotTrackingObjects = new List<GameObject>();

        private UIList _cacheRewardItemList;
        public UIList CacheRewardItemList
        {
            get
            {
                if (_cacheRewardItemList == null)
                {
                    _cacheRewardItemList = gameObject.AddComponent<UIList>();
                    _cacheRewardItemList.uiPrefab = uiRewardItemPrefab.gameObject;
                    _cacheRewardItemList.uiContainer = uiRewardItemContainer;
                }
                return _cacheRewardItemList;
            }
        }

        private UIList _cacheSelectableRewardItemList;
        public UIList CacheSelectableRewardItemList
        {
            get
            {
                if (_cacheSelectableRewardItemList == null)
                {
                    _cacheSelectableRewardItemList = gameObject.AddComponent<UIList>();
                    _cacheSelectableRewardItemList.uiPrefab = uiRewardItemPrefab.gameObject;
                    _cacheSelectableRewardItemList.uiContainer = uiSelectableRewardItemContainer;
                }
                return _cacheSelectableRewardItemList;
            }
        }

        private UIList _cacheRandomRewardItemList;
        public UIList CacheRandomRewardItemList
        {
            get
            {
                if (_cacheRandomRewardItemList == null)
                {
                    _cacheRandomRewardItemList = gameObject.AddComponent<UIList>();
                    _cacheRandomRewardItemList.uiPrefab = uiRewardItemPrefab.gameObject;
                    _cacheRandomRewardItemList.uiContainer = uiRandomRewardItemContainer;
                }
                return _cacheRandomRewardItemList;
            }
        }

        private UICharacterItemSelectionManager _cacheRewardItemSelectionManager;
        public UICharacterItemSelectionManager CacheRewardItemSelectionManager
        {
            get
            {
                if (_cacheRewardItemSelectionManager == null)
                    _cacheRewardItemSelectionManager = gameObject.GetOrAddComponent<UICharacterItemSelectionManager>();
                _cacheRewardItemSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheRewardItemSelectionManager;
            }
        }

        private UIList _cacheQuestTaskList;
        public UIList CacheQuestTaskList
        {
            get
            {
                if (_cacheQuestTaskList == null)
                {
                    _cacheQuestTaskList = gameObject.AddComponent<UIList>();
                    _cacheQuestTaskList.uiPrefab = uiQuestTaskPrefab.gameObject;
                    _cacheQuestTaskList.uiContainer = uiQuestTaskContainer;
                }
                return _cacheQuestTaskList;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            MigrateStatusObject();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            uiTextDescription = null;
            uiTextRewardExp = null;
            uiTextRewardGold = null;
            uiTextRewardStatPoints = null;
            uiTextRewardSkillPoints = null;
            uiRewardItemDialog = null;
            uiRewardItemPrefab = null;
            uiRewardItemRoot = null;
            uiRewardItemContainer = null;
            uiSelectableRewardItemRoot = null;
            uiSelectableRewardItemContainer = null;
            uiRandomRewardItemRoot = null;
            uiRandomRewardItemContainer = null;
            uiRewardCurrencies = null;
            uiQuestTaskRoot = null;
            uiQuestTaskPrefab = null;
            uiQuestTaskContainer = null;
            toggleQuestTracking = null;
            questOnGoingStatusObject = null;
            questOnGoingStatusObjects.Nullify();
            questOnGoingStatusObjects?.Clear();
            questTasksCompleteStatusObject = null;
            questTasksCompleteStatusObjects.Nullify();
            questTasksCompleteStatusObjects?.Clear();
            questCompleteStatusObject = null;
            questCompleteStatusObjects.Nullify();
            questCompleteStatusObjects?.Clear();
            questIsTrackingObject = null;
            questIsTrackingObjects.Nullify();
            questIsTrackingObjects?.Clear();
            questIsNotTrackingObject = null;
            questIsNotTrackingObjects.Nullify();
            questIsNotTrackingObjects?.Clear();
            _cacheRewardItemList = null;
            _cacheSelectableRewardItemList = null;
            _cacheRandomRewardItemList = null;
            _cacheRewardItemSelectionManager = null;
            _cacheQuestTaskList = null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (MigrateStatusObject())
                EditorUtility.SetDirty(this);
        }
#endif

        protected bool MigrateStatusObject()
        {
            bool hasChanges = false;
            if (questOnGoingStatusObject != null && !questOnGoingStatusObjects.Contains(questOnGoingStatusObject))
            {
                questOnGoingStatusObjects.Add(questOnGoingStatusObject);
                questOnGoingStatusObject = null;
                hasChanges = true;
            }
            if (questTasksCompleteStatusObject != null && !questTasksCompleteStatusObjects.Contains(questTasksCompleteStatusObject))
            {
                questTasksCompleteStatusObjects.Add(questTasksCompleteStatusObject);
                questTasksCompleteStatusObject = null;
                hasChanges = true;
            }
            if (questCompleteStatusObject != null && !questCompleteStatusObjects.Contains(questCompleteStatusObject))
            {
                questCompleteStatusObjects.Add(questCompleteStatusObject);
                questCompleteStatusObject = null;
                hasChanges = true;
            }
            if (questIsTrackingObject != null && !questIsTrackingObjects.Contains(questIsTrackingObject))
            {
                questIsTrackingObjects.Add(questIsTrackingObject);
                questIsTrackingObject = null;
                hasChanges = true;
            }
            if (questIsNotTrackingObject != null && !questIsNotTrackingObjects.Contains(questIsNotTrackingObject))
            {
                questIsNotTrackingObjects.Add(questIsNotTrackingObject);
                questIsNotTrackingObject = null;
                hasChanges = true;
            }
            return hasChanges;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CacheRewardItemSelectionManager.eventOnSelected.RemoveListener(OnSelectRewardItem);
            CacheRewardItemSelectionManager.eventOnSelected.AddListener(OnSelectRewardItem);
            CacheRewardItemSelectionManager.eventOnDeselected.RemoveListener(OnDeselectRewardItem);
            CacheRewardItemSelectionManager.eventOnDeselected.AddListener(OnDeselectRewardItem);
            if (uiRewardItemDialog != null)
                uiRewardItemDialog.onHide.AddListener(OnRewardItemDialogHide);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (uiRewardItemDialog != null)
                uiRewardItemDialog.onHide.RemoveListener(OnRewardItemDialogHide);
            CacheRewardItemSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnRewardItemDialogHide()
        {
            CacheRewardItemSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnSelectRewardItem(UICharacterItem ui)
        {
            if (ui.Data.characterItem.IsEmptySlot())
            {
                CacheRewardItemSelectionManager.DeselectSelectedUI();
                return;
            }
            if (uiRewardItemDialog != null)
            {
                uiRewardItemDialog.selectionManager = CacheRewardItemSelectionManager;
                ui.CloneTo(uiRewardItemDialog);
                uiRewardItemDialog.Show();
            }
        }

        protected virtual void OnDeselectRewardItem(UICharacterItem ui)
        {
            if (uiRewardItemDialog != null)
            {
                uiRewardItemDialog.onHide.RemoveListener(OnRewardItemDialogHide);
                uiRewardItemDialog.Hide();
                uiRewardItemDialog.onHide.AddListener(OnRewardItemDialogHide);
            }
        }

        protected override void UpdateUI()
        {
            Quest quest = !Data.IsEmpty() ? Data.GetQuest() : null;
            if (quest != null && showQuestTaskList)
            {
                UIQuestTask tempUiQuestTask;
                CacheQuestTaskList.Generate(quest.GetTasks(Data.randomTasksIndex), (index, task, ui) =>
                {
                    tempUiQuestTask = ui.GetComponent<UIQuestTask>();
                    bool isComplete = false;
                    int progress = Data.GetProgress(GameInstance.PlayingCharacter, index, out isComplete);
                    tempUiQuestTask.Data = new UIQuestTaskData(task, progress);
                    tempUiQuestTask.Show();
                });
            }

            bool isComplete = CharacterQuest.isComplete;
            bool isAllTasksDone = CharacterQuest.IsAllTasksDone(GameInstance.PlayingCharacter, out bool hasCompleteAfterTalkedTask);

            string titleFormat = isComplete ?
                LanguageManager.GetText(formatKeyTitleComplete) :
                (isAllTasksDone && !hasCompleteAfterTalkedTask ?
                    LanguageManager.GetText(formatKeyTitleTasksComplete) :
                    LanguageManager.GetText(formatKeyTitleOnGoing));

            if (uiTextTitle != null)
                uiTextTitle.text = ZString.Format(titleFormat, Quest == null ? LanguageManager.GetUnknowTitle() : Quest.Title);
        }

        protected override void UpdateData()
        {
            bool isComplete = CharacterQuest.isComplete;
            bool isTracking = CharacterQuest.isTracking;
            bool isAllTasksDone = CharacterQuest.IsAllTasksDone(GameInstance.PlayingCharacter, out bool hasCompleteAfterTalkedTask);

            string titleFormat = isComplete ?
                LanguageManager.GetText(formatKeyTitleComplete) :
                (isAllTasksDone && !hasCompleteAfterTalkedTask ?
                    LanguageManager.GetText(formatKeyTitleTasksComplete) :
                    LanguageManager.GetText(formatKeyTitleOnGoing));

            if (uiTextTitle != null)
                uiTextTitle.text = ZString.Format(titleFormat, Quest == null ? LanguageManager.GetUnknowTitle() : Quest.Title);

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    Quest == null ? LanguageManager.GetUnknowDescription() : Quest.Description);
            }

            if (uiTextRewardExp != null)
            {
                uiTextRewardExp.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRewardExp),
                    Quest == null ? "0" : Quest.rewardExp.ToString("N0"));
            }

            if (uiTextRewardGold != null)
            {
                uiTextRewardGold.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRewardGold),
                    Quest == null ? "0" : Quest.rewardGold.ToString("N0"));
            }

            if (uiTextRewardStatPoints != null)
            {
                uiTextRewardStatPoints.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRewardStatPoints),
                    Quest == null ? "0" : Quest.rewardStatPoints.ToString("N0"));
            }

            if (uiTextRewardSkillPoints != null)
            {
                uiTextRewardSkillPoints.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRewardSkillPoints),
                    Quest == null ? "0" : Quest.rewardSkillPoints.ToString("N0"));
            }

            // Prepare reward items
            CacheRewardItemSelectionManager.Clear();

            // Reward Items
            if (Quest != null && showRewardItemList)
            {
                CacheRewardItemList.Generate(Quest.rewardItems, (index, rewardItem, ui) =>
                {
                    UICharacterItem uiCharacterItem = ui.GetComponent<UICharacterItem>();
                    uiCharacterItem.Setup(new UICharacterItemData(CharacterItem.Create(rewardItem.item, 1, rewardItem.amount), InventoryType.NonEquipItems), GameInstance.PlayingCharacter, -1);
                    uiCharacterItem.Show();
                    CacheRewardItemSelectionManager.Add(uiCharacterItem);
                });
            }

            if (uiRewardItemRoot != null)
                uiRewardItemRoot.SetActive(showRewardItemList && Quest.rewardItems.Length > 0);

            // Selectable Reward Items
            if (Quest != null && showSelectableRewardItemList)
            {
                CacheSelectableRewardItemList.Generate(Quest.selectableRewardItems, (index, SelectablerewardItem, ui) =>
                {
                    UICharacterItem uiCharacterItem = ui.GetComponent<UICharacterItem>();
                    uiCharacterItem.Setup(new UICharacterItemData(CharacterItem.Create(SelectablerewardItem.item, 1, SelectablerewardItem.amount), InventoryType.NonEquipItems), GameInstance.PlayingCharacter, -1);
                    uiCharacterItem.Show();
                    CacheRewardItemSelectionManager.Add(uiCharacterItem);
                });
            }

            if (uiSelectableRewardItemRoot != null)
                uiSelectableRewardItemRoot.SetActive(showSelectableRewardItemList && Quest.selectableRewardItems.Length > 0);

            // Random Reward Items
            if (Quest != null && showRandomRewardItemList)
            {
                CacheRandomRewardItemList.Generate(Quest.randomRewardItems, (index, RandomrewardItem, ui) =>
                {
                    UICharacterItem uiCharacterItem = ui.GetComponent<UICharacterItem>();
                    uiCharacterItem.Setup(new UICharacterItemData(CharacterItem.Create(RandomrewardItem.item, 1, RandomrewardItem.maxAmount), InventoryType.NonEquipItems), GameInstance.PlayingCharacter, -1);
                    uiCharacterItem.Show();
                    CacheRewardItemSelectionManager.Add(uiCharacterItem);
                });
            }

            if (uiRandomRewardItemRoot != null)
                uiRandomRewardItemRoot.SetActive(showRandomRewardItemList && Quest.randomRewardItems.Length > 0);

            CacheRewardItemSelectionManager.DeselectSelectedUI();

            // Reward Currencies
            if (uiRewardCurrencies != null)
            {
                if (showRewardCurrencies)
                {
                    uiRewardCurrencies.Show();
                    uiRewardCurrencies.Data = Quest.CacheRewardCurrencies;
                }
                else
                {
                    uiRewardCurrencies.Hide();
                }
            }

            // Quest tasks
            if (uiQuestTaskRoot != null)
                uiQuestTaskRoot.SetActive(showQuestTaskList && Quest.GetTasks(Data.randomTasksIndex).Length > 0);

            // Quest status
            foreach (GameObject obj in questOnGoingStatusObjects)
            {
                if (obj != null) obj.SetActive(!isComplete && !isAllTasksDone);
            }

            foreach (GameObject obj in questTasksCompleteStatusObjects)
            {
                if (obj != null) obj.SetActive(!isComplete && isAllTasksDone);
            }

            foreach (GameObject obj in questCompleteStatusObjects)
            {
                if (obj != null) obj.SetActive(isComplete);
            }

            foreach (GameObject obj in questIsTrackingObjects)
            {
                if (obj != null) obj.SetActive(isTracking);
            }

            foreach (GameObject obj in questIsNotTrackingObjects)
            {
                if (obj != null) obj.SetActive(!isTracking);
            }

            if (toggleQuestTracking != null)
                toggleQuestTracking.SetIsOnWithoutNotify(Data.isTracking);
        }

        public void OnToggleTracking(bool isOn)
        {
            GameInstance.PlayingCharacterEntity.CallCmdChangeQuestTracking(Data.dataId, isOn);
        }
    }
}
