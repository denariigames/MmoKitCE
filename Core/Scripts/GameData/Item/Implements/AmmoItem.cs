using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.AMMO_ITEM_FILE, menuName = GameDataMenuConsts.AMMO_ITEM_MENU, order = GameDataMenuConsts.AMMO_ITEM_ORDER)]
    public partial class AmmoItem : BaseItem, IAmmoItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_AMMO.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Ammo; }
        }

        [Category(2, "Ammo Settings")]
        [SerializeField]
        [Tooltip("Ammo type data")]
        private AmmoType ammoType = null;
        public AmmoType AmmoType
        {
            get { return ammoType; }
        }
        
#if UNITY_EDITOR || !UNITY_SERVER
        [Category(3, "In-Scene Objects/Appearance")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableEquipModel))]
#endif
        protected GameObject equipModel;
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceGameObject addressableEquipModel = null;
#endif
#endif

#if UNITY_EDITOR || !UNITY_SERVER
        public UniTask<GameObject> GetAmmoAttachModel()
        {
            GameObject equipModel = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            equipModel = this.equipModel;
#endif
#if !DISABLE_ADDRESSABLES
            return addressableEquipModel.GetOrLoadAssetAsyncOrUsePrefab(equipModel);
#else
            return UniTask.FromResult(equipModel);
#endif
        }

        public IItem SetAmmoAttachModel(GameObject equipModel)
        {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            this.equipModel = equipModel;
#endif
            return this;
        }
#endif
        
        [SerializeField]
        [Tooltip("Increasing damages stats while attacking by weapon which put this item")]
        private DamageIncremental[] increaseDamages = new DamageIncremental[0];
        public DamageIncremental[] IncreaseDamages
        {
            get { return increaseDamages; }
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddAmmoTypes(AmmoType);
        }
    }
}
