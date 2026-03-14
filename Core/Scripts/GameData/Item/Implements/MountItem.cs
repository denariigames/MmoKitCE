using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.MOUNT_ITEM_FILE, menuName = GameDataMenuConsts.MOUNT_ITEM_MENU, order = GameDataMenuConsts.MOUNT_ITEM_ORDER)]
    public partial class MountItem : BaseItem, IMountItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_MOUNT.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Mount; }
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

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Category(3, "Mount Settings")]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableMountEntity))]
#endif
        private VehicleEntity mountEntity = null;
#endif
        public VehicleEntity VehicleEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return mountEntity;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceVehicleEntity addressableMountEntity = null;
        public AssetReferenceVehicleEntity AddressableVehicleEntity
        {
            get { return addressableMountEntity; }
        }
#endif

        [SerializeField]
        private IncrementalFloat mountDuration;
        public IncrementalFloat MountDuration { get { return mountDuration; } }

        [SerializeField]
        private bool noMountDuration;
        public bool NoMountDuration { get { return noMountDuration; } }

        [SerializeField]
        private float useItemCooldown = 0f;
        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }

        public bool UseItem(BaseCharacterEntity characterEntity, int itemIndex, CharacterItem characterItem)
        {
            if (!characterEntity.CanUseItem())
                return false;

            characterEntity.SpawnMount(MountType.MountItem, characterItem.id, MountDuration.GetAmount(characterItem.level));
            return true;
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

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddVehicleEntities(VehicleEntity);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceVehicleEntities(AddressableVehicleEntity);
#endif
        }
    }
}
