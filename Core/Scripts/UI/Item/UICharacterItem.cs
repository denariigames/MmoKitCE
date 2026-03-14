using Cysharp.Text;
using Insthync.AddressableAssetTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UICharacterItem : UIDataForCharacter<UICharacterItemData>
    {
        public CharacterItem CharacterItem { get { return Data.characterItem; } }
        public int Level { get { return Data.targetLevel; } }
        public InventoryType InventoryType { get { return Data.inventoryType; } }
        public StorageType StorageType { get; set; }
        public string StorageOwnerId { get; set; }
        public BaseItem Item { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() : null; } }
        public IUsableItem UsableItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetUsableItem() : null; } }
        public IPotionItem PotionItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetPotionItem() : null; } }
        public IBuildingItem BuildingItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetBuildingItem() : null; } }
        public IEquipmentItem EquipmentItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetEquipmentItem() : null; } }
        public IArmorItem ArmorItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetArmorItem() : null; } }
        public IShieldItem ShieldItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetShieldItem() : null; } }
        public IDefendEquipmentItem DefendItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetDefendItem() : null; } }
        public IWeaponItem WeaponItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetWeaponItem() : null; } }
        public IAmmoItem AmmoItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetAmmoItem() : null; } }
        public ISocketEnhancerItem SocketEnhancerItem { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetSocketEnhancerItem() : null; } }
        public IItemWithBuffData ItemWithBuffData { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() as IItemWithBuffData : null; } }
        public IItemWithBuildingEntity ItemWithBuildingEntity { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() as IItemWithBuildingEntity : null; } }
        public IItemWithMonsterCharacterEntity ItemWithMonsterEntity { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() as IItemWithMonsterCharacterEntity : null; } }
        public IItemWithVehicleEntity ItemWithVehicleEntity { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() as IItemWithVehicleEntity : null; } }
        public IItemWithSkillData ItemWithSkillData { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() as IItemWithSkillData : null; } }
        public IItemWithAttributeData ItemWithAttributeData { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() as IItemWithAttributeData : null; } }
        public IItemWithStatusEffectApplyings ItemWithStatusEffectApplyings { get { return !CharacterItem.IsEmptySlot() ? CharacterItem.GetItem() as IItemWithStatusEffectApplyings : null; } }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Rarity Title}")]
        public UILocaleKeySetting formatKeyRarityTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_RARITY);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);
        [Tooltip("Format => {0} = {Refine Level}")]
        public UILocaleKeySetting formatKeyRefineLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_REFINE_LEVEL);
        [Tooltip("Format => {0} = {Refine Level}")]
        public UILocaleKeySetting formatKeyTitleWithRefineLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_TITLE_WITH_REFINE_LEVEL);
        [Tooltip("Format => {0} = {Sell Price}")]
        public UILocaleKeySetting formatKeySellPrice = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_PRICE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Amount}, {1} = {Max Stack}")]
        public UILocaleKeySetting formatKeyStack = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_STACK);
        [Tooltip("Format => {0} = {Total item amount}")]
        public UILocaleKeySetting formatKeyTotalItemAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Durability}, {1} = {Max Durability}")]
        public UILocaleKeySetting formatKeyDurability = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_DURABILITY);
        [Tooltip("Format => {0} = {Weight}")]
        public UILocaleKeySetting formatKeyWeight = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_WEIGHT);
        [Tooltip("Format => {0} = {Current Exp}, {1} = {Max Exp}")]
        public UILocaleKeySetting formatKeyExp = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_EXP);
        [Tooltip("Format => {0} = {Lock Remains Duration}")]
        public UILocaleKeySetting formatKeyLockRemainsDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Building Title}")]
        public UILocaleKeySetting formatKeyBuilding = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_BUILDING);
        [Tooltip("Format => {0} = {Pet Title}")]
        public UILocaleKeySetting formatKeyPet = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_PET);
        [Tooltip("Format => {0} = {Mount Title}")]
        public UILocaleKeySetting formatKeyMount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_MOUNT);
        [Tooltip("Format => {0} = {Skill Title}")]
        public UILocaleKeySetting formatKeySkill = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_SKILL);
        [Tooltip("Format => {0} = {Attribute Title}")]
        public UILocaleKeySetting formatKeyAttribute = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_ATTRIBUTE);
        [Tooltip("Format => {0} = {Cooldown Duration}")]
        public UILocaleKeySetting formatKeyCoolDownDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SKILL_COOLDOWN_DURATION);
        [Tooltip("Format => {0} = {Cooldown Remains Duration}")]
        public UILocaleKeySetting formatKeyCoolDownRemainsDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Item Type Title}")]
        public UILocaleKeySetting formatKeyItemType = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_TYPE);
        [Tooltip("Format => {0} = {Duration to be expired}")]
        public UILocaleKeySetting formatKeyExpireDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_EXPIRE_DURATION);
        [Tooltip("Format => {0} = {When it will be expired}")]
        public UILocaleKeySetting formatKeyExpireTime = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_EXPIRE_TIME);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public TextWrapper uiTextRarity;
        public TextWrapper uiTextLevel;
        public Image imageIcon;
        public Image imageRarity;
        public TextWrapper uiTextItemType;
        public TextWrapper uiTextSellPrice;
        public TextWrapper uiTextStack;
        public TextWrapper uiTextTotalItemAmount;
        public TextWrapper uiTextDurability;
        public UIGageValue uiGageDurability;
        public TextWrapper uiTextWeight;
        public TextWrapper uiTextExp;
        public UIGageValue uiGageExp;

        [Header("Item Expiring")]
        public TextWrapper uiTextExpireDuration;
        public TextWrapper uiTextExpireTime;

        [Header("Item Locking")]
        public TextWrapper uiTextLockRemainsDuration;
        public GameObject[] lockObjects = new GameObject[0];
        [FormerlySerializedAs("noLockObjects")]
        public GameObject[] notLockObjects = new GameObject[0];

        [Header("Data Emptiness")]
        public GameObject[] emptyObjects = new GameObject[0];
        public GameObject[] notEmptyObjects = new GameObject[0];

        [Header("Equipment - UI Elements")]
        public UIItemRequirement uiRequirement;
        [FormerlySerializedAs("uiStats")]
        public UICharacterStats uiIncreaseStats;
        public UICharacterStats uiIncreaseStatsRate;
        public UIAttributeAmounts uiIncreaseAttributes;
        public UIAttributeAmounts uiIncreaseAttributesRate;
        public UIResistanceAmounts uiIncreaseResistances;
        public UIArmorAmounts uiIncreaseArmors;
        public UIArmorAmounts uiIncreaseArmorsRate;
        [FormerlySerializedAs("uiIncreaseDamageAmounts")]
        public UIDamageElementAmounts uiIncreaseDamages;
        public UIDamageElementAmounts uiIncreaseDamagesRate;
        public UIStatusEffectResistances uiStatusEffectResistances;
        public UISkillLevels uiIncreaseSkillLevels;
        public UIEquipmentSet uiEquipmentSet;
        public UIEquipmentSockets uiEquipmentSockets;
        public UIItemRandomBonus uiItemRandomBonus;
        [Tooltip("Use this component to show refine info, if you are going to refine when click button, use `OnClickRefineItem` function")]
        public UIRefineItem uiRefineItem;
        [Tooltip("Use this component to show dismantle info, if you are going to dismantle when click button, use `OnClickDismantleItem` function")]
        public UIDismantleItem uiDismantleItem;
        [Tooltip("Use this component to show repair info, if you are going to repair when click button, use `OnClickRepairItem` function")]
        public UIRepairItem uiRepairItem;

        [Header("Armor/Shield - UI Elements")]
        public UIArmorAmount uiArmorAmount;

        [Header("Weapon - UI Elements")]
        [FormerlySerializedAs("uiDamageAmounts")]
        public UIDamageElementAmount uiDamageAmount;

        [Header("Weapon Ammo - UI Elements")]
        public TextWrapper uiTextCurrentAmmo;
        public TextWrapper uiTextReserveAmmo;
        public TextWrapper uiTextSumAmmo;
        public GameObject[] requireAmmoSymbols;
        public GameObject[] noRequireAmmoSymbols;
        public UIGageValue gageAmmo;

        [Header("Building - UI Elements")]
        public TextWrapper uiTextBuilding;

        [Header("Item with Monster/Pet - UI Elements")]
        [FormerlySerializedAs("uiTextPet")]
        public TextWrapper uiTextMonster;

        [Header("Item with Vehicle/Mount - UI Elements")]
        [FormerlySerializedAs("uiTextMount")]
        public TextWrapper uiTextVehicle;

        [Header("Item with Buff/Potion Buff - UI Elements")]
        [FormerlySerializedAs("uiPotionBuff")]
        public UIBuff uiBuff;

        [Header("Item with Skill - UI Elements")]
        public TextWrapper uiTextSkill;
        public UICharacterSkill uiSkill;

        [Header("Item with Attribute - UI Elements")]
        public TextWrapper uiTextAttribute;
        public UICharacterAttribute uiAttribute;

        [Header("Item with Status Effect Applyings - UI Elements")]
        public UIStatusEffectApplyings uiStatusEffectApplyingsSelfWhenAttacking;
        public UIStatusEffectApplyings uiStatusEffectApplyingsEnemyWhenAttacking;
        public UIStatusEffectApplyings uiStatusEffectApplyingsSelfWhenAttacked;
        public UIStatusEffectApplyings uiStatusEffectApplyingsEnemyWhenAttacked;

        [Header("Cooldown")]
        public TextWrapper uiTextCoolDownDuration;
        public TextWrapper uiTextCoolDownRemainsDuration;
        public Image imageCoolDownGage;
        public GameObject[] countDownObjects = new GameObject[0];
        public GameObject[] noCountDownObjects = new GameObject[0];

        [Header("Events")]
        public UnityEvent onSetLevelZeroData = new UnityEvent();
        public UnityEvent onSetNonLevelZeroData = new UnityEvent();
        public UnityEvent onSetEquippedData = new UnityEvent();
        public UnityEvent onSetUnEquippedData = new UnityEvent();
        public UnityEvent onSetUnEquippableData = new UnityEvent();
        public UnityEvent onSetUsableData = new UnityEvent();
        public UnityEvent onSetStorageItemData = new UnityEvent();
        public UnityEvent onSetItemsContainerItemData = new UnityEvent();
        public UnityEvent onSetUnknowSourceData = new UnityEvent();
        public UnityEvent onNpcSellItemDialogAppear = new UnityEvent();
        public UnityEvent onNpcSellItemDialogDisappear = new UnityEvent();
        public UnityEvent onRefineItemDialogAppear = new UnityEvent();
        public UnityEvent onRefineItemDialogDisappear = new UnityEvent();
        public UnityEvent onDismantleItemDialogAppear = new UnityEvent();
        public UnityEvent onDismantleItemDialogDisappear = new UnityEvent();
        public UnityEvent onRepairItemDialogAppear = new UnityEvent();
        public UnityEvent onRepairItemDialogDisappear = new UnityEvent();
        public UnityEvent onEnhanceSocketItemDialogAppear = new UnityEvent();
        public UnityEvent onEnhanceSocketItemDialogDisappear = new UnityEvent();
        public UnityEvent onStorageDialogAppear = new UnityEvent();
        public UnityEvent onStorageDialogDisappear = new UnityEvent();
        public UnityEvent onEnterDealingState = new UnityEvent();
        public UnityEvent onExitDealingState = new UnityEvent();
        public UnityEvent onStartVendingDialogAppear = new UnityEvent();
        public UnityEvent onStartVendingDialogDisappear = new UnityEvent();
        public UnityEvent onSetEquipmentWithAmmo = new UnityEvent();
        public UnityEvent onSetEquipmentWithoutAmmo = new UnityEvent();
        public UnityEvent onClickAimToUseItem = new UnityEvent();
        public UnityEvent onClickUseItem = new UnityEvent();

        [Header("Options")]
        public UICharacterItemDragHandler uiDragging;
        [Tooltip("UI which will be shown if this item level not reached max level")]
        public UICharacterItem uiNextLevelItem;
        [Tooltip("UIs in this list will be shown when set this item is equipment item and inventory type is `NonEquipItems` or `EquipItems`")]
        public UICharacterItem[] uiComparingEquipments;
        public bool showAmountWhenMaxIsOne;
        public bool showLevelAsDefault;
        public bool dontAppendRefineLevelToTitle;
        public bool dontShowComparingEquipments;
        public bool dontCalculateRandomBonus;
        public bool changeObjectNameByData = true;

        public bool IsSetupAsEquipSlot { get; private set; }
        public string EquipPosition { get; private set; }
        public byte EquipSlotIndex { get; private set; }

        protected bool _isSellItemDialogAppeared;
        protected bool _isRefineItemDialogAppeared;
        protected bool _isDismantleItemDialogAppeared;
        protected bool _isRepairItemDialogAppeared;
        protected bool _isEnhanceSocketItemDialogAppeared;
        protected bool _isStorageDialogAppeared;
        protected bool _isDealingStateEntered;
        protected bool _isStartVendingDialogAppeared;
        protected float _lockRemainsDuration;
        protected bool _dirtyIsLock;
        protected float _coolDownRemainsDuration;
        protected bool _dirtyIsCountDown;
        protected bool _forceUpdateUi = true;
        protected CalculatedItemRandomBonus _randomBonus = null;
        protected Dictionary<Attribute, float> _tempAttributes = new Dictionary<Attribute, float>();
        protected Dictionary<Attribute, float> _tempAttributesRate = new Dictionary<Attribute, float>();
        protected Dictionary<DamageElement, float> _tempResistances = new Dictionary<DamageElement, float>();
        protected Dictionary<DamageElement, float> _tempArmors = new Dictionary<DamageElement, float>();
        protected Dictionary<DamageElement, float> _tempArmorsRate = new Dictionary<DamageElement, float>();
        protected Dictionary<DamageElement, MinMaxFloat> _tempDamageAmounts = new Dictionary<DamageElement, MinMaxFloat>();
        protected Dictionary<DamageElement, MinMaxFloat> _tempDamageAmountsRate = new Dictionary<DamageElement, MinMaxFloat>();
        protected Dictionary<StatusEffect, float> _tempStatusEffectResistances = new Dictionary<StatusEffect, float>();
        protected Dictionary<BaseSkill, int> _tempSkillLevels = new Dictionary<BaseSkill, int>();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            uiTextDescription = null;
            uiTextRarity = null;
            uiTextLevel = null;
            imageIcon = null;
            imageRarity = null;
            uiTextItemType = null;
            uiTextSellPrice = null;
            uiTextStack = null;
            uiTextTotalItemAmount = null;
            uiTextDurability = null;
            uiGageDurability = null;
            uiTextWeight = null;
            uiTextExp = null;
            uiGageExp = null;
            uiTextExpireDuration = null;
            uiTextExpireTime = null;
            uiTextLockRemainsDuration = null;
            lockObjects.Nullify();
            notLockObjects.Nullify();
            emptyObjects.Nullify();
            notEmptyObjects.Nullify();
            uiRequirement = null;
            uiIncreaseStats = null;
            uiIncreaseStatsRate = null;
            uiIncreaseAttributes = null;
            uiIncreaseAttributesRate = null;
            uiIncreaseResistances = null;
            uiIncreaseArmors = null;
            uiIncreaseArmorsRate = null;
            uiIncreaseDamages = null;
            uiIncreaseDamagesRate = null;
            uiStatusEffectResistances = null;
            uiIncreaseSkillLevels = null;
            uiEquipmentSet = null;
            uiEquipmentSockets = null;
            uiItemRandomBonus = null;
            uiRefineItem = null;
            uiDismantleItem = null;
            uiRepairItem = null;
            uiArmorAmount = null;
            uiDamageAmount = null;
            uiTextCurrentAmmo = null;
            uiTextReserveAmmo = null;
            uiTextSumAmmo = null;
            requireAmmoSymbols.Nullify();
            noRequireAmmoSymbols.Nullify();
            gageAmmo = null;
            uiTextBuilding = null;
            uiTextMonster = null;
            uiTextVehicle = null;
            uiBuff = null;
            uiTextSkill = null;
            uiSkill = null;
            uiTextAttribute = null;
            uiAttribute = null;
            uiStatusEffectApplyingsSelfWhenAttacking = null;
            uiStatusEffectApplyingsEnemyWhenAttacking = null;
            uiStatusEffectApplyingsSelfWhenAttacked = null;
            uiStatusEffectApplyingsEnemyWhenAttacked = null;
            uiTextCoolDownDuration = null;
            uiTextCoolDownRemainsDuration = null;
            imageCoolDownGage = null;
            countDownObjects.Nullify();
            noCountDownObjects.Nullify();
            onSetLevelZeroData?.RemoveAllListeners();
            onSetLevelZeroData = null;
            onSetNonLevelZeroData?.RemoveAllListeners();
            onSetNonLevelZeroData = null;
            onSetEquippedData?.RemoveAllListeners();
            onSetEquippedData = null;
            onSetUnEquippedData?.RemoveAllListeners();
            onSetUnEquippedData = null;
            onSetUnEquippableData?.RemoveAllListeners();
            onSetUnEquippableData = null;
            onSetUsableData?.RemoveAllListeners();
            onSetUsableData = null;
            onSetStorageItemData?.RemoveAllListeners();
            onSetStorageItemData = null;
            onSetItemsContainerItemData?.RemoveAllListeners();
            onSetItemsContainerItemData = null;
            onSetUnknowSourceData?.RemoveAllListeners();
            onSetUnknowSourceData = null;
            onNpcSellItemDialogAppear?.RemoveAllListeners();
            onNpcSellItemDialogAppear = null;
            onNpcSellItemDialogDisappear?.RemoveAllListeners();
            onNpcSellItemDialogDisappear = null;
            onRefineItemDialogAppear?.RemoveAllListeners();
            onRefineItemDialogAppear = null;
            onRefineItemDialogDisappear?.RemoveAllListeners();
            onRefineItemDialogDisappear = null;
            onDismantleItemDialogAppear?.RemoveAllListeners();
            onDismantleItemDialogAppear = null;
            onDismantleItemDialogDisappear?.RemoveAllListeners();
            onDismantleItemDialogDisappear = null;
            onRepairItemDialogAppear?.RemoveAllListeners();
            onRepairItemDialogAppear = null;
            onRepairItemDialogDisappear?.RemoveAllListeners();
            onRepairItemDialogDisappear = null;
            onEnhanceSocketItemDialogAppear?.RemoveAllListeners();
            onEnhanceSocketItemDialogAppear = null;
            onEnhanceSocketItemDialogDisappear?.RemoveAllListeners();
            onEnhanceSocketItemDialogDisappear = null;
            onStorageDialogAppear?.RemoveAllListeners();
            onStorageDialogAppear = null;
            onStorageDialogDisappear?.RemoveAllListeners();
            onStorageDialogDisappear = null;
            onEnterDealingState?.RemoveAllListeners();
            onEnterDealingState = null;
            onExitDealingState?.RemoveAllListeners();
            onExitDealingState = null;
            onStartVendingDialogAppear?.RemoveAllListeners();
            onStartVendingDialogAppear = null;
            onStartVendingDialogDisappear?.RemoveAllListeners();
            onStartVendingDialogDisappear = null;
            onSetEquipmentWithAmmo?.RemoveAllListeners();
            onSetEquipmentWithAmmo = null;
            onSetEquipmentWithoutAmmo?.RemoveAllListeners();
            onSetEquipmentWithoutAmmo = null;
            uiDragging = null;
            uiNextLevelItem = null;
            uiComparingEquipments.Nullify();
            _randomBonus = null;
            _tempAttributes.Clear();
            _tempAttributes = null;
            _tempAttributesRate.Clear();
            _tempAttributesRate = null;
            _tempResistances.Clear();
            _tempResistances = null;
            _tempArmors.Clear();
            _tempArmors = null;
            _tempArmorsRate.Clear();
            _tempArmorsRate = null;
            _tempDamageAmounts.Clear();
            _tempDamageAmounts = null;
            _tempDamageAmountsRate.Clear();
            _tempDamageAmountsRate = null;
            _tempStatusEffectResistances.Clear();
            _tempStatusEffectResistances = null;
            _tempSkillLevels.Clear();
            _tempSkillLevels = null;
        }

        public void SetupAsEquipSlot(string equipPosition, byte equipSlotIndex)
        {
            IsSetupAsEquipSlot = true;
            EquipPosition = equipPosition;
            EquipSlotIndex = equipSlotIndex;
        }

        public override void CloneTo(UISelectionEntry<UICharacterItemData> target)
        {
            base.CloneTo(target);
            if (target != null && target is UICharacterItem castedTarget)
            {
                castedTarget.IsSetupAsEquipSlot = IsSetupAsEquipSlot;
                castedTarget.EquipPosition = EquipPosition;
                castedTarget.EquipSlotIndex = EquipSlotIndex;
                castedTarget.StorageType = StorageType;
                castedTarget.StorageOwnerId = StorageOwnerId;
            }
        }

        protected CalculatedItemRandomBonus GetRandomBonus()
        {
            if (EquipmentItem != null && !dontCalculateRandomBonus && _randomBonus == null)
                _randomBonus = new CalculatedItemRandomBonus(EquipmentItem, CharacterItem.level, CharacterItem.randomSeed, CharacterItem.version);
            return _randomBonus;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onUpdateRightWeaponAmmoSim += OnUpdateRightWeaponAmmoSim;
            GameInstance.PlayingCharacterEntity.onUpdateLeftWeaponAmmoSim += OnUpdateLeftWeaponAmmoSim;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _lockRemainsDuration = 0f;
            if (uiComparingEquipments != null)
            {
                foreach (UICharacterItem uiComparingEquipment in uiComparingEquipments)
                {
                    uiComparingEquipment.Hide();
                }
            }
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onUpdateRightWeaponAmmoSim -= OnUpdateRightWeaponAmmoSim;
            GameInstance.PlayingCharacterEntity.onUpdateLeftWeaponAmmoSim -= OnUpdateLeftWeaponAmmoSim;
        }


        public override void ManagedUpdate()
        {
            if (_forceUpdateUi)
                _updateCountDown = 0f;

            base.ManagedUpdate();

            float deltaTime = Time.deltaTime;
            UpdateLockRemainsDuration(deltaTime);

            IUsableItem usableItem = UsableItem;
            if (usableItem != null)
            {
                UpdateUIUsableItemCoolDownRemainsDuration(usableItem, deltaTime);
            }
            else
            {
                if (uiTextCoolDownDuration != null)
                    uiTextCoolDownDuration.SetGameObjectActive(false);
                if (uiTextCoolDownRemainsDuration != null)
                    uiTextCoolDownRemainsDuration.SetGameObjectActive(false);
                if (imageCoolDownGage != null)
                    imageCoolDownGage.gameObject.SetActive(false);
            }

            _forceUpdateUi = false;
        }

        private void UpdateExpireTime()
        {

            if (uiTextExpireTime != null)
            {
                if (!CharacterItem.IsEmptySlot() && CharacterItem.expireTime > 0)
                {
                    System.DateTime dateTime = GenericUtils.GetDateTimeBySeconds(CharacterItem.expireTime).ToLocalTime();
                    uiTextExpireTime.SetGameObjectActive(true);
                    uiTextExpireTime.text = ZString.Format(
                        LanguageManager.GetText(formatKeyExpireTime),
                        (new System.DateTime(dateTime.Ticks) - System.DateTime.Now).GetPrettyDate(true));
                }
                else
                {
                    uiTextExpireTime.SetGameObjectActive(false);
                }
            }
        }

        private void UpdateLockRemainsDuration(float deltaTime)
        {
            _lockRemainsDuration = CharacterItem.lockRemainsDuration;

            if (_lockRemainsDuration > 0f)
            {
                _lockRemainsDuration -= deltaTime;
                if (_lockRemainsDuration <= 0f)
                    _lockRemainsDuration = 0f;
            }
            else
            {
                _lockRemainsDuration = 0f;
            }

            if (uiTextLockRemainsDuration != null)
            {
                uiTextLockRemainsDuration.SetGameObjectActive(_lockRemainsDuration > 0);
                uiTextLockRemainsDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeyLockRemainsDuration),
                    _lockRemainsDuration.ToString("N0"));
            }

            bool isLock = _lockRemainsDuration > 0f;
            if (_forceUpdateUi || _dirtyIsLock != isLock)
            {
                _dirtyIsLock = isLock;
                foreach (GameObject obj in lockObjects)
                {
                    obj.SetActive(isLock);
                }
                foreach (GameObject obj in notLockObjects)
                {
                    obj.SetActive(!isLock);
                }
            }
        }

        private void UpdateUIUsableItemCoolDownRemainsDuration(IUsableItem item, float deltaTime)
        {
            float coolDownDuration = item != null ? item.UseItemCooldown : 0f;
            UpdateUICoolDownRemainsDuration(coolDownDuration, deltaTime);
        }

        private void UpdateUICoolDownRemainsDuration(float coolDownDuration, float deltaTime)
        {
            if (_coolDownRemainsDuration > 0f)
            {
                _coolDownRemainsDuration -= deltaTime;
                if (_coolDownRemainsDuration <= 0f)
                    _coolDownRemainsDuration = 0f;
            }
            else
            {
                _coolDownRemainsDuration = 0f;
            }

            if (uiTextCoolDownDuration != null)
            {
                uiTextCoolDownDuration.SetGameObjectActive(coolDownDuration > 0f);
                uiTextCoolDownDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCoolDownDuration),
                    coolDownDuration.ToString("N0"));
            }

            if (uiTextCoolDownRemainsDuration != null)
            {
                uiTextCoolDownRemainsDuration.SetGameObjectActive(_coolDownRemainsDuration > 0);
                uiTextCoolDownRemainsDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCoolDownRemainsDuration),
                    _coolDownRemainsDuration.ToString("N0"));
            }

            if (imageCoolDownGage != null)
            {
                imageCoolDownGage.fillAmount = coolDownDuration <= 0 ? 0 : _coolDownRemainsDuration / coolDownDuration;
                imageCoolDownGage.gameObject.SetActive(imageCoolDownGage.fillAmount > 0f);
            }

            bool isCountDown = _coolDownRemainsDuration > 0f;
            if (_forceUpdateUi || _dirtyIsCountDown != isCountDown)
            {
                _dirtyIsCountDown = isCountDown;
                foreach (GameObject obj in countDownObjects)
                {
                    obj.SetActive(isCountDown);
                }
                foreach (GameObject obj in noCountDownObjects)
                {
                    obj.SetActive(!isCountDown);
                }
            }
        }

        protected void UpdateCoolDownRemainsDuration(float diffToChangeRemainsDuration = 0f)
        {
            if (_coolDownRemainsDuration <= 0f && Character != null && UsableItem != null)
            {
                int indexOfSkillUsage = Character.IndexOfSkillUsage(SkillUsageType.UsableItem, UsableItem.DataId);
                if (indexOfSkillUsage >= 0 && Mathf.Abs(Character.SkillUsages[indexOfSkillUsage].coolDownRemainsDuration - _coolDownRemainsDuration) > diffToChangeRemainsDuration)
                    _coolDownRemainsDuration = Character.SkillUsages[indexOfSkillUsage].coolDownRemainsDuration;
                else
                    _coolDownRemainsDuration = 0f;
            }
        }

        protected override void UpdateUI()
        {
            UpdateCoolDownRemainsDuration();
            UpdateExpireTime();

            if (!IsOwningCharacter() || !IsVisible())
                return;

            UpdateShopUIVisibility(false);
            UpdateRefineItemUIVisibility(false);
            UpdateDismantleItemUIVisibility(false);
            UpdateRepairItemUIVisibility(false);
            UpdateEnhanceSocketUIVisibility(false);
            UpdateStorageUIVisibility(false);
            UpdateDealingState(false);
            UpdateStartVendingUIVisibility(false);
        }

        public void OnUpdateRightWeaponAmmoSim(int ammo)
        {
            UpdateAmmo();
        }

        public void OnUpdateLeftWeaponAmmoSim(int ammo)
        {
            UpdateAmmo();
        }

        protected void UpdateAmmo()
        {
            if (WeaponItem != null && (WeaponItem.WeaponType.AmmoType != null || WeaponItem.AmmoItemIds.Count > 0))
            {
                CharacterItem equippedRightHand = GameInstance.PlayingCharacter.EquipWeapons.rightHand;
                bool isEmptyRightHand = equippedRightHand.IsEmptySlot();
                bool isThisRightHand = string.Equals(equippedRightHand.id, CharacterItem.id);
                IWeaponItem rightHandWeapon = isEmptyRightHand ? null : equippedRightHand.GetWeaponItem();

                CharacterItem equippedLeftHand = GameInstance.PlayingCharacter.EquipWeapons.leftHand;
                bool isEmptyLeftHand = equippedLeftHand.IsEmptySlot();
                bool isThisLeftHand = string.Equals(equippedLeftHand.id, CharacterItem.id);
                IWeaponItem leftHandWeapon = isEmptyLeftHand ? null : equippedLeftHand.GetWeaponItem();

                int currentAmmo = CharacterItem.ammo;
                if (!isEmptyRightHand &&
                    isThisRightHand &&
                    rightHandWeapon != null)
                {
                    currentAmmo = GameInstance.PlayingCharacterEntity.RightWeaponAmmoSim;
                }
                if (!isEmptyLeftHand &&
                    isThisLeftHand &&
                    leftHandWeapon != null)
                {
                    currentAmmo = GameInstance.PlayingCharacterEntity.LeftWeaponAmmoSim;
                }
                int reserveAmmo = GameInstance.PlayingCharacter.CountAllAmmos(WeaponItem);

                if (uiTextCurrentAmmo != null)
                {
                    uiTextCurrentAmmo.SetGameObjectActive(WeaponItem.AmmoCapacity > 0);
                    uiTextCurrentAmmo.text = currentAmmo.ToString("N0");
                }

                if (uiTextReserveAmmo != null)
                {
                    uiTextReserveAmmo.SetGameObjectActive(true);
                    uiTextReserveAmmo.text = reserveAmmo.ToString("N0");
                }

                if (uiTextSumAmmo != null)
                {
                    uiTextSumAmmo.SetGameObjectActive(true);
                    uiTextSumAmmo.text = (currentAmmo + reserveAmmo).ToString("N0");
                }

                if (requireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in requireAmmoSymbols)
                    {
                        symbol.SetActive(true);
                    }
                }

                if (noRequireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in noRequireAmmoSymbols)
                    {
                        symbol.SetActive(false);
                    }
                }

                if (gageAmmo != null)
                {
                    gageAmmo.SetVisible(WeaponItem.AmmoCapacity > 0);
                    gageAmmo.Update(currentAmmo, WeaponItem.AmmoCapacity);
                }
            }
            else
            {
                if (uiTextCurrentAmmo != null)
                    uiTextCurrentAmmo.SetGameObjectActive(false);

                if (uiTextReserveAmmo != null)
                    uiTextReserveAmmo.SetGameObjectActive(false);

                if (uiTextSumAmmo != null)
                    uiTextSumAmmo.SetGameObjectActive(false);

                if (requireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in requireAmmoSymbols)
                    {
                        symbol.SetActive(false);
                    }
                }

                if (noRequireAmmoSymbols != null)
                {
                    foreach (GameObject symbol in noRequireAmmoSymbols)
                    {
                        symbol.SetActive(true);
                    }
                }

                if (gageAmmo != null)
                    gageAmmo.SetVisible(false);
            }
        }

        protected override void UpdateData()
        {
            if (changeObjectNameByData)
                name = $"(UICharacterItem){(Item == null ? string.Empty : Item.Id)}";

            bool isEmpty = Item == null;

            CharacterItem equippedRightHand = GameInstance.PlayingCharacter.EquipWeapons.rightHand;
            bool isEmptyRightHand = equippedRightHand.IsEmptySlot();
            bool isThisRightHand = string.Equals(equippedRightHand.id, CharacterItem.id);
            IWeaponItem rightHandWeapon = isEmptyRightHand ? null : equippedRightHand.GetWeaponItem();
            IShieldItem rightHandShield = isEmptyRightHand ? null : equippedRightHand.GetShieldItem();

            CharacterItem equippedLeftHand = GameInstance.PlayingCharacter.EquipWeapons.leftHand;
            bool isEmptyLeftHand = equippedLeftHand.IsEmptySlot();
            bool isThisLeftHand = string.Equals(equippedLeftHand.id, CharacterItem.id);
            IWeaponItem leftHandWeapon = isEmptyLeftHand ? null : equippedLeftHand.GetWeaponItem();
            IShieldItem leftHandShield = isEmptyLeftHand ? null : equippedLeftHand.GetShieldItem();

            foreach (GameObject obj in emptyObjects)
            {
                obj.SetActive(isEmpty);
            }
            foreach (GameObject obj in notEmptyObjects)
            {
                obj.SetActive(!isEmpty);
            }

            _randomBonus = null;
            UpdateCoolDownRemainsDuration(1f);

            if (Level <= 0)
            {
                onSetLevelZeroData.Invoke();
            }
            else
            {
                onSetNonLevelZeroData.Invoke();
            }

            switch (InventoryType)
            {
                case InventoryType.Unknow:
                    onSetUnknowSourceData.Invoke();
                    break;
                case InventoryType.StorageItems:
                    onSetStorageItemData.Invoke();
                    break;
                case InventoryType.ItemsContainer:
                    onSetItemsContainerItemData.Invoke();
                    break;
                case InventoryType.NonEquipItems:
                case InventoryType.EquipItems:
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipWeaponLeft:
                    if (EquipmentItem != null)
                    {
                        if (InventoryType == InventoryType.NonEquipItems)
                            onSetUnEquippedData.Invoke();
                        else
                            onSetEquippedData.Invoke();
                        if (CharacterItem.ammo > 0)
                            onSetEquipmentWithAmmo.Invoke();
                        else
                            onSetEquipmentWithoutAmmo.Invoke();
                    }
                    else
                    {
                        onSetUnEquippableData.Invoke();
                        if (UsableItem != null)
                            onSetUsableData.Invoke();
                    }
                    break;
            }

            if (uiTextTitle != null)
            {
                string str = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    isEmpty ? LanguageManager.GetUnknowTitle() : Item.GetTitle(Level));
                if (!dontAppendRefineLevelToTitle && EquipmentItem != null && Level > 1)
                {
                    str = ZString.Format(
                        LanguageManager.GetText(formatKeyTitleWithRefineLevel),
                        isEmpty ? LanguageManager.GetUnknowTitle() : Item.GetTitle(Level),
                        (Level - 1).ToString("N0"));
                }
                uiTextTitle.text = str;
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    isEmpty ? LanguageManager.GetUnknowDescription() : Item.Description);
            }

            if (uiTextRarity != null)
            {
                uiTextRarity.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRarityTitle),
                    isEmpty ? LanguageManager.GetUnknowTitle() : Item.RarityTitle);
            }

            if (uiTextLevel != null)
            {
                uiTextLevel.SetGameObjectActive(EquipmentItem != null || ItemWithMonsterEntity != null);
                if (EquipmentItem != null)
                {
                    if (showLevelAsDefault)
                    {
                        uiTextLevel.text = ZString.Format(
                            LanguageManager.GetText(formatKeyLevel),
                            Level.ToString("N0"));
                    }
                    else
                    {
                        uiTextLevel.text = ZString.Format(
                            LanguageManager.GetText(formatKeyRefineLevel),
                            (Level - 1).ToString("N0"));
                    }
                }
                else if (ItemWithMonsterEntity != null)
                {
                    uiTextLevel.text = ZString.Format(
                        LanguageManager.GetText(formatKeyLevel),
                        Level.ToString("N0"));
                }
            }

            imageIcon.SetImageGameDataIcon(Item);
            imageRarity.SetImageGameDataIcon(isEmpty ? null : Item.ItemRefine);

            if (uiTextItemType != null)
            {
                uiTextItemType.text = ZString.Format(
                    LanguageManager.GetText(formatKeyItemType),
                    isEmpty ? LanguageManager.GetUnknowTitle() : Item.TypeTitle);
            }

            if (uiTextExpireDuration != null)
            {
                if (Item != null && Item.ExpireDuration > 0)
                {
                    uiTextExpireDuration.SetGameObjectActive(true);
                    uiTextExpireDuration.text = ZString.Format(
                        LanguageManager.GetText(formatKeyExpireDuration),
                        System.TimeSpan.FromHours(Item.ExpireDuration).GetPrettyDate(true));
                }
                else
                {
                    uiTextExpireDuration.SetGameObjectActive(false);
                }
            }

            // It's how much player selling item to NPC, so use `sellItemPriceRate`
            float sellPriceRate = 1f + (GameInstance.PlayingCharacter == null ? 0 : GameInstance.PlayingCharacter.GetCaches().Stats.sellItemPriceRate);
            if (uiTextSellPrice != null)
            {
                int sellPrice = Mathf.CeilToInt(Item.SellPrice * sellPriceRate);
                uiTextSellPrice.text = ZString.Format(
                    LanguageManager.GetText(formatKeySellPrice),
                    isEmpty ? "0" : sellPrice.ToString("N0"));
            }

            if (uiTextStack != null)
            {
                string stackString;
                if (isEmpty)
                {
                    stackString = ZString.Format(
                        LanguageManager.GetText(formatKeyStack),
                        "0",
                        "0");
                }
                else
                {
                    if (CharacterItem.amount > Item.MaxStack || Item.MaxStack == int.MaxValue)
                    {
                        stackString = ZString.Format(
                            LanguageManager.GetText(formatKeyAmount),
                            CharacterItem.amount.ToString("N0"));
                    }
                    else
                    {
                        stackString = ZString.Format(
                            LanguageManager.GetText(formatKeyStack),
                            CharacterItem.amount.ToString("N0"),
                            Item.MaxStack.ToString("N0"));
                    }
                }
                uiTextStack.SetGameObjectActive(!CharacterItem.IsEmptySlot() && (showAmountWhenMaxIsOne || (Item != null && (CharacterItem.amount > Item.MaxStack || Item.MaxStack > 1))));
                uiTextStack.text = stackString;
            }

            if (uiTextTotalItemAmount != null)
            {
                int total = Character.CountNonEquipItems(CharacterItem.dataId);
                uiTextTotalItemAmount.text = ZString.Format(
                        LanguageManager.GetText(formatKeyTotalItemAmount),
                        total.ToString("N0"));
            }

            if (EquipmentItem != null && EquipmentItem.MaxDurability > 0)
            {
                if (uiTextDurability != null)
                {
                    uiTextDurability.SetGameObjectActive(true);
                    uiTextDurability.text = ZString.Format(
                        LanguageManager.GetText(formatKeyDurability),
                        CharacterItem.durability.ToString("N0"),
                        EquipmentItem.MaxDurability.ToString("N0"));
                }

                if (uiGageDurability != null)
                {
                    uiGageDurability.SetVisible(true);
                    uiGageDurability.Update(CharacterItem.durability, EquipmentItem.MaxDurability);
                }
            }
            else
            {
                if (uiTextDurability != null)
                    uiTextDurability.SetGameObjectActive(false);

                if (uiGageDurability != null)
                    uiGageDurability.SetVisible(false);
            }

            UpdateAmmo();

            if (uiTextWeight != null)
            {
                uiTextWeight.text = ZString.Format(
                    LanguageManager.GetText(formatKeyWeight),
                    isEmpty ? 0f.ToString("N2") : Item.Weight.ToString("N2"));
            }

            if (uiRequirement != null)
            {
                if ((EquipmentItem == null ||
                    (EquipmentItem.Requirement.level <= 0 &&
                    !EquipmentItem.Requirement.HasAvailableClasses() &&
                    !EquipmentItem.Requirement.HasAvailableFactions() &&
                    EquipmentItem.RequireAttributeAmounts.Count == 0)) &&
                    (UsableItem == null ||
                    (UsableItem.Requirement.level <= 0 &&
                    !UsableItem.Requirement.HasAvailableClasses() &&
                    !UsableItem.Requirement.HasAvailableFactions() &&
                    UsableItem.RequireAttributeAmounts.Count == 0)))
                {
                    uiRequirement.Hide();
                }
                else if (EquipmentItem != null)
                {
                    uiRequirement.Show();
                    uiRequirement.Data = EquipmentItem;
                }
                else if (UsableItem != null)
                {
                    uiRequirement.Show();
                    uiRequirement.Data = UsableItem;
                }
            }

            if (uiIncreaseStats != null)
            {
                CharacterStats stats = new CharacterStats();
                if (EquipmentItem != null)
                {
                    stats += EquipmentItem.GetIncreaseStats(Level);
                    if (!dontCalculateRandomBonus)
                    {
                        stats += GetRandomBonus().GetIncreaseStats();
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    stats += SocketEnhancerItem.SocketEnhanceEffect.Stats;
                }

                if (stats.IsEmpty())
                {
                    // Hide ui if stats is empty
                    uiIncreaseStats.Hide();
                }
                else
                {
                    uiIncreaseStats.displayType = UICharacterStats.DisplayType.Simple;
                    uiIncreaseStats.isBonus = true;
                    uiIncreaseStats.Show();
                    uiIncreaseStats.Data = stats;
                }
            }

            if (uiIncreaseStatsRate != null)
            {
                CharacterStats statsRate = new CharacterStats();
                if (EquipmentItem != null)
                {
                    statsRate += EquipmentItem.GetIncreaseStatsRate(Level);
                    if (!dontCalculateRandomBonus)
                    {
                        statsRate += GetRandomBonus().GetIncreaseStatsRate();
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    statsRate += SocketEnhancerItem.SocketEnhanceEffect.StatsRate;
                }

                if (statsRate.IsEmpty())
                {
                    // Hide ui if stats is empty
                    uiIncreaseStatsRate.Hide();
                }
                else
                {
                    uiIncreaseStatsRate.displayType = UICharacterStats.DisplayType.Rate;
                    uiIncreaseStatsRate.isBonus = true;
                    uiIncreaseStatsRate.Show();
                    uiIncreaseStatsRate.Data = statsRate;
                }
            }

            if (uiIncreaseAttributes != null)
            {
                _tempAttributes.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseAttributes(Level, _tempAttributes);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineAttributes(_tempAttributes, GetRandomBonus().GetIncreaseAttributes());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineAttributes(_tempAttributes, SocketEnhancerItem.SocketEnhanceEffect.Attributes);
                }

                if (_tempAttributes == null || _tempAttributes.Count == 0)
                {
                    // Hide ui if attributes is empty
                    uiIncreaseAttributes.Hide();
                }
                else
                {
                    uiIncreaseAttributes.displayType = UIAttributeAmounts.DisplayType.Simple;
                    uiIncreaseAttributes.isBonus = true;
                    uiIncreaseAttributes.Show();
                    uiIncreaseAttributes.Data = _tempAttributes;
                }
            }

            if (uiIncreaseAttributesRate != null)
            {
                _tempAttributesRate.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseAttributesRate(Level, _tempAttributesRate);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineAttributes(_tempAttributesRate, GetRandomBonus().GetIncreaseAttributesRate());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineAttributes(_tempAttributesRate, SocketEnhancerItem.SocketEnhanceEffect.AttributesRate);
                }

                if (_tempAttributesRate == null || _tempAttributesRate.Count == 0)
                {
                    // Hide ui if attributes is empty
                    uiIncreaseAttributesRate.Hide();
                }
                else
                {
                    uiIncreaseAttributesRate.displayType = UIAttributeAmounts.DisplayType.Rate;
                    uiIncreaseAttributesRate.isBonus = true;
                    uiIncreaseAttributesRate.Show();
                    uiIncreaseAttributesRate.Data = _tempAttributesRate;
                }
            }

            if (uiIncreaseResistances != null)
            {
                _tempResistances.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseResistances(Level, _tempResistances);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineResistances(_tempResistances, GetRandomBonus().GetIncreaseResistances());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineResistances(_tempResistances, SocketEnhancerItem.SocketEnhanceEffect.Resistances);
                }

                if (_tempResistances == null || _tempResistances.Count == 0)
                {
                    // Hide ui if resistances is empty
                    uiIncreaseResistances.Hide();
                }
                else
                {
                    uiIncreaseResistances.isBonus = true;
                    uiIncreaseResistances.Show();
                    uiIncreaseResistances.Data = _tempResistances;
                }
            }

            if (uiIncreaseArmors != null)
            {
                _tempArmors.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseArmors(Level, _tempArmors);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineArmors(_tempArmors, GetRandomBonus().GetIncreaseArmors());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineArmors(_tempArmors, SocketEnhancerItem.SocketEnhanceEffect.Armors);
                }

                if (_tempArmors == null || _tempArmors.Count == 0)
                {
                    // Hide ui if armors is empty
                    uiIncreaseArmors.Hide();
                }
                else
                {
                    uiIncreaseArmors.displayType = UIArmorAmounts.DisplayType.Simple;
                    uiIncreaseArmors.isBonus = true;
                    uiIncreaseArmors.Show();
                    uiIncreaseArmors.Data = _tempArmors;
                }
            }

            if (uiIncreaseArmorsRate != null)
            {
                _tempArmorsRate.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseArmorsRate(Level, _tempArmorsRate);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineArmors(_tempArmorsRate, GetRandomBonus().GetIncreaseArmorsRate());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineArmors(_tempArmorsRate, SocketEnhancerItem.SocketEnhanceEffect.ArmorsRate);
                }

                if (_tempArmorsRate == null || _tempArmorsRate.Count == 0)
                {
                    // Hide ui if armors is empty
                    uiIncreaseArmorsRate.Hide();
                }
                else
                {
                    uiIncreaseArmorsRate.displayType = UIArmorAmounts.DisplayType.Rate;
                    uiIncreaseArmorsRate.isBonus = true;
                    uiIncreaseArmorsRate.Show();
                    uiIncreaseArmorsRate.Data = _tempArmorsRate;
                }
            }

            if (uiIncreaseDamages != null)
            {
                _tempDamageAmounts.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseDamages(Level, _tempDamageAmounts);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineDamages(_tempDamageAmounts, GetRandomBonus().GetIncreaseDamages());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineDamages(_tempDamageAmounts, SocketEnhancerItem.SocketEnhanceEffect.Damages);
                }

                if (_tempDamageAmounts == null || _tempDamageAmounts.Count == 0)
                {
                    // Hide ui if damage amounts is empty
                    uiIncreaseDamages.Hide();
                }
                else
                {
                    uiIncreaseDamages.displayType = UIDamageElementAmounts.DisplayType.Simple;
                    uiIncreaseDamages.isBonus = true;
                    uiIncreaseDamages.Show();
                    uiIncreaseDamages.Data = _tempDamageAmounts;
                }
            }

            if (uiIncreaseDamagesRate != null)
            {
                _tempDamageAmountsRate.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseDamagesRate(Level, _tempDamageAmountsRate);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineDamages(_tempDamageAmountsRate, GetRandomBonus().GetIncreaseDamagesRate());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineDamages(_tempDamageAmountsRate, SocketEnhancerItem.SocketEnhanceEffect.DamagesRate);
                }

                if (_tempDamageAmountsRate == null || _tempDamageAmountsRate.Count == 0)
                {
                    // Hide ui if damage amounts is empty
                    uiIncreaseDamagesRate.Hide();
                }
                else
                {
                    uiIncreaseDamages.displayType = UIDamageElementAmounts.DisplayType.Rate;
                    uiIncreaseDamagesRate.isBonus = true;
                    uiIncreaseDamagesRate.Show();
                    uiIncreaseDamagesRate.Data = _tempDamageAmountsRate;
                }
            }

            if (uiStatusEffectResistances != null)
            {
                _tempStatusEffectResistances.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseStatusEffectResistances(Level, _tempStatusEffectResistances);
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineStatusEffectResistances(_tempStatusEffectResistances, SocketEnhancerItem.SocketEnhanceEffect.StatusEffectResistances);
                }

                if (_tempStatusEffectResistances == null || _tempStatusEffectResistances.Count == 0)
                {
                    // Hide ui if armors is empty
                    uiStatusEffectResistances.Hide();
                }
                else
                {
                    uiStatusEffectResistances.isBonus = true;
                    uiStatusEffectResistances.Show();
                    uiStatusEffectResistances.UpdateData(_tempStatusEffectResistances);
                }
            }

            if (uiIncreaseSkillLevels != null)
            {
                _tempSkillLevels.Clear();
                if (EquipmentItem != null)
                {
                    EquipmentItem.GetIncreaseSkills(Level, _tempSkillLevels);
                    if (!dontCalculateRandomBonus)
                    {
                        GameDataHelpers.CombineSkills(_tempSkillLevels, GetRandomBonus().GetIncreaseSkills());
                    }
                }
                else if (SocketEnhancerItem != null)
                {
                    GameDataHelpers.CombineSkills(_tempSkillLevels, SocketEnhancerItem.SocketEnhanceEffect.Skills);
                }

                if (_tempSkillLevels == null || _tempSkillLevels.Count == 0)
                {
                    // Hide ui if skill levels is empty
                    uiIncreaseSkillLevels.Hide();
                }
                else
                {
                    uiIncreaseSkillLevels.displayType = UISkillLevels.DisplayType.Simple;
                    uiIncreaseSkillLevels.isBonus = true;
                    uiIncreaseSkillLevels.Show();
                    uiIncreaseSkillLevels.Data = _tempSkillLevels;
                }
            }

            if (uiEquipmentSet != null)
            {
                if (EquipmentItem == null || EquipmentItem.EquipmentSet == null || EquipmentItem.EquipmentSet.Effects.Length == 0)
                {
                    // Only equipment item has equipment set data
                    uiEquipmentSet.Hide();
                }
                else
                {
                    uiEquipmentSet.Show();
                    int equippedCount;
                    Character.GetCaches().EquipmentSets.TryGetValue(EquipmentItem.EquipmentSet, out equippedCount);
                    uiEquipmentSet.Data = new UIEquipmentSetData(EquipmentItem.EquipmentSet, equippedCount);
                }
            }

            if (uiEquipmentSockets != null)
            {
                int maxSocket = EquipmentItem?.AvailableSocketEnhancerTypes?.Length ?? 0;
                if (maxSocket <= 0)
                {
                    uiEquipmentSockets.Hide();
                }
                else
                {
                    uiEquipmentSockets.Show();
                    uiEquipmentSockets.Data = new UIEquipmentSocketsData(CharacterItem.sockets, EquipmentItem.AvailableSocketEnhancerTypes);
                }
            }

            if (uiItemRandomBonus != null)
            {
                if (EquipmentItem == null)
                {
                    uiItemRandomBonus.Hide();
                }
                else
                {
                    uiItemRandomBonus.Show();
                    uiItemRandomBonus.Data = EquipmentItem.RandomBonus;
                }
            }

            if (uiRefineItem != null)
            {
                if (EquipmentItem == null)
                {
                    uiRefineItem.Hide();
                }
                else
                {
                    uiRefineItem.Show();
                    uiRefineItem.Data = new UIOwningCharacterItemData(InventoryType, IndexOfData, EquipSlotIndex);
                }
            }

            if (uiDismantleItem != null)
            {
                if (!GameInstance.Singleton.dismantleFilter.Filter(CharacterItem))
                {
                    uiDismantleItem.Hide();
                }
                else
                {
                    uiDismantleItem.Show();
                    uiDismantleItem.Data = new UIOwningCharacterItemData(InventoryType, IndexOfData, EquipSlotIndex);
                }
            }

            if (uiRepairItem != null)
            {
                if (EquipmentItem == null)
                {
                    uiRepairItem.Hide();
                }
                else
                {
                    uiRepairItem.Show();
                    uiRepairItem.Data = new UIOwningCharacterItemData(InventoryType, IndexOfData, EquipSlotIndex);
                }
            }

            if (uiArmorAmount != null)
            {
                if (DefendItem == null)
                {
                    uiArmorAmount.Hide();
                }
                else
                {
                    uiArmorAmount.Show();
                    KeyValuePair<DamageElement, float> kvPair = CharacterItem.GetArmorAmount();
                    uiArmorAmount.Data = new UIArmorAmountData(kvPair.Key, kvPair.Value);
                }
            }

            if (uiDamageAmount != null)
            {
                if (WeaponItem == null)
                {
                    uiDamageAmount.Hide();
                }
                else
                {
                    uiDamageAmount.Show();
                    KeyValuePair<DamageElement, MinMaxFloat> kvPair = CharacterItem.GetDamageAmount();
                    uiDamageAmount.Data = new UIDamageElementAmountData(kvPair.Key, kvPair.Value);
                }
            }

            BaseMonsterCharacterEntity tempMonsterEntity = null;
            if (ItemWithMonsterEntity != null)
            {
#if !DISABLE_ADDRESSABLES
                tempMonsterEntity = ItemWithMonsterEntity.AddressableMonsterCharacterEntity
                    .GetOrLoadAssetOrUsePrefab(ItemWithMonsterEntity.MonsterCharacterEntity);
#else
                tempMonsterEntity = ItemWithMonsterEntity.MonsterCharacterEntity;
#endif
            }
            if (tempMonsterEntity == null)
            {
                if (uiTextMonster != null)
                    uiTextMonster.SetGameObjectActive(false);

                if (uiTextExp != null)
                    uiTextExp.SetGameObjectActive(false);

                if (uiGageExp != null)
                    uiGageExp.SetVisible(false);
            }
            else
            {
                if (uiTextMonster != null)
                {
                    uiTextMonster.SetGameObjectActive(true);
                    uiTextMonster.text = ZString.Format(
                        LanguageManager.GetText(formatKeyPet),
                        tempMonsterEntity.Title);
                }

                // Item's EXP (pet's EXP)
                int currentExp;
                int nextLevelExp;
                tempMonsterEntity.GetProperCurrentByNextLevelExp(CharacterItem.level, CharacterItem.exp, out currentExp, out nextLevelExp);

                if (uiTextExp != null)
                {
                    uiTextExp.SetGameObjectActive(true);
                    uiTextExp.text = ZString.Format(
                        LanguageManager.GetText(formatKeyExp),
                        currentExp.ToString("N0"),
                        nextLevelExp.ToString("N0"));
                }

                if (uiGageExp != null)
                {
                    uiGageExp.SetVisible(true);
                    uiGageExp.Update(currentExp, nextLevelExp);
                }
            }

            BuildingEntity tempBuildingEntity = null;
            if (ItemWithBuildingEntity != null)
            {
#if !DISABLE_ADDRESSABLES
                tempBuildingEntity = ItemWithBuildingEntity.AddressableBuildingEntity
                    .GetOrLoadAssetOrUsePrefab(ItemWithBuildingEntity.BuildingEntity);
#else
                tempBuildingEntity = ItemWithBuildingEntity.BuildingEntity;
#endif
            }
            if (tempBuildingEntity == null)
            {
                if (uiTextBuilding != null)
                    uiTextBuilding.SetGameObjectActive(false);
            }
            else
            {
                if (uiTextBuilding != null)
                {
                    uiTextBuilding.SetGameObjectActive(true);
                    uiTextBuilding.text = ZString.Format(
                        LanguageManager.GetText(formatKeyBuilding),
                        tempBuildingEntity.Title);
                }
            }

            VehicleEntity tempVehicleEntity = null;
            if (ItemWithVehicleEntity != null)
            {
#if !DISABLE_ADDRESSABLES
                tempVehicleEntity = ItemWithVehicleEntity.AddressableVehicleEntity
                        .GetOrLoadAssetOrUsePrefab(ItemWithVehicleEntity.VehicleEntity);
#else
                tempVehicleEntity = ItemWithVehicleEntity.VehicleEntity;
#endif
            }
            if (tempVehicleEntity == null)
            {
                if (uiTextVehicle != null)
                    uiTextVehicle.SetGameObjectActive(false);
            }
            else
            {
                if (uiTextVehicle != null)
                {
                    uiTextVehicle.SetGameObjectActive(true);
                    uiTextVehicle.text = ZString.Format(
                        LanguageManager.GetText(formatKeyMount),
                        tempVehicleEntity.Title);
                }
            }

            if (uiBuff != null)
            {
                if (ItemWithBuffData == null || ItemWithBuffData.BuffData == null)
                {
                    uiBuff.Hide();
                }
                else
                {
                    uiBuff.Show();
                    uiBuff.Data = new UIBuffData(ItemWithBuffData.BuffData, Level);
                }
            }

            if (uiSkill != null)
            {
                if (ItemWithSkillData == null || ItemWithSkillData.SkillData == null)
                {
                    uiSkill.Hide();
                }
                else
                {
                    uiSkill.Setup(new UICharacterSkillData(ItemWithSkillData.SkillData, ItemWithSkillData.SkillLevel), Character, -1);
                    uiSkill.Show();
                }
            }

            if (uiTextSkill != null)
            {
                if (ItemWithSkillData == null || ItemWithSkillData.SkillData == null)
                {
                    uiTextSkill.SetGameObjectActive(false);
                }
                else
                {
                    uiTextSkill.SetGameObjectActive(true);
                    uiTextSkill.text = ZString.Format(
                        LanguageManager.GetText(formatKeySkill),
                        ItemWithSkillData.SkillData.Title,
                        ItemWithSkillData.SkillLevel);
                }
            }

            if (uiAttribute != null)
            {
                if (ItemWithAttributeData == null || ItemWithAttributeData.AttributeData == null)
                {
                    uiAttribute.Hide();
                }
                else
                {
                    uiAttribute.Setup(new UICharacterAttributeData(ItemWithAttributeData.AttributeData, ItemWithAttributeData.AttributeAmount), Character, -1);
                    uiAttribute.Show();
                }
            }

            if (uiTextAttribute != null)
            {
                if (ItemWithAttributeData == null || ItemWithAttributeData.AttributeData == null)
                {
                    uiTextAttribute.SetGameObjectActive(false);
                }
                else
                {
                    uiTextAttribute.SetGameObjectActive(true);
                    uiTextAttribute.text = ZString.Format(
                        LanguageManager.GetText(formatKeyAttribute),
                        ItemWithAttributeData.AttributeData.Title,
                        ItemWithAttributeData.AttributeAmount);
                }
            }

            if (uiStatusEffectApplyingsSelfWhenAttacking != null)
            {
                if (ItemWithStatusEffectApplyings == null || ItemWithStatusEffectApplyings.SelfStatusEffectsWhenAttacking == null || ItemWithStatusEffectApplyings.SelfStatusEffectsWhenAttacking.Length == 0)
                {
                    uiStatusEffectApplyingsSelfWhenAttacking.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsSelfWhenAttacking.UpdateData(ItemWithStatusEffectApplyings.SelfStatusEffectsWhenAttacking, Level, UIStatusEffectApplyingTarget.SelfWhenAttacking);
                    uiStatusEffectApplyingsSelfWhenAttacking.Show();
                }
            }

            if (uiStatusEffectApplyingsEnemyWhenAttacking != null)
            {
                if (ItemWithStatusEffectApplyings == null || ItemWithStatusEffectApplyings.EnemyStatusEffectsWhenAttacking == null || ItemWithStatusEffectApplyings.EnemyStatusEffectsWhenAttacking.Length == 0)
                {
                    uiStatusEffectApplyingsEnemyWhenAttacking.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsEnemyWhenAttacking.UpdateData(ItemWithStatusEffectApplyings.EnemyStatusEffectsWhenAttacking, Level, UIStatusEffectApplyingTarget.EnemyWhenAttacking);
                    uiStatusEffectApplyingsEnemyWhenAttacking.Show();
                }
            }

            if (uiStatusEffectApplyingsSelfWhenAttacked != null)
            {
                if (ItemWithStatusEffectApplyings == null || ItemWithStatusEffectApplyings.SelfStatusEffectsWhenAttacked == null || ItemWithStatusEffectApplyings.SelfStatusEffectsWhenAttacked.Length == 0)
                {
                    uiStatusEffectApplyingsSelfWhenAttacked.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsSelfWhenAttacked.UpdateData(ItemWithStatusEffectApplyings.SelfStatusEffectsWhenAttacked, Level, UIStatusEffectApplyingTarget.SelfWhenAttacked);
                    uiStatusEffectApplyingsSelfWhenAttacked.Show();
                }
            }

            if (uiStatusEffectApplyingsEnemyWhenAttacked != null)
            {
                if (ItemWithStatusEffectApplyings == null || ItemWithStatusEffectApplyings.EnemyStatusEffectsWhenAttacked == null || ItemWithStatusEffectApplyings.EnemyStatusEffectsWhenAttacked.Length == 0)
                {
                    uiStatusEffectApplyingsEnemyWhenAttacked.Hide();
                }
                else
                {
                    uiStatusEffectApplyingsEnemyWhenAttacked.UpdateData(ItemWithStatusEffectApplyings.EnemyStatusEffectsWhenAttacked, Level, UIStatusEffectApplyingTarget.EnemyWhenAttacked);
                    uiStatusEffectApplyingsEnemyWhenAttacked.Show();
                }
            }

            if (uiNextLevelItem != null)
            {
                if (Level + 1 > Item.MaxLevel)
                {
                    uiNextLevelItem.Hide();
                }
                else
                {
                    uiNextLevelItem.Setup(new UICharacterItemData(CharacterItem, Level + 1, InventoryType), Character, IndexOfData);
                    uiNextLevelItem.Show();
                }
            }

            if (uiComparingEquipments != null && !dontShowComparingEquipments)
            {
                foreach (UICharacterItem uiComparingEquipment in uiComparingEquipments)
                {
                    uiComparingEquipment.Hide();
                }
                if (IsOwningCharacter())
                {
                    int comparingEquipmentIndex = 0;
                    if (WeaponItem != null)
                    {
                        if (!isEmptyRightHand &&
                            !isThisRightHand &&
                            rightHandWeapon != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                equippedRightHand,
                                InventoryType.EquipWeaponRight, 0);
                            comparingEquipmentIndex++;
                        }
                        if (!isEmptyLeftHand &&
                            !isThisLeftHand &&
                            leftHandWeapon != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                equippedLeftHand,
                                InventoryType.EquipWeaponLeft, 0);
                            comparingEquipmentIndex++;
                        }
                    }
                    if (ShieldItem != null)
                    {
                        if (!isEmptyRightHand &&
                            !isThisRightHand &&
                            rightHandShield != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                equippedRightHand,
                                InventoryType.EquipWeaponRight, 0);
                            comparingEquipmentIndex++;
                        }
                        if (!isEmptyLeftHand &&
                            !isThisLeftHand &&
                            leftHandShield != null)
                        {
                            SetupAndShowUIComparingEquipment(comparingEquipmentIndex,
                                equippedLeftHand,
                                InventoryType.EquipWeaponLeft, 0);
                            comparingEquipmentIndex++;
                        }
                    }
                    if (ArmorItem != null)
                    {
                        CharacterItem equipItem;
                        for (int equipItemIndex = 0; equipItemIndex < GameInstance.PlayingCharacter.EquipItems.Count; ++equipItemIndex)
                        {
                            equipItem = GameInstance.PlayingCharacter.EquipItems[equipItemIndex];
                            if (!equipItem.IsEmptySlot() &&
                                !string.Equals(equipItem.id, CharacterItem.id) &&
                                equipItem.GetArmorItem() != null &&
                                equipItem.GetArmorItem().ArmorType == ArmorItem.ArmorType)
                            {
                                SetupAndShowUIComparingEquipment(comparingEquipmentIndex, equipItem, InventoryType.EquipItems, equipItemIndex);
                                comparingEquipmentIndex++;
                            }
                        }
                    }
                }
            }

            UpdateShopUIVisibility(true);
            UpdateRefineItemUIVisibility(true);
            UpdateDismantleItemUIVisibility(true);
            UpdateRepairItemUIVisibility(true);
            UpdateEnhanceSocketUIVisibility(true);
            UpdateStorageUIVisibility(true);
            UpdateDealingState(true);
            UpdateStartVendingUIVisibility(true);
        }

        private void SetupAndShowUIComparingEquipment(int index, CharacterItem characterItem, InventoryType inventoryType, int indexOfData)
        {
            if (uiComparingEquipments == null || index >= uiComparingEquipments.Length)
                return;
            uiComparingEquipments[index].Setup(new UICharacterItemData(characterItem, inventoryType), GameInstance.PlayingCharacter, indexOfData);
            uiComparingEquipments[index].Show();
        }

        private void UpdateShopUIVisibility(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isSellItemDialogAppeared)
                {
                    _isSellItemDialogAppeared = false;
                    if (onNpcSellItemDialogDisappear != null)
                        onNpcSellItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsShopDialogVisible())
            {
                if (isInit || !_isSellItemDialogAppeared)
                {
                    _isSellItemDialogAppeared = true;
                    if (onNpcSellItemDialogAppear != null)
                        onNpcSellItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (isInit || _isSellItemDialogAppeared)
                {
                    _isSellItemDialogAppeared = false;
                    if (onNpcSellItemDialogDisappear != null)
                        onNpcSellItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateRefineItemUIVisibility(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isRefineItemDialogAppeared)
                {
                    _isRefineItemDialogAppeared = false;
                    if (onRefineItemDialogDisappear != null)
                        onRefineItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsRefineItemDialogVisible() &&
                EquipmentItem != null && InventoryType != InventoryType.StorageItems)
            {
                if (isInit || !_isRefineItemDialogAppeared)
                {
                    _isRefineItemDialogAppeared = true;
                    if (onRefineItemDialogAppear != null)
                        onRefineItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (isInit || _isRefineItemDialogAppeared)
                {
                    _isRefineItemDialogAppeared = false;
                    if (onRefineItemDialogDisappear != null)
                        onRefineItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateDismantleItemUIVisibility(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isDismantleItemDialogAppeared)
                {
                    _isDismantleItemDialogAppeared = false;
                    if (onDismantleItemDialogDisappear != null)
                        onDismantleItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsDismantleItemDialogVisible() &&
                GameInstance.Singleton.dismantleFilter.Filter(CharacterItem))
            {
                if (isInit || !_isDismantleItemDialogAppeared)
                {
                    _isDismantleItemDialogAppeared = true;
                    if (onDismantleItemDialogAppear != null)
                        onDismantleItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (isInit || _isDismantleItemDialogAppeared)
                {
                    _isDismantleItemDialogAppeared = false;
                    if (onDismantleItemDialogDisappear != null)
                        onDismantleItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateRepairItemUIVisibility(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isRepairItemDialogAppeared)
                {
                    _isRepairItemDialogAppeared = false;
                    if (onRepairItemDialogDisappear != null)
                        onRepairItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsRepairItemDialogVisible() &&
                EquipmentItem != null && InventoryType != InventoryType.StorageItems)
            {
                if (isInit || !_isRepairItemDialogAppeared)
                {
                    _isRepairItemDialogAppeared = true;
                    if (onRepairItemDialogAppear != null)
                        onRepairItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (isInit || _isRepairItemDialogAppeared)
                {
                    _isRepairItemDialogAppeared = false;
                    if (onRepairItemDialogDisappear != null)
                        onRepairItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateEnhanceSocketUIVisibility(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isEnhanceSocketItemDialogAppeared)
                {
                    _isEnhanceSocketItemDialogAppeared = false;
                    if (onEnhanceSocketItemDialogDisappear != null)
                        onEnhanceSocketItemDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsEnhanceSocketItemDialogVisible() &&
                EquipmentItem != null && InventoryType != InventoryType.StorageItems)
            {
                if (isInit || !_isEnhanceSocketItemDialogAppeared)
                {
                    _isEnhanceSocketItemDialogAppeared = true;
                    if (onEnhanceSocketItemDialogAppear != null)
                        onEnhanceSocketItemDialogAppear.Invoke();
                }
            }
            else
            {
                if (isInit || _isEnhanceSocketItemDialogAppeared)
                {
                    _isEnhanceSocketItemDialogAppeared = false;
                    if (onEnhanceSocketItemDialogDisappear != null)
                        onEnhanceSocketItemDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateStorageUIVisibility(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isStorageDialogAppeared)
                {
                    _isStorageDialogAppeared = false;
                    if (onStorageDialogDisappear != null)
                        onStorageDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsStorageDialogVisible())
            {
                if (isInit || !_isStorageDialogAppeared)
                {
                    _isStorageDialogAppeared = true;
                    if (onStorageDialogAppear != null)
                        onStorageDialogAppear.Invoke();
                }
            }
            else
            {
                if (isInit || _isStorageDialogAppeared)
                {
                    _isStorageDialogAppeared = false;
                    if (onStorageDialogDisappear != null)
                        onStorageDialogDisappear.Invoke();
                }
            }
        }

        private void UpdateDealingState(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isDealingStateEntered)
                {
                    _isDealingStateEntered = false;
                    if (onExitDealingState != null)
                        onExitDealingState.Invoke();
                }
                return;
            }
            // Check visible dealing dialog
            if (GameInstance.ItemUIVisibilityManager.IsDealingDialogVisibleWithDealingState() &&
                InventoryType == InventoryType.NonEquipItems)
            {
                if (isInit || !_isDealingStateEntered)
                {
                    _isDealingStateEntered = true;
                    if (onEnterDealingState != null)
                        onEnterDealingState.Invoke();
                }
            }
            else
            {
                if (isInit || _isDealingStateEntered)
                {
                    _isDealingStateEntered = false;
                    if (onExitDealingState != null)
                        onExitDealingState.Invoke();
                }
            }
        }

        private void UpdateStartVendingUIVisibility(bool isInit)
        {
            if (!IsOwningCharacter())
            {
                if (isInit || _isStartVendingDialogAppeared)
                {
                    _isStartVendingDialogAppeared = false;
                    if (onStartVendingDialogDisappear != null)
                        onStartVendingDialogDisappear.Invoke();
                }
                return;
            }
            // Check visible item dialog
            if (GameInstance.ItemUIVisibilityManager.IsStartVendingDialogVisible() &&
                InventoryType == InventoryType.NonEquipItems)
            {
                if (isInit || !_isStartVendingDialogAppeared)
                {
                    _isStartVendingDialogAppeared = true;
                    if (onStartVendingDialogAppear != null)
                        onStartVendingDialogAppear.Invoke();
                }
            }
            else
            {
                if (isInit || _isStartVendingDialogAppeared)
                {
                    _isStartVendingDialogAppeared = false;
                    if (onStartVendingDialogDisappear != null)
                        onStartVendingDialogDisappear.Invoke();
                }
            }
        }

        public void OnClickEquip()
        {
            // Only unequpped equipment can be equipped
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();

            GameInstance.ClientInventoryHandlers.RequestEquipItem(
                GameInstance.PlayingCharacter,
                IndexOfData,
                GameInstance.PlayingCharacter.EquipWeaponSet,
                ClientInventoryActions.ResponseEquipArmor,
                ClientInventoryActions.ResponseEquipWeapon);
        }

        public void OnClickUnEquip()
        {
            // Only equipped equipment can be unequipped
            if (!IsOwningCharacter() || (InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();

            GameInstance.ClientInventoryHandlers.RequestUnEquipItem(
                InventoryType,
                IndexOfData,
                CharacterItem.equipSlotIndex,
                -1,
                ClientInventoryActions.ResponseUnEquipArmor,
                ClientInventoryActions.ResponseUnEquipWeapon);
        }

        public void OnClickRemoveAmmo()
        {
            // Only equipped equipment can be unequipped
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            GameInstance.ClientInventoryHandlers.RequestRemoveAmmoFromItem(new RequestRemoveAmmoFromItemMessage()
            {
                inventoryType = InventoryType,
                index = IndexOfData,
                equipSlotIndex = EquipSlotIndex,
            }, ClientInventoryActions.ResponseRemoveAmmoFromItem);
        }

        public void OnClickUse()
        {
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (UsableItem == null)
                return;

            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();

            if (UsableItem.HasCustomAimControls())
            {
                // Controlling by hotkey controller
                UICharacterHotkeys.SetupHotkeyForAimming(HotkeyType.Item, Item.Id);
                onClickAimToUseItem.Invoke();
                return;
            }

            UICharacterHotkeys.SetupHotkeyForAimming(HotkeyType.Item, Item.Id);
            onClickUseItem.Invoke();
        }

        #region Drop Item Functions
        public void OnClickDrop()
        {
            // Only unequipped equipment can be dropped
            if (!IsOwningCharacter() || GameInstance.PlayingCharacterEntity == null)
                return;

            switch (GameInstance.Singleton.playerDropItemMode)
            {
                case PlayerDropItemMode.DestroyItem:
                    if (CharacterItem.amount == 1)
                    {
                        UISceneGlobal.Singleton.ShowMessageDialog(
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM.ToString()),
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM_DESCRIPTION.ToString()),
                            false, true, true, false, null, () =>
                            {
                                OnDropAmountConfirmed(1);
                            });
                    }
                    else
                    {
                        UISceneGlobal.Singleton.ShowInputDialog(
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM.ToString()),
                            LanguageManager.GetText(UITextKeys.UI_DESTROY_ITEM_DESCRIPTION.ToString()),
                            OnDropAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
                    }
                    break;
                case PlayerDropItemMode.DropOnGround:
                    if (CharacterItem.amount == 1)
                    {
                        OnDropAmountConfirmed(1);
                    }
                    else
                    {
                        UISceneGlobal.Singleton.ShowInputDialog(
                            LanguageManager.GetText(UITextKeys.UI_DROP_ITEM.ToString()),
                            LanguageManager.GetText(UITextKeys.UI_DROP_ITEM_DESCRIPTION.ToString()),
                            OnDropAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
                    }
                    break;
            }
        }

        private void OnDropAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.PlayingCharacterEntity.CallCmdDropItem(InventoryType, IndexOfData, EquipSlotIndex, amount);
        }
        #endregion

        #region Sell Item Functions
        public void OnClickSell()
        {
            // Only unequipped equipment can be sold
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (CharacterItem.amount == 1)
            {
                OnSellItemAmountConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_SELL_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_SELL_ITEM_DESCRIPTION.ToString()), OnSellItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnSellItemAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            if (amount <= 0)
                return;
            GameInstance.ClientInventoryHandlers.RequestSellItem(new RequestSellItemMessage()
            {
                inventoryType = InventoryType,
                index = IndexOfData,
                equipSlotIndex = EquipSlotIndex,
                amount = amount,
            }, ClientInventoryActions.ResponseSellItem);
        }
        #endregion

        #region Set Dealing Item Functions
        public void OnClickSetDealingItem()
        {
            // Only unequipped equipment can be offered
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (CharacterItem.amount == 1)
            {
                OnSetDealingItemAmountConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_OFFER_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_OFFER_ITEM_DESCRIPTION.ToString()), OnSetDealingItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnSetDealingItemAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdSetDealingItem(CharacterItem.id, amount);
        }
        #endregion

        #region Move To Storage Functions
        public void OnClickMoveToStorage()
        {
            if (GameInstance.OpenedStorages.Count > 0)
            {
                StorageId storageId = GameInstance.OpenedStorages.Keys.FirstOrDefault();
                OnClickMoveToStorage(storageId.storageType, storageId.storageOwnerId, -1);
            }
        }

        public void OnClickMoveToStorage(StorageType storageType, string storageOwnerId, int storageItemIndex)
        {
            // Only unequipped equipment can be moved to storage
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (CharacterItem.amount == 1)
            {
                OnClickMoveToStorageConfirmed(storageType, storageOwnerId, storageItemIndex, 1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_TO_STORAGE.ToString()), LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_TO_STORAGE_DESCRIPTION.ToString()), (amount) =>
                {
                    OnClickMoveToStorageConfirmed(storageType, storageOwnerId, storageItemIndex, amount);
                }, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnClickMoveToStorageConfirmed(StorageType storageType, string storageOwnerId, int storageItemIndex, int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.ClientStorageHandlers.RequestMoveItemToStorage(new RequestMoveItemToStorageMessage()
            {
                storageType = storageType,
                storageOwnerId = storageOwnerId,
                inventoryItemIndex = IndexOfData,
                inventoryItemAmount = amount,
                storageItemIndex = storageItemIndex,
                inventoryType = InventoryType,
                equipSlotIndexOrWeaponSet = EquipSlotIndex,
            }, ClientStorageActions.ResponseMoveItemToStorage);
        }
        #endregion

        #region Move From Storage Functions
        public void OnClickMoveFromStorage()
        {
            OnClickMoveFromStorage(InventoryType.NonEquipItems, Character.EquipWeaponSet, -1);
        }

        public void OnClickMoveFromStorage(InventoryType inventoryType, byte equipSlotIndex, int inventoryItemIndex)
        {
            // Only storage items can be moved from storage
            if (!IsOwningCharacter() || InventoryType != InventoryType.StorageItems)
                return;

            if (CharacterItem.amount == 1)
            {
                OnClickMoveFromStorageConfirmed(inventoryType, equipSlotIndex, inventoryItemIndex, 1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_STORAGE.ToString()), LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_STORAGE_DESCRIPTION.ToString()), (amount) =>
                {
                    OnClickMoveFromStorageConfirmed(inventoryType, equipSlotIndex, inventoryItemIndex, amount);
                }, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnClickMoveFromStorageConfirmed(InventoryType inventoryType, byte equipSlotIndex, int inventoryItemIndex, int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();

            StorageType storageType = StorageType;
            string storageOwnerId = StorageOwnerId;
            GameInstance.ClientStorageHandlers.RequestMoveItemFromStorage(new RequestMoveItemFromStorageMessage()
            {
                storageType = storageType,
                storageOwnerId = storageOwnerId,
                storageItemIndex = IndexOfData,
                storageItemAmount = amount,
                inventoryItemIndex = inventoryItemIndex,
                inventoryType = inventoryType,
                equipSlotIndexOrWeaponSet = equipSlotIndex,
            }, ClientStorageActions.ResponseMoveItemFromStorage);
        }
        #endregion

        #region Move From Items Container Functions
        public void OnClickPickUpFromContainer()
        {
            // Only Items container items can be moved from ItemsContainer
            if (!IsOwningCharacter() || InventoryType != InventoryType.ItemsContainer)
                return;

            if (CharacterItem.amount == 1)
            {
                OnClickPickUpFromContainerConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER.ToString()), LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER_DESCRIPTION.ToString()), (amount) =>
                {
                    OnClickPickUpFromContainerConfirmed(amount);
                }, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnClickPickUpFromContainerConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.PlayingCharacterEntity.CallCmdPickupItemFromContainer(GameInstance.ItemsContainerUIVisibilityManager.ItemsContainerEntity.ObjectId, IndexOfData, amount);
        }
        #endregion

        #region Set Refine Item Functions
        /// <summary>
        /// Use this function to set item which you want to refine to `UIRefineItem` instance
        /// </summary>
        public void OnClickSetRefineItem()
        {
            // Only owning character can refine item
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ItemUIVisibilityManager.ShowRefineItemDialog(InventoryType, IndexOfData, EquipSlotIndex);
                if (selectionManager != null)
                    selectionManager.DeselectSelectedUI();
            }
        }

        /// <summary>
        /// Use this function to refine the item
        /// </summary>
        public void OnClickRefineItem()
        {
            if (!GameInstance.Singleton.canRefineItemByPlayer)
                return;

            // Only owning character can refine item
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ClientInventoryHandlers.RequestRefineItem(new RequestRefineItemMessage()
                {
                    inventoryType = InventoryType,
                    index = IndexOfData,
                    equipSlotIndex = EquipSlotIndex,
                }, ClientInventoryActions.ResponseRefineItem);
            }
        }
        #endregion

        #region Set Dismantle Item Functions
        /// <summary>
        /// Use this function to set item which you want to dismantle to `UIDismantleItem` instance
        /// </summary>
        public void OnClickSetDismantleItem()
        {
            // Only unequipped equipment can be dismantled
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (!GameInstance.Singleton.dismantleFilter.Filter(CharacterItem))
                return;

            GameInstance.ItemUIVisibilityManager.ShowDismantleItemDialog(InventoryType, IndexOfData, EquipSlotIndex);
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
        }

        /// <summary>
        /// Use this function to dismantle the item
        /// </summary>
        public void OnClickDismantleItem()
        {
            if (!GameInstance.Singleton.canDismantleItemByPlayer)
                return;

            // Only unequipped equipment can be dismantled
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (!GameInstance.Singleton.dismantleFilter.Filter(CharacterItem))
                return;

            if (CharacterItem.amount == 1)
            {
                OnDismantleItemAmountConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM.ToString()), LanguageManager.GetText(UITextKeys.UI_DISMANTLE_ITEM_DESCRIPTION.ToString()), OnDismantleItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnDismantleItemAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            GameInstance.ClientInventoryHandlers.RequestDismantleItem(new RequestDismantleItemMessage()
            {
                inventoryType = InventoryType,
                index = IndexOfData,
                equipSlotIndex = EquipSlotIndex,
                amount = amount,
            }, ClientInventoryActions.ResponseDismantleItem);
        }
        #endregion

        #region Set Repair Item Functions
        /// <summary>
        /// Use this function to set item which you want to repair to `UIRepairItem` instance
        /// </summary>
        public void OnClickSetRepairItem()
        {
            // Only owning character can repair item
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ItemUIVisibilityManager.ShowRepairItemDialog(InventoryType, IndexOfData, EquipSlotIndex);
                if (selectionManager != null)
                    selectionManager.DeselectSelectedUI();
            }
        }

        /// <summary>
        /// Use this function to repair the item
        /// </summary>
        public void OnClickRepairItem()
        {
            if (!GameInstance.Singleton.canRepairItemByPlayer)
                return;

            // Only owning character can repair item
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ClientInventoryHandlers.RequestRepairItem(new RequestRepairItemMessage()
                {
                    inventoryType = InventoryType,
                    index = IndexOfData,
                    equipSlotIndex = EquipSlotIndex,
                }, ClientInventoryActions.ResponseRepairItem);
            }
        }
        #endregion

        #region Set Enhance Socket Item Functions
        /// <summary>
        /// Use this function to set item which you want to enhance (by socket) to `UIEnhanceSocketItem` instance
        /// </summary>
        public void OnClickSetEnhanceSocketItem()
        {
            // Only owning character can enhance item
            if (!IsOwningCharacter() || (InventoryType != InventoryType.NonEquipItems && InventoryType != InventoryType.EquipItems && InventoryType != InventoryType.EquipWeaponRight && InventoryType != InventoryType.EquipWeaponLeft))
                return;

            if (EquipmentItem != null)
            {
                GameInstance.ItemUIVisibilityManager.ShowEnhanceSocketItemDialog(InventoryType, IndexOfData, EquipSlotIndex);
                if (selectionManager != null)
                    selectionManager.DeselectSelectedUI();
            }
        }
        #endregion

        #region Vending Functions
        public void OnClickAddVendingItem()
        {
            // Only unequipped equipment can be offered
            if (!IsOwningCharacter() || InventoryType != InventoryType.NonEquipItems)
                return;

            if (CharacterItem.amount == 1)
            {
                OnPutVendingItemAmountConfirmed(1);
            }
            else
            {
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_PUT_VENDING_ITEM_AMOUNT.ToString()), LanguageManager.GetText(UITextKeys.UI_PUT_VENDING_ITEM_AMOUNT_DESCRIPTION.ToString()), OnPutVendingItemAmountConfirmed, 1, CharacterItem.amount, CharacterItem.amount);
            }
        }

        private void OnPutVendingItemAmountConfirmed(int amount)
        {
            if (selectionManager != null)
                selectionManager.DeselectSelectedUI();
            UIStartVending ui = FindFirstObjectByType<UIStartVending>();
            ui.AddItem(CharacterItem.id, amount, 0);
        }

        public void OnClickRemoveVendingItem()
        {
            UIStartVending ui = FindFirstObjectByType<UIStartVending>();
            ui.RemoveItem(IndexOfData);
        }

        public void OnClickBuyVendingItem()
        {
            GameInstance.PlayingCharacterEntity.VendingComponent.CallCmdBuyItem(IndexOfData);
        }
        #endregion
    }
}
