using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.WARP_TO_RESPAWN_POINT_ITEM_FILE, menuName = GameDataMenuConsts.WARP_TO_RESPAWN_POINT_ITEM_MENU, order = GameDataMenuConsts.WARP_TO_RESPAWN_POINT_ITEM_ORDER)]
    public class WarpToRespawnPointItem : BaseItem, IUsableItem
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
            WarpPortalType respawnPortalType = WarpPortalType.Default;
#if !DISABLE_DIFFER_MAP_RESPAWNING
            string respawnMapName = playerCharacterEntity.RespawnMapName;
            Vector3 respawnPosition = playerCharacterEntity.RespawnPosition;
#else
            string respawnMapName = playerCharacterEntity.CurrentMapName;
            Vector3 respawnPosition = playerCharacterEntity.CurrentPosition;
#endif
            bool respawnOverrideRotation = false;
            Vector3 respawnRotation = Vector3.zero;
            BaseMapInfo mapInfo = BaseGameNetworkManager.CurrentMapInfo;
            if (mapInfo != null)
                mapInfo.GetRespawnPoint(playerCharacterEntity, out respawnPortalType, out respawnMapName, out respawnPosition, out respawnOverrideRotation, out respawnRotation);
            switch (respawnPortalType)
            {
                case WarpPortalType.Default:
                    BaseGameNetworkManager.Singleton.WarpCharacter(playerCharacterEntity, respawnMapName, respawnPosition, respawnOverrideRotation, respawnRotation);
                    break;
                case WarpPortalType.EnterInstance:
                    BaseGameNetworkManager.Singleton.WarpCharacterToInstance(playerCharacterEntity, respawnMapName, respawnPosition, respawnOverrideRotation, respawnRotation);
                    break;
            }
            return true;
        }
    }
}
