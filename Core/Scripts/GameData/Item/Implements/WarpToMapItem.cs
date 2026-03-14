using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.WARP_TO_MAP_ITEM_FILE, menuName = GameDataMenuConsts.WARP_TO_MAP_ITEM_MENU, order = GameDataMenuConsts.WARP_TO_MAP_ITEM_ORDER)]
    public class WarpToMapItem : BaseItem, IUsableItem
    {
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

        [SerializeField]
        private WarpPortalType warpPortalType = WarpPortalType.Default;
        [Tooltip("Map which character will warp to when use the item, leave this empty to warp character to other position in the same map")]
        [SerializeField]
        private BaseMapInfo warpToMapInfo = null;
        [Tooltip("Position which character will warp to when use the item")]
        [SerializeField]
        private Vector3 warpToPosition = Vector3.zero;
        [Tooltip("If this is `TRUE` it will change character's rotation when warp")]
        [SerializeField]
        private bool warpOverrideRotation = false;
        [Tooltip("This will be used if `warpOverrideRotation` is `TRUE` to change character's rotation when warp")]
        [SerializeField]
        private Vector3 warpToRotation = Vector3.zero;

        [SerializeField]
        private float useItemCooldown = 0f;
        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }

        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_CONSUMABLE.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Potion; }
        }

        public bool HasCustomAimControls()
        {
            return false;
        }

        public AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return default;
        }

        public void FinishAimControls(bool isCancel)
        {

        }

        public bool IsChanneledAbility()
        {
            return false;
        }

        public bool UseItem(BaseCharacterEntity characterEntity, int itemIndex, CharacterItem characterItem)
        {
            BasePlayerCharacterEntity playerCharacterEntity = characterEntity as BasePlayerCharacterEntity;
            if (playerCharacterEntity == null)
                return false;
            if (!characterEntity.CanUseItem() || !characterEntity.DecreaseItemsByIndex(itemIndex, 1, false))
                return false;
            characterEntity.FillEmptySlots();
            if (warpToMapInfo == null)
                BaseGameNetworkManager.Singleton.WarpCharacter(warpPortalType, playerCharacterEntity, string.Empty, warpToPosition, warpOverrideRotation, warpToRotation);
            else
                BaseGameNetworkManager.Singleton.WarpCharacter(warpPortalType, playerCharacterEntity, warpToMapInfo.Id, warpToPosition, warpOverrideRotation, warpToRotation);
            return true;
        }
    }
}
