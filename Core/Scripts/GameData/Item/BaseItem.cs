using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public abstract partial class BaseItem : BaseGameData, IItem
    {
        [Category("Item Settings")]
        [SerializeField]
        [Min(0)]
        protected int sellPrice = 0;
        [SerializeField]
        [Min(0f)]
        protected float weight = 0f;
        [SerializeField]
        protected bool noSlotUsage = false;
        [SerializeField]
        [Min(1)]
        protected int maxStack = 1;
        [SerializeField]
        [Tooltip("If this value > 0 it will override weapon's ammo capacity when reload (if the weapon use this as ammo)")]
        private int overrideAmmoCapacity = 0;
        [SerializeField]
        protected ItemRefine itemRefine = null;
        [SerializeField]
        [Tooltip("This is duration to lock item at first time when pick up dropped item or bought it from NPC or IAP system")]
        protected float lockDuration = 0;
        [SerializeField]
        [Tooltip("Time unit for `ExpireDuration`")]
        protected ETimeUnits expireDurationUnit = ETimeUnits.Hours;
        [SerializeField]
        [Tooltip("This is duration to make item to be expired and destroyed from inventory, set it to 0 to not apply expiring duration, set it to 7 to make it expire in next 7 hours (if `ExpireDurationUnit` is `Hours)")]
        protected int expireDuration = 0;

#if UNITY_EDITOR || !UNITY_SERVER
        [Category(10, "In-Scene Objects/Appearance")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableDropModel))]
#endif
        protected GameObject dropModel = null;
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceGameObject addressableDropModel;
#endif
#endif

        [Category(50, "Dismantle Settings")]
        [SerializeField]
        protected int dismantleReturnGold = 0;
        [SerializeField]
        protected ItemAmount[] dismantleReturnItems = new ItemAmount[0];
        [SerializeField]
        protected CurrencyAmount[] dismantleReturnCurrencies = new CurrencyAmount[0];

        [Category(51, "Restriction Settings")]
        [SerializeField]
        protected bool restrictDealing;
        [SerializeField]
        [FormerlySerializedAs("restractDropping")]
        protected bool restrictDropping;
        [SerializeField]
        protected bool restrictSelling;
        [SerializeField]
        protected bool deadDropProtected;


        [Category(100, "Cash Shop Generating Settings")]
        [SerializeField]
        protected CashShopItemGeneratingData[] cashShopItemGeneratingList = new CashShopItemGeneratingData[0];

        public override string Title
        {
            get
            {
                if (itemRefine == null || itemRefine.TitleColor.a == 0)
                    return base.Title;
                return "<color=#" + ColorUtility.ToHtmlStringRGB(itemRefine.TitleColor) + ">" + base.Title + "</color>";
            }
        }

        public virtual string RarityTitle
        {
            get
            {
                if (itemRefine == null)
                    return "Normal";
                return "<color=#" + ColorUtility.ToHtmlStringRGB(itemRefine.TitleColor) + ">" + itemRefine.Title + "</color>";
            }
        }

        public string GetTitle(int level)
        {
            if (itemRefine == null)
                return base.Title;
            Color titleColor = itemRefine.GetTitleColor(level);
            if (titleColor.a == 0)
                return base.Title;
            return "<color=#" + ColorUtility.ToHtmlStringRGB(titleColor) + ">" + base.Title + "</color>";
        }

        public abstract string TypeTitle { get; }

        public abstract ItemType ItemType { get; }

        public int SellPrice { get => sellPrice; set => sellPrice = value; }

        public float Weight { get => weight; set => weight = value; }

        public bool NoSlotUsage { get => noSlotUsage; set => noSlotUsage = value; }

        public int MaxStack { get => maxStack; set => maxStack = value; }

        public int OverrideAmmoCapacity { get => overrideAmmoCapacity; set => overrideAmmoCapacity = value; }

        public ItemRefine ItemRefine { get => itemRefine; set => itemRefine = value; }

        public float LockDuration { get => lockDuration; set => lockDuration = value; }

        public ETimeUnits ExpireDurationUnit { get => expireDurationUnit; set => expireDurationUnit = value; }

        public int ExpireDuration { get => expireDuration; set => expireDuration = value; }

#if UNITY_EDITOR || !UNITY_SERVER
        public UniTask<GameObject> GetDropModel()
        {
            GameObject dropModel = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            dropModel = this.dropModel;
#endif
#if !DISABLE_ADDRESSABLES
            return addressableDropModel.GetOrLoadAssetAsyncOrUsePrefab(dropModel);
#else
            return UniTask.FromResult(dropModel);
#endif
        }

        public IItem SetDropModel(GameObject dropModel)
        {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            this.dropModel = dropModel;
#endif
            return this;
        }
#endif

        public int DismantleReturnGold { get => dismantleReturnGold; set => dismantleReturnGold = value; }

        public ItemAmount[] DismantleReturnItems { get => dismantleReturnItems; set => dismantleReturnItems = value; }

        public CurrencyAmount[] DismantleReturnCurrencies { get => dismantleReturnCurrencies; set => dismantleReturnCurrencies = value; }

        public virtual bool RestrictDealing { get { return restrictDealing; } }

        public virtual bool RestrictDropping { get { return restrictDropping; } }

        public virtual bool RestrictSelling { get { return restrictSelling; } }

        public virtual bool DeadDropProtected { get { return deadDropProtected; } }

        public int MaxLevel
        {
            get
            {
                if (!ItemRefine || ItemRefine.Levels == null || ItemRefine.Levels.Length == 0)
                    return 1;
                return ItemRefine.Levels.Length;
            }
        }

        public override bool Validate()
        {
            bool hasChanges = false;
            // Equipment / Pet max stack always equals to 1
            switch (ItemType)
            {
                case ItemType.Armor:
                case ItemType.Weapon:
                case ItemType.Shield:
                case ItemType.Pet:
                case ItemType.Mount:
                    if (maxStack != 1)
                    {
                        maxStack = 1;
                        hasChanges = true;
                    }
                    break;
            }
            return hasChanges;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (ItemRefine != null)
                ItemRefine.PrepareRelatesData();
            GameInstance.AddItems(DismantleReturnItems);
            GameInstance.AddCurrencies(DismantleReturnCurrencies);
        }

        public void GenerateCashShopItems()
        {
            if (cashShopItemGeneratingList == null || cashShopItemGeneratingList.Length == 0)
                return;

            CashShopItemGeneratingData generatingData;
            CashShopItem cashShopItem;
            for (int i = 0; i < cashShopItemGeneratingList.Length; ++i)
            {
                generatingData = cashShopItemGeneratingList[i];
                cashShopItem = CreateInstance<CashShopItem>();
                cashShopItem.name = $"<CASHSHOPITEM_{name}_{i}>";
                cashShopItem.GenerateByItem(this, generatingData);
                GameInstance.CashShopItems[cashShopItem.DataId] = cashShopItem;
            }
        }
    }
}
