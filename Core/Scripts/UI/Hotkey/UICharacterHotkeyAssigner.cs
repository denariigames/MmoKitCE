using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UICharacterHotkeyAssigner : UIBase
    {
        public UICharacterHotkey uiCharacterHotkey;
        public UICharacterSkill uiCharacterSkillPrefab;
        public UICharacterItem uiCharacterItemPrefab;
        public UIGuildSkill uiGuildSkillPrefab;
        public Transform uiCharacterSkillContainer;
        public Transform uiCharacterItemContainer;
        public Transform uiGuildSkillContainer;
        public bool autoHideIfNothingToAssign = false;
        public bool hideFoundUsableItems = true;

        private UIList _cacheSkillList;
        public UIList CacheSkillList
        {
            get
            {
                if (_cacheSkillList == null)
                {
                    _cacheSkillList = gameObject.AddComponent<UIList>();
                    if (uiCharacterSkillPrefab != null)
                        _cacheSkillList.uiPrefab = uiCharacterSkillPrefab.gameObject;
                    _cacheSkillList.uiContainer = uiCharacterSkillContainer;
                }
                return _cacheSkillList;
            }
        }

        private UIList _cacheItemList;
        public UIList CacheItemList
        {
            get
            {
                if (_cacheItemList == null)
                {
                    _cacheItemList = gameObject.AddComponent<UIList>();
                    if (uiCharacterItemPrefab != null)
                        _cacheItemList.uiPrefab = uiCharacterItemPrefab.gameObject;
                    _cacheItemList.uiContainer = uiCharacterItemContainer;
                }
                return _cacheItemList;
            }
        }

        private UIList _cacheGuildSkillList;
        public UIList CacheGuildSkillList
        {
            get
            {
                if (_cacheGuildSkillList == null)
                {
                    _cacheGuildSkillList = gameObject.AddComponent<UIList>();
                    if (uiGuildSkillPrefab != null)
                        _cacheGuildSkillList.uiPrefab = uiGuildSkillPrefab.gameObject;
                    _cacheGuildSkillList.uiContainer = uiGuildSkillContainer;
                }
                return _cacheGuildSkillList;
            }
        }

        private UICharacterSkillSelectionManager _cacheSkillSelectionManager;
        public UICharacterSkillSelectionManager CacheSkillSelectionManager
        {
            get
            {
                if (_cacheSkillSelectionManager == null)
                    _cacheSkillSelectionManager = gameObject.GetOrAddComponent<UICharacterSkillSelectionManager>();
                _cacheSkillSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheSkillSelectionManager;
            }
        }

        private UICharacterItemSelectionManager _cacheItemSelectionManager;
        public UICharacterItemSelectionManager CacheItemSelectionManager
        {
            get
            {
                if (_cacheItemSelectionManager == null)
                    _cacheItemSelectionManager = gameObject.GetOrAddComponent<UICharacterItemSelectionManager>();
                _cacheItemSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheItemSelectionManager;
            }
        }

        private UIGuildSkillSelectionManager _cacheGuildSkillSelectionManager;
        public UIGuildSkillSelectionManager CacheGuildSkillSelectionManager
        {
            get
            {
                if (_cacheGuildSkillSelectionManager == null)
                    _cacheGuildSkillSelectionManager = gameObject.GetOrAddComponent<UIGuildSkillSelectionManager>();
                _cacheGuildSkillSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _cacheGuildSkillSelectionManager;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiCharacterHotkey = null;
            uiCharacterSkillPrefab = null;
            uiCharacterItemPrefab = null;
            uiGuildSkillPrefab = null;
            uiCharacterSkillContainer = null;
            uiCharacterItemContainer = null;
            uiGuildSkillContainer = null;
            _cacheSkillList = null;
            _cacheItemList = null;
            _cacheGuildSkillList = null;
            _cacheSkillSelectionManager = null;
            _cacheItemSelectionManager = null;
            _cacheGuildSkillSelectionManager = null;
        }

        public void Setup(UICharacterHotkey uiCharacterHotkey)
        {
            this.uiCharacterHotkey = uiCharacterHotkey;
        }

        public override void Show()
        {
            UICharacterHotkeys.SetUsingHotkey(null);
            if (GameInstance.PlayingCharacterEntity == null)
            {
                CacheSkillList.HideAll();
                CacheItemList.HideAll();
                CacheGuildSkillList.HideAll();
                return;
            }
            base.Show();
        }

        public override void OnShow()
        {
            CacheSkillSelectionManager.eventOnSelect.RemoveListener(OnSelectCharacterSkill);
            CacheSkillSelectionManager.eventOnSelect.AddListener(OnSelectCharacterSkill);
            CacheSkillList.doNotRemoveContainerChildren = true;

            CacheItemSelectionManager.eventOnSelect.RemoveListener(OnSelectCharacterItem);
            CacheItemSelectionManager.eventOnSelect.AddListener(OnSelectCharacterItem);
            CacheItemList.doNotRemoveContainerChildren = true;

            CacheGuildSkillSelectionManager.eventOnSelect.RemoveListener(OnSelectGuildSkill);
            CacheGuildSkillSelectionManager.eventOnSelect.AddListener(OnSelectGuildSkill);
            CacheGuildSkillList.doNotRemoveContainerChildren = true;

            int countAssignable = 0;

            // Setup skill list
            UICharacterSkill tempUiCharacterSkill;
            CharacterSkill tempCharacterSkill;
            BaseSkill tempSkill;
            int tempIndexOfSkill;
            CacheSkillList.HideAll();
            CacheSkillList.Generate(GameInstance.PlayingCharacterEntity.GetCaches().Skills, (index, skillLevel, ui) =>
            {
                if (!ui)
                    return;
                tempUiCharacterSkill = ui.GetComponent<UICharacterSkill>();
                tempSkill = skillLevel.Key;
                tempIndexOfSkill = GameInstance.PlayingCharacterEntity.IndexOfSkill(tempSkill.DataId);
                // Set character skill data
                tempCharacterSkill = CharacterSkill.Create(tempSkill, skillLevel.Value);
                if (uiCharacterHotkey.CanAssignCharacterSkill(tempCharacterSkill))
                {
                    tempUiCharacterSkill.Setup(new UICharacterSkillData(tempCharacterSkill), GameInstance.PlayingCharacterEntity, tempIndexOfSkill);
                    tempUiCharacterSkill.Show();
                    CacheSkillSelectionManager.Add(tempUiCharacterSkill);
                    ++countAssignable;
                }
                else
                {
                    tempUiCharacterSkill.Hide();
                }
            });

            // Setup item list
            UICharacterItem tempUiCharacterItem;
            CacheItemList.HideAll();
            Dictionary<int, UICharacterItem> foundUsableItemUIs = new Dictionary<int, UICharacterItem>();
            CacheItemList.Generate(GameInstance.PlayingCharacterEntity.NonEquipItems, (index, characterItem, ui) =>
            {
                if (!ui)
                    return;
                tempUiCharacterItem = ui.GetComponent<UICharacterItem>();
                if (uiCharacterHotkey.CanAssignCharacterItem(characterItem))
                {
                    IUsableItem usableItem = characterItem.GetUsableItem();
                    if (usableItem == null || (hideFoundUsableItems && foundUsableItemUIs.TryGetValue(usableItem.DataId, out UICharacterItem foundUI)))
                    {
                        tempUiCharacterItem.Hide();
                    }
                    else
                    {
                        tempUiCharacterItem.Setup(new UICharacterItemData(characterItem, InventoryType.NonEquipItems), GameInstance.PlayingCharacterEntity, index);
                        tempUiCharacterItem.Show();
                        CacheItemSelectionManager.Add(tempUiCharacterItem);
                        if (!foundUsableItemUIs.ContainsKey(usableItem.DataId))
                            foundUsableItemUIs.Add(usableItem.DataId, tempUiCharacterItem);
                        ++countAssignable;
                    }
                }
                else
                {
                    tempUiCharacterItem.Hide();
                }
            });

            // Setup guild skill list
            UIGuildSkill tempUiGuildSkill;
            GuildSkill tempGuildSkill;
            CacheGuildSkillList.HideAll();
            if (GameInstance.JoinedGuild != null)
            {
                CacheGuildSkillList.Generate(GameInstance.JoinedGuild.GetSkillLevels(), (index, guildSkillLevel, ui) =>
                {
                    if (!ui)
                        return;
                    tempUiGuildSkill = ui.GetComponent<UIGuildSkill>();
                    if (!GameInstance.GuildSkills.TryGetValue(guildSkillLevel.Key, out tempGuildSkill))
                    {
                        tempUiGuildSkill.Hide();
                        return;
                    }
                    if (uiCharacterHotkey.CanAssignGuildSkill(tempGuildSkill))
                    {
                        tempUiGuildSkill.Data = new UIGuildSkillData()
                        {
                            guildSkill = tempGuildSkill,
                            targetLevel = guildSkillLevel.Value,
                        };
                        tempUiGuildSkill.Show();
                        CacheGuildSkillSelectionManager.Add(tempUiGuildSkill);
                        ++countAssignable;
                    }
                    else
                    {
                        tempUiGuildSkill.Hide();
                    }
                });
            }

            if (autoHideIfNothingToAssign && countAssignable == 0)
                Hide();
        }

        public override void OnHide()
        {
            CacheSkillSelectionManager.DeselectSelectedUI();
            CacheItemSelectionManager.DeselectSelectedUI();
        }

        protected void OnSelectCharacterSkill(UICharacterSkill ui)
        {
            GameInstance.PlayingCharacterEntity.AssignSkillHotkey(uiCharacterHotkey.HotkeyId, ui.CharacterSkill);
            Hide();
        }

        protected void OnSelectCharacterItem(UICharacterItem ui)
        {
            GameInstance.PlayingCharacterEntity.AssignItemHotkey(uiCharacterHotkey.HotkeyId, ui.CharacterItem);
            Hide();
        }

        protected void OnSelectGuildSkill(UIGuildSkill ui)
        {
            GameInstance.PlayingCharacterEntity.AssignGuildSkillHotkey(uiCharacterHotkey.HotkeyId, ui.GuildSkill);
            Hide();
        }

        public void OnClickUnAssign()
        {
            GameInstance.PlayingCharacterEntity.UnAssignHotkey(uiCharacterHotkey.HotkeyId);
            Hide();
        }
    }
}
