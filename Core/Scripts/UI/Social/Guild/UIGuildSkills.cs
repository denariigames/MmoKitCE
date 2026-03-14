using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGuildSkills : UIBase
    {
        [Header("UI Elements")]
        public GameObject listEmptyObject;
        public UIGuildSkill uiDialog;
        public UIGuildSkill uiPrefab;
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

        private UIGuildSkillSelectionManager _cacheSelectionManager;
        public UIGuildSkillSelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.GetOrAddComponent<UIGuildSkillSelectionManager>();
                return _cacheSelectionManager;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            listEmptyObject = null;
            uiDialog = null;
            uiPrefab = null;
            uiContainer = null;
            _cacheList = null;
            _cacheSelectionManager = null;
        }

        protected virtual void OnEnable()
        {
            CacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
            CacheSelectionManager.eventOnSelected.RemoveListener(OnSelect);
            CacheSelectionManager.eventOnSelected.AddListener(OnSelect);
            CacheSelectionManager.eventOnDeselected.RemoveListener(OnDeselect);
            CacheSelectionManager.eventOnDeselected.AddListener(OnDeselect);
            if (uiDialog != null)
                uiDialog.onHide.AddListener(OnDialogHide);
        }

        protected virtual void OnDisable()
        {
            if (uiDialog != null)
                uiDialog.onHide.RemoveListener(OnDialogHide);
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnDialogHide()
        {
            CacheSelectionManager.DeselectSelectedUI();
        }

        protected virtual void OnSelect(UIGuildSkill ui)
        {
            if (uiDialog != null)
            {
                uiDialog.selectionManager = CacheSelectionManager;
                ui.CloneTo(uiDialog);
                uiDialog.Show();
            }
        }

        protected virtual void OnDeselect(UIGuildSkill ui)
        {
            if (uiDialog != null)
                uiDialog.Hide();
        }

        public void UpdateData()
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
            if (GameInstance.JoinedGuild != null)
                list.AddRange(GameInstance.JoinedGuild.skillLevels);
            UpdateData(list);
        }

        public void UpdateData(List<KeyValuePair<int, int>> list)
        {
            CacheSelectionManager.DeselectSelectedUI();
            CacheSelectionManager.Clear();

            if (list.Count == 0)
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

            UIGuildSkill tempUI;
            CacheList.Generate(list, (index, data, ui) =>
            {
                if (!GameInstance.GuildSkills.TryGetValue(data.Key, out GuildSkill guildSkill))
                    guildSkill = null;
                tempUI = ui.GetComponent<UIGuildSkill>();
                tempUI.Data = new UIGuildSkillData(guildSkill, data.Value);
                tempUI.Show();
                CacheSelectionManager.Add(tempUI);
            });
        }
    }
}
