using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SKILL_ITEM_FILE, menuName = GameDataMenuConsts.SKILL_ITEM_MENU, order = GameDataMenuConsts.SKILL_ITEM_ORDER)]
    public partial class SkillItem : BaseItem, ISkillItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_SKILL.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Skill; }
        }

        [Category(2, "Requirements")]
        [SerializeField]
        private ItemRequirement requirement = new ItemRequirement();
        public ItemRequirement Requirement
        {
            get { return requirement; }
        }

        [System.NonSerialized]
        private Dictionary<Attribute, float> _cacheRequireAttributeAmounts = null;
        public Dictionary<Attribute, float> RequireAttributeAmounts
        {
            get
            {
                if (_cacheRequireAttributeAmounts == null)
                {
                    _cacheRequireAttributeAmounts = new Dictionary<Attribute, float>();
                    GameDataHelpers.CombineAttributes(requirement.attributeAmounts, _cacheRequireAttributeAmounts, 1f);
                }
                return _cacheRequireAttributeAmounts;
            }
        }

        [Category(3, "Skill Settings")]
        [SerializeField]
        private BaseSkill usingSkill = null;
        public BaseSkill SkillData
        {
            get { return usingSkill; }
        }

        [SerializeField]
        private int usingSkillLevel = 0;
        public int SkillLevel
        {
            get { return usingSkillLevel; }
        }

        [SerializeField]
        private float useItemCooldown = 0f;
        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }

        public bool UseItem(BaseCharacterEntity characterEntity, int itemIndex, CharacterItem characterItem)
        {
            // TODO: May changes this function later.
            return true;
        }

        public bool HasCustomAimControls()
        {
            return SkillData.HasCustomAimControls();
        }

        public AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return SkillData.UpdateAimControls(aimAxes, SkillLevel);
        }

        public void FinishAimControls(bool isCancel)
        {
            SkillData.FinishAimControls(isCancel);
        }

        public bool IsChanneledAbility()
        {
            return SkillData.IsChanneledAbility();
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddSkills(SkillData);
        }
    }
}
