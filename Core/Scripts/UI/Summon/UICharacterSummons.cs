using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacterSummons : UIBase
    {
        [FormerlySerializedAs("uiSummonDialog")]
        public UICharacterSummon uiDialog;
        [FormerlySerializedAs("uiCharacterSummonPrefab")]
        public UICharacterSummon uiPrefab;
        [FormerlySerializedAs("uiCharacterSummonContainer")]
        public Transform uiContainer;

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

        private UICharacterSummonSelectionManager _cacheSelectionManager;
        public UICharacterSummonSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UICharacterSummonSelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSelectionManager;
            }
        }

        public ICharacterData Character { get; protected set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
            Character = null;
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
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onSummonsOperation += OnSummonsOperation;
        }

        protected virtual void OnDisable()
        {
            if (uiDialog != null)
                uiDialog.onHide.RemoveListener(OnDialogHide);
            CacheSelectionManager.DeselectSelectedUI();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onSummonsOperation -= OnSummonsOperation;
        }

        private void OnSummonsOperation(LiteNetLibSyncListOp operation, int index, CharacterSummon oldItem, CharacterSummon newItem)
        {
            UpdateOwningCharacterData();
        }

        public void UpdateOwningCharacterData()
        {
            if (GameInstance.PlayingCharacter == null) return;
            UpdateData(GameInstance.PlayingCharacter);
        }

        protected virtual void OnDialogHide()
        {
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnSelect(UICharacterSummon ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UICharacterSummon ui)
        {
            if (uiDialog != null)
            {
                uiDialog.onHide.RemoveListener(OnDialogHide);
                uiDialog.Hide();
                uiDialog.onHide.AddListener(OnDialogHide);
            }
        }

        public virtual void UpdateData(ICharacterData character)
        {
            Character = character;
            uint selectedSummonObjectId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.CharacterSummon.objectId : 0;
            CacheSelectionManager.Clear();

            Dictionary<int, UICharacterSummon> stackingSkillSummons = new Dictionary<int, UICharacterSummon>();
            UICharacterSummon selectedUI = null;
            UICharacterSummon tempUI;
            CacheList.Generate(character.Summons, (index, data, ui) =>
            {
                if (data.type == SummonType.Skill && stackingSkillSummons.ContainsKey(data.dataId))
                {
                    stackingSkillSummons[data.dataId].AddStackingEntry(data);
                    ui.gameObject.SetActive(false);
                }
                else
                {
                    tempUI = ui.GetComponent<UICharacterSummon>();
                    tempUI.Setup(data, character, index);
                    tempUI.Show();
                    switch (data.type)
                    {
                        case SummonType.Skill:
                            stackingSkillSummons.Add(data.dataId, tempUI);
                            break;
                        case SummonType.PetItem:
                            ui.transform.SetAsFirstSibling();
                            break;
                    }
                    CacheSelectionManager.Add(tempUI);
                    if (selectedSummonObjectId == data.objectId)
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
    }
}
