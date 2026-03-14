using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacterSkills : UIBase
    {
        [Header("Filter")]
        public List<string> filterCategories = new List<string>();
        public List<SkillType> filterSkillTypes = new List<SkillType>();

        [Header("UI Elements")]
        public GameObject listEmptyObject;
        [FormerlySerializedAs("uiSkillDialog")]
        public UICharacterSkill uiDialog;
        [FormerlySerializedAs("uiCharacterSkillPrefab")]
        public UICharacterSkill uiPrefab;
        [FormerlySerializedAs("uiCharacterSkillContainer")]
        public Transform uiContainer;

        [Header("Options")]
        [Tooltip("If this is `TRUE` it won't update data when controlling character's data changes")]
        public bool notForOwningCharacter;

        public bool NotForOwningCharacter
        {
            get { return notForOwningCharacter; }
            set
            {
                notForOwningCharacter = value;
                RegisterOwningCharacterEvents();
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

        private UICharacterSkillSelectionManager _cacheSelectionManager;
        public UICharacterSkillSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UICharacterSkillSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        public ICharacterData Character { get; protected set; }
        public Dictionary<BaseSkill, int> LoadedList { get; private set; } = new Dictionary<BaseSkill, int>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            filterCategories?.Clear();
            filterSkillTypes?.Clear();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            Character = null;
            LoadedList?.Clear();
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.eventOnSelect.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelect.AddListener(OnSelect);
            CacheSelectionManager.eventOnDeselect.RemoveListener(OnDeselect);
            CacheSelectionManager.eventOnDeselect.AddListener(OnDeselect);
            if (uiDialog != null)
                uiDialog.onHide.AddListener(OnDialogHide);
            UpdateOwningCharacterData();
            RegisterOwningCharacterEvents();
        }

        protected virtual void OnDisable()
        {
            if (uiDialog != null)
                uiDialog.onHide.RemoveListener(OnDialogHide);
            CacheSelectionManager.DeselectSelectedUI();
            UnregisterOwningCharacterEvents();
        }

        public void RegisterOwningCharacterEvents()
        {
            UnregisterOwningCharacterEvents();
            if (notForOwningCharacter || !GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onRecached += UpdateOwningCharacterData;
        }

        public void UnregisterOwningCharacterEvents()
        {
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onRecached -= UpdateOwningCharacterData;
        }

        public void UpdateOwningCharacterData()
        {
            if (notForOwningCharacter || GameInstance.PlayingCharacter == null) return;
            UpdateData(GameInstance.PlayingCharacter);
        }

        protected virtual void OnDialogHide()
        {
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnSelect(UICharacterSkill ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UICharacterSkill ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public void UpdateData(ICharacterData character)
        {
            Character = character;
            LoadedList.Clear();
            if (Character != null && Character.GetDatabase() != null)
            {
                LoadedList = new Dictionary<BaseSkill, int>(Character.GetCaches().Skills);
            }
            GenerateList();
        }

        public void UpdateData(ICharacterData character, IDictionary<BaseSkill, int> skills)
        {
            Character = character;
            LoadedList.Clear();
            foreach (KeyValuePair<BaseSkill, int> skill in skills)
            {
                LoadedList[skill.Key] = skill.Value;
            }
            GenerateList();
        }

        public void UpdateData(ICharacterData character, IDictionary<int, int> skills)
        {
            Character = character;
            LoadedList.Clear();
            BaseSkill tempSkill;
            foreach (KeyValuePair<int, int> skill in skills)
            {
                if (GameInstance.Skills.TryGetValue(skill.Key, out tempSkill))
                    LoadedList[tempSkill] = skill.Value;
            }
            GenerateList();
        }

        public void UpdateData(ICharacterData character, IList<SkillLevel> skills)
        {
            Character = character;
            LoadedList.Clear();
            foreach (SkillLevel skill in skills)
            {
                LoadedList[skill.skill] = skill.level;
            }
            GenerateList();
        }

        public virtual void GenerateList()
        {
            int selectedDataId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.Skill.DataId : 0;
            CacheSelectionManager.Clear();

            Dictionary<BaseSkill, int> filteredList = UICharacterSkillsUtils.GetFilteredList(LoadedList, filterCategories, filterSkillTypes);
            if (Character == null || filteredList.Count == 0)
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

            UICharacterSkill selectedUI = null;
            UICharacterSkill tempUI;
            int tempIndexOfLearnedSkill;
            int tempLearnedSkillLevel;
            // Combine skills from database (skill that can level up) with learned skill and equipment skill
            CacheList.Generate(filteredList, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UICharacterSkill>();
                tempIndexOfLearnedSkill = Character.IndexOfSkill(data.Key.DataId);
                tempLearnedSkillLevel = tempIndexOfLearnedSkill >= 0 ? Character.Skills[tempIndexOfLearnedSkill].level : 0;
                // Set UI data, Create new character skill data based on learned skill, target level is sum of learned skill and equipment skill
                tempUI.Setup(new UICharacterSkillData(CharacterSkill.Create(data.Key, tempLearnedSkillLevel), data.Value), Character, tempIndexOfLearnedSkill);
                tempUI.Show();
                UICharacterSkillDragHandler dragHandler = tempUI.GetComponentInChildren<UICharacterSkillDragHandler>();
                if (dragHandler != null)
                    dragHandler.SetupForSkills(tempUI);
                CacheSelectionManager.Add(tempUI);
                if (selectedDataId == data.Key.DataId)
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
