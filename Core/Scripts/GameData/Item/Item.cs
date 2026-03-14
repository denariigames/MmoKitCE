using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.LEGACY_ITEM_FILE, menuName = GameDataMenuConsts.LEGACY_ITEM_MENU, order = GameDataMenuConsts.LEGACY_ITEM_ORDER)]
    public partial class Item : BaseItem,
        IAmmoItem, IArmorItem, IShieldItem, IWeaponItem,
        IPotionItem, IBuildingItem, IPetItem, IMountItem, ISkillItem,
        ISocketEnhancerItem, IItemWithAttributeData
    {
        public enum LegacyItemType : byte
        {
            Junk,
            Armor,
            Weapon,
            Shield,
            Potion,
            Ammo,
            Building,
            Pet,
            SocketEnhancer,
            Mount,
            AttributeIncrease,
            AttributeReset,
            Skill,
            SkillLearn,
            SkillReset,
        }

        [Category("Item Settings")]
        public LegacyItemType itemType;
        public float useItemCooldown;
        public string autoUseKey;

        [Category("In-Scene Objects/Appearance")]
        public EquipmentModel[] equipmentModels;
        [Tooltip("This will be available with `Weapon` item, set it in case that it will be equipped at left hand")]
        [FormerlySerializedAs("subEquipmentModels")]
        public EquipmentModel[] offHandEquipmentModels;
        public EquipmentModel[] sheathModels;
        public EquipmentModel[] offHandSheathModels;

        [Category(2, "Item Requirement Settings")]
        public ItemRequirement requirement = new ItemRequirement();
        [Category(3, "Equipment Settings")]
        [Header("Generic Equipment Settings")]
        public EquipmentSet equipmentSet;
        [Tooltip("Equipment durability, If this set to 0 it will not broken")]
        [Range(0f, 1000f)]
        public float maxDurability;
        [Tooltip("If this is TRUE, your equipment will be destroyed when durability = 0")]
        public bool destroyIfBroken;
        [Tooltip("Its length is max amount of enhancement sockets")]
        public SocketEnhancerType[] availableSocketEnhancerTypes = new SocketEnhancerType[0];
        [System.Obsolete("Deprecated, will be removed later")]
        [HideInInspector]
        public byte maxSocket;

        [Header("Armor/Shield Settings")]
        public ArmorType armorType;
        public ArmorIncremental armorAmount;

        [Header("Weapon Settings")]
        public WeaponType weaponType;
        public bool doRecoilingAsAttackAnimation = false;
        public DamageIncremental damageAmount;
        public IncrementalMinMaxFloat harvestDamageAmount;
        [Range(0f, 1f)]
        [Tooltip("This will be multiplied with character's movement speed while equipped this weapon")]
        public float moveSpeedRateWhileEquipped = 1f;
        [Range(0f, 1f)]
        [Tooltip("This is move speed rate while reloading this weapon")]
        public float moveSpeedRateWhileReloading = 1f;
        [Range(0f, 1f)]
        [Tooltip("This is move speed rate while charging this weapon")]
        public float moveSpeedRateWhileCharging = 1f;
        [Range(0f, 1f)]
        [Tooltip("This is move speed rate while attacking with this weapon")]
        public float moveSpeedRateWhileAttacking = 0f;
        public MovementRestriction movementRestrictionWhileReloading = default;
        public MovementRestriction movementRestrictionWhileCharging = default;
        public MovementRestriction movementRestrictionWhileAttacking = default;
        public ActionRestriction attackRestriction = default;
        public ActionRestriction reloadRestriction = default;
        public BaseItem[] ammoItems = new BaseItem[0];
        [Tooltip("For macine gun may set this to 30 as magazine capacity, if this is 0 it will not need to have ammo loaded to shoot but still need ammo in inventory")]
        public int ammoCapacity = 0;
        [Tooltip("If this is `TRUE`, ammo capacity will not overrided by ammo's setting")]
        public bool noAmmoCapacityOverriding = false;
        [Tooltip("If this is `TRUE`, ammo data ID will not being changed by ammo item's data ID")]
        public bool noAmmoDataIdChange = false;
        public BaseWeaponAbility weaponAbility;
        public BaseWeaponAbility[] weaponAbilities = new BaseWeaponAbility[0];
        public CrosshairSetting crosshairSetting = new CrosshairSetting()
        {
            expandPerFrame = 3f,
            shrinkPerFrame = 8f,
            minSpread = 10f,
            maxSpread = 50f,
            addSpreadWhileAttackAndMoving = 20f,
        };

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [HideInInspector]
        public AudioClip launchClip;
#endif
        public AudioClipWithVolumeSettings[] launchClipSettings = new AudioClipWithVolumeSettings[0];

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [HideInInspector]
        public AudioClip reloadClip;
#endif
        public AudioClipWithVolumeSettings[] reloadClipSettings = new AudioClipWithVolumeSettings[0];

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [HideInInspector]
        public AudioClip reloadedClip;
#endif
        public AudioClipWithVolumeSettings[] reloadedClipSettings = new AudioClipWithVolumeSettings[0];

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [HideInInspector]
        public AudioClip emptyClip;
#endif
        public AudioClipWithVolumeSettings[] emptyClipSettings = new AudioClipWithVolumeSettings[0];

        [Header("Fire Configs")]
        public FireType fireType;
        public float rateOfFire;
        public float reloadDuration;
        public int maxAmmoEachReload;
        [FormerlySerializedAs("fireStagger")]
        public Vector2 fireSpreadRange;
        public Vector2 fireSpreadRangeWhileFpsViewMode = Vector2.one * float.MinValue;
        public Vector2 fireSpreadRangeWhileShoulderViewMode = Vector2.one * float.MinValue;
        public Vector2 fireSpreadRangeWhileAiming = Vector2.one * float.MinValue;
        [FormerlySerializedAs("fireSpread")]
        public byte fireSpreadAmount;
        public float recoil = 1f;
        public float recoilWhileFpsViewMode = float.MinValue;
        public float recoilWhileShoulderViewMode = float.MinValue;
        public float recoilWhileAiming = float.MinValue;
        public float chargeDuration;
        public bool destroyImmediatelyAfterFired;

        [Category(4, "Buff/Bonus Settings")]
        public Buff buff = new Buff();
        public CharacterStatsIncremental increaseStats;
        public CharacterStatsIncremental increaseStatsRate;
        [ArrayElementTitle("attribute")]
        public AttributeIncremental[] increaseAttributes;
        [ArrayElementTitle("attribute")]
        public AttributeIncremental[] increaseAttributesRate;
        [ArrayElementTitle("damageElement")]
        public ResistanceIncremental[] increaseResistances;
        [ArrayElementTitle("damageElement")]
        public ArmorIncremental[] increaseArmors;
        [ArrayElementTitle("damageElement")]
        public ArmorIncremental[] increaseArmorsRate;
        [ArrayElementTitle("damageElement")]
        public DamageIncremental[] increaseDamages;
        [ArrayElementTitle("damageElement")]
        public DamageIncremental[] increaseDamagesRate;
        // TODO: This is deprecated
        [HideInInspector]
        [ArrayElementTitle("skill")]
        public SkillLevel[] increaseSkillLevels;
        public SkillIncremental[] increaseSkills;
        [ArrayElementTitle("statusEffect")]
        private StatusEffectResistanceIncremental[] increaseStatusEffectResistances;

        [Category(2, "Ammo Settings")]
        public AmmoType ammoType;

        [Category(2, "Building Settings")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableBuildingEntity))]
#endif
        public BuildingEntity buildingEntity;
#endif
#if !DISABLE_ADDRESSABLES
        public AssetReferenceBuildingEntity addressableBuildingEntity;
#endif

        [Category(2, "Pet Settings")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressablePetEntity))]
#endif
        public BaseMonsterCharacterEntity petEntity;
#endif
#if !DISABLE_ADDRESSABLES
        public AssetReferenceBaseMonsterCharacterEntity addressablePetEntity;
#endif
        public IncrementalFloat summonDuration;
        public bool noSummonDuration = true;

        [Category(2, "Mount Settings")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableMountEntity))]
#endif
        public VehicleEntity mountEntity;
#endif
#if !DISABLE_ADDRESSABLES
        public AssetReferenceVehicleEntity addressableMountEntity;
#endif
        public IncrementalFloat mountDuration;
        public bool noMountDuration = true;

        [Category(2, "Socket Enhancer Settings")]
        public SocketEnhancerType socketEnhancerType = SocketEnhancerType.Type1;

        [Category("Buff/Bonus Settings")]
        // For socket enhancer items
        public EquipmentBonus socketEnhanceEffect;

        // For attribute increase/reset items
        public AttributeAmount attributeAmount;

        // For skill learn/reset items
        public SkillLevel skillLevel;

        [ArrayElementTitle("statusEffect")]
        public StatusEffectApplying[] selfStatusEffectsWhenAttacking;
        [ArrayElementTitle("statusEffect")]
        public StatusEffectApplying[] enemyStatusEffectsWhenAttacking;
        [ArrayElementTitle("statusEffect")]
        public StatusEffectApplying[] selfStatusEffectsWhenAttacked;
        [ArrayElementTitle("statusEffect")]
        public StatusEffectApplying[] enemyStatusEffectsWhenAttacked;

        // For equipment items
        public ItemRandomBonus randomBonus = new ItemRandomBonus();

        public override ItemType ItemType
        {
            get
            {
                switch (itemType)
                {
                    case LegacyItemType.Junk:
                        return ItemType.Junk;
                    case LegacyItemType.Armor:
                        return ItemType.Armor;
                    case LegacyItemType.Weapon:
                        return ItemType.Weapon;
                    case LegacyItemType.Shield:
                        return ItemType.Shield;
                    case LegacyItemType.Potion:
                    case LegacyItemType.AttributeIncrease:
                    case LegacyItemType.AttributeReset:
                    case LegacyItemType.SkillLearn:
                    case LegacyItemType.SkillReset:
                        return ItemType.Potion;
                    case LegacyItemType.Ammo:
                        return ItemType.Ammo;
                    case LegacyItemType.Building:
                        return ItemType.Building;
                    case LegacyItemType.Pet:
                        return ItemType.Pet;
                    case LegacyItemType.SocketEnhancer:
                        return ItemType.SocketEnhancer;
                    case LegacyItemType.Mount:
                        return ItemType.Mount;
                    case LegacyItemType.Skill:
                        return ItemType.Skill;
                    default:
                        return ItemType.Junk;
                }
            }
        }

        public override string TypeTitle
        {
            get
            {
                switch (itemType)
                {
                    case LegacyItemType.Junk:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_JUNK.ToString());
                    case LegacyItemType.Armor:
                        return ArmorType.Title;
                    case LegacyItemType.Weapon:
                        return WeaponType.Title;
                    case LegacyItemType.Shield:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_SHIELD.ToString());
                    case LegacyItemType.Potion:
                    case LegacyItemType.AttributeIncrease:
                    case LegacyItemType.AttributeReset:
                    case LegacyItemType.SkillLearn:
                    case LegacyItemType.SkillReset:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_POTION.ToString());
                    case LegacyItemType.Ammo:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_AMMO.ToString());
                    case LegacyItemType.Building:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_BUILDING.ToString());
                    case LegacyItemType.Pet:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_PET.ToString());
                    case LegacyItemType.SocketEnhancer:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_SOCKET_ENHANCER.ToString());
                    case LegacyItemType.Mount:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_MOUNT.ToString());
                    case LegacyItemType.Skill:
                        return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_SKILL.ToString());
                    default:
                        return LanguageManager.GetUnknowTitle();
                }
            }
        }

        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }

        public string AutoUseKey
        {
            get { return autoUseKey; }
        }

        #region Implement IAmmoItem
        public AmmoType AmmoType
        {
            get { return ammoType; }
        }

#if UNITY_EDITOR || !UNITY_SERVER
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableAmmoModel))]
#endif
        protected GameObject ammoModel;
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceGameObject addressableAmmoModel = null;
#endif
#endif

#if UNITY_EDITOR || !UNITY_SERVER
        public UniTask<GameObject> GetAmmoAttachModel()
        {
            GameObject equipModel = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            equipModel = ammoModel;
#endif
#if !DISABLE_ADDRESSABLES
            return addressableAmmoModel.GetOrLoadAssetAsyncOrUsePrefab(equipModel);
#else
            return UniTask.FromResult(equipModel);
#endif
        }
#endif
#endregion

            #region Implement IEquipmentItem
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

        public EquipmentSet EquipmentSet
        {
            get { return equipmentSet; }
        }

        public float MaxDurability
        {
            get { return maxDurability; }
        }

        public bool DestroyIfBroken
        {
            get { return destroyIfBroken; }
        }

        public SocketEnhancerType[] AvailableSocketEnhancerTypes
        {
            get { return availableSocketEnhancerTypes; }
        }

        public EquipmentModel[] EquipmentModels
        {
            get { return equipmentModels; }
        }

        public CharacterStatsIncremental IncreaseStats
        {
            get { return increaseStats; }
        }

        public CharacterStatsIncremental IncreaseStatsRate
        {
            get { return increaseStatsRate; }
        }

        public AttributeIncremental[] IncreaseAttributes
        {
            get { return increaseAttributes; }
        }

        public AttributeIncremental[] IncreaseAttributesRate
        {
            get { return increaseAttributesRate; }
        }

        public ResistanceIncremental[] IncreaseResistances
        {
            get { return increaseResistances; }
        }

        public ArmorIncremental[] IncreaseArmors
        {
            get { return increaseArmors; }
        }

        public ArmorIncremental[] IncreaseArmorsRate
        {
            get { return increaseArmorsRate; }
        }

        public DamageIncremental[] IncreaseDamages
        {
            get { return increaseDamages; }
        }

        public DamageIncremental[] IncreaseDamagesRate
        {
            get { return increaseDamagesRate; }
        }

        public SkillIncremental[] IncreaseSkills
        {
            get { return increaseSkills; }
        }

        public StatusEffectResistanceIncremental[] IncreaseStatusEffectResistances
        {
            get { return increaseStatusEffectResistances; }
        }

        public StatusEffectApplying[] SelfStatusEffectsWhenAttacking
        {
            get { return selfStatusEffectsWhenAttacking; }
        }

        public StatusEffectApplying[] EnemyStatusEffectsWhenAttacking
        {
            get { return enemyStatusEffectsWhenAttacking; }
        }

        public StatusEffectApplying[] SelfStatusEffectsWhenAttacked
        {
            get { return selfStatusEffectsWhenAttacked; }
        }

        public StatusEffectApplying[] EnemyStatusEffectsWhenAttacked
        {
            get { return enemyStatusEffectsWhenAttacked; }
        }

        public ItemRandomBonus RandomBonus
        {
            get { return randomBonus; }
        }
        #endregion

        #region Implement IDefendEquipmentItem
        public ArmorIncremental ArmorAmount
        {
            get { return armorAmount; }
        }
        #endregion

        #region Implement IArmorItem
        public ArmorType ArmorType
        {
            get
            {
                if (armorType == null && GameInstance.Singleton != null)
                    armorType = GameInstance.Singleton.DefaultArmorType;
                return armorType;
            }
        }
        #endregion

        #region Implement IWeaponItem
        public WeaponType WeaponType
        {
            get
            {
                if (weaponType == null && GameInstance.Singleton != null)
                    weaponType = GameInstance.Singleton.DefaultWeaponType;
                return weaponType;
            }
        }

        public bool DoRecoilingAsAttackAnimation
        {
            get { return doRecoilingAsAttackAnimation; }
            set { doRecoilingAsAttackAnimation = value; }
        }

        public EquipmentModel[] OffHandEquipmentModels
        {
            get { return offHandEquipmentModels; }
        }

        public EquipmentModel[] SheathModels
        {
            get { return sheathModels; }
            set { sheathModels = value; }
        }

        public EquipmentModel[] OffHandSheathModels
        {
            get { return offHandSheathModels; }
            set { offHandSheathModels = value; }
        }

        public DamageIncremental DamageAmount
        {
            get { return damageAmount; }
        }

        public IncrementalMinMaxFloat HarvestDamageAmount
        {
            get { return harvestDamageAmount; }
        }

        public float MoveSpeedRateWhileEquipped
        {
            get { return moveSpeedRateWhileEquipped; }
        }

        public float MoveSpeedRateWhileReloading
        {
            get { return moveSpeedRateWhileReloading; }
        }

        public float MoveSpeedRateWhileCharging
        {
            get { return moveSpeedRateWhileCharging; }
        }

        public float MoveSpeedRateWhileAttacking
        {
            get { return moveSpeedRateWhileAttacking; }
        }

        public MovementRestriction MovementRestrictionWhileReloading
        {
            get { return movementRestrictionWhileReloading; }
        }

        public MovementRestriction MovementRestrictionWhileCharging
        {
            get { return movementRestrictionWhileCharging; }
        }

        public MovementRestriction MovementRestrictionWhileAttacking
        {
            get { return movementRestrictionWhileAttacking; }
        }

        public ActionRestriction AttackRestriction
        {
            get { return attackRestriction; }
        }

        public ActionRestriction ReloadRestriction
        {
            get { return reloadRestriction; }
        }

        [System.NonSerialized]
        private HashSet<int> _cacheAmmoItemIds = null;
        public HashSet<int> AmmoItemIds
        {
            get
            {
                if (_cacheAmmoItemIds == null)
                {
                    _cacheAmmoItemIds = new HashSet<int>();
                    for (int i = 0; i < ammoItems.Length; ++i)
                    {
                        if (ammoItems[i] == null)
                            continue;
                        _cacheAmmoItemIds.Add(ammoItems[i].DataId);
                    }
                }
                return _cacheAmmoItemIds;
            }
        }

        public int AmmoCapacity
        {
            get { return ammoCapacity; }
        }

        public bool NoAmmoCapacityOverriding
        {
            get { return noAmmoCapacityOverriding; }
        }

        public bool NoAmmoDataIdChange
        {
            get { return noAmmoDataIdChange; }
        }

        public BaseWeaponAbility[] WeaponAbilities
        {
            get { return weaponAbilities; }
        }

        public CrosshairSetting CrosshairSetting
        {
            get { return crosshairSetting; }
        }

        public AudioClipWithVolumeSettings LaunchClip
        {
            get
            {
                if (launchClipSettings != null && launchClipSettings.Length > 0)
                    return launchClipSettings[Random.Range(0, launchClipSettings.Length - 1)];
                return null;
            }
        }

        public AudioClipWithVolumeSettings ReloadClip
        {
            get
            {
                if (reloadClipSettings != null && reloadClipSettings.Length > 0)
                    return reloadClipSettings[Random.Range(0, reloadClipSettings.Length - 1)];
                return null;
            }
        }

        public AudioClipWithVolumeSettings ReloadedClip
        {
            get
            {
                if (reloadedClipSettings != null && reloadedClipSettings.Length > 0)
                    return reloadedClipSettings[Random.Range(0, reloadedClipSettings.Length - 1)];
                return null;
            }
        }

        public AudioClipWithVolumeSettings EmptyClip
        {
            get
            {
                if (emptyClipSettings != null && emptyClipSettings.Length > 0)
                    return emptyClipSettings[Random.Range(0, emptyClipSettings.Length - 1)];
                return null;
            }
        }

        public FireType FireType
        {
            get { return fireType; }
        }

        public float RateOfFire
        {
            get { return rateOfFire; }
        }

        public float ReloadDuration
        {
            get { return reloadDuration; }
        }

        public int MaxAmmoEachReload
        {
            get { return maxAmmoEachReload; }
        }

        public Vector2 FireSpreadRange
        {
            get { return fireSpreadRange; }
        }

        public Vector2 FireSpreadRangeWhileFpsViewMode
        {
            get { return fireSpreadRangeWhileFpsViewMode; }
        }

        public Vector2 FireSpreadRangeWhileShoulderViewMode
        {
            get { return fireSpreadRangeWhileShoulderViewMode; }
        }

        public Vector2 FireSpreadRangeWhileAiming
        {
            get { return fireSpreadRangeWhileAiming; }
        }

        public byte FireSpreadAmount
        {
            get { return fireSpreadAmount; }
        }

        public float Recoil
        {
            get { return recoil; }
        }

        public float RecoilWhileFpsViewMode
        {
            get { return recoilWhileFpsViewMode; }
        }

        public float RecoilWhileShoulderViewMode
        {
            get { return recoilWhileShoulderViewMode; }
        }

        public float RecoilWhileAiming
        {
            get { return recoilWhileAiming; }
        }

        public float ChargeDuration
        {
            get { return chargeDuration; }
        }

        public bool DestroyImmediatelyAfterFired
        {
            get { return destroyImmediatelyAfterFired; }
        }
        #endregion

        #region Implement IPotionItem, IBuildingItem, IPetItem, IMountItem, ISkillItem
        public Buff BuffData
        {
            get
            {
                if (itemType == LegacyItemType.Potion)
                    return buff;
                return null;
            }
        }

        public BuildingEntity BuildingEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if (itemType == LegacyItemType.Building)
                    return buildingEntity;
#endif
                return null;
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceBuildingEntity AddressableBuildingEntity
        {
            get
            {
                if (itemType == LegacyItemType.Building)
                    return addressableBuildingEntity;
                return null;
            }
        }
#endif

        public BaseMonsterCharacterEntity MonsterCharacterEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if (itemType == LegacyItemType.Pet)
                    return petEntity;
#endif
                return null;
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceBaseMonsterCharacterEntity AddressableMonsterCharacterEntity
        {
            get
            {
                if (itemType == LegacyItemType.Pet)
                    return addressablePetEntity;
                return null;
            }
        }
#endif

        public IncrementalFloat SummonDuration
        {
            get { return summonDuration; }
        }

        public bool NoSummonDuration
        {
            get { return noSummonDuration; }
        }

        public VehicleEntity VehicleEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if (itemType == LegacyItemType.Mount)
                    return mountEntity;
#endif
                return null;
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceVehicleEntity AddressableVehicleEntity
        {
            get
            {
                if (itemType == LegacyItemType.Mount)
                    return addressableMountEntity;
                return null;
            }
        }
#endif

        public IncrementalFloat MountDuration
        {
            get { return mountDuration; }
        }

        public bool NoMountDuration
        {
            get { return noMountDuration; }
        }

        public BaseSkill SkillData
        {
            get
            {
                if (itemType == LegacyItemType.Skill || itemType == LegacyItemType.SkillLearn)
                    return skillLevel.skill;
                return null;
            }
        }

        public int SkillLevel
        {
            get
            {
                if (itemType == LegacyItemType.Skill || itemType == LegacyItemType.SkillLearn)
                    return skillLevel.level;
                return 0;
            }
        }

        public Attribute AttributeData
        {
            get
            {
                if (itemType == LegacyItemType.AttributeIncrease)
                    return attributeAmount.attribute;
                return null;
            }
        }

        public float AttributeAmount
        {
            get
            {
                if (itemType == LegacyItemType.AttributeIncrease)
                    return attributeAmount.amount;
                return 0;
            }
        }
#endregion

        #region Implement ISocketEnhancerItem
        public SocketEnhancerType SocketEnhancerType
        {
            get { return socketEnhancerType; }
        }

        public EquipmentBonus SocketEnhanceEffect
        {
            get { return socketEnhanceEffect; }
        }

#if UNITY_EDITOR || !UNITY_SERVER
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableSocketEnhancerModel))]
#endif
        protected GameObject socketEnhancerModel;
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceGameObject addressableSocketEnhancerModel = null;
#endif
#endif

#if UNITY_EDITOR || !UNITY_SERVER
        public UniTask<GameObject> GetSocketEnhancerAttachModel()
        {
            GameObject equipModel = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            equipModel = socketEnhancerModel;
#endif
#if !DISABLE_ADDRESSABLES
            return addressableSocketEnhancerModel.GetOrLoadAssetAsyncOrUsePrefab(equipModel);
#else
            return UniTask.FromResult(equipModel);
#endif
        }
#endif
#endregion

        public override bool Validate()
        {
            bool hasChanges = false;
            if (weaponAbility != null && (weaponAbilities == null || weaponAbilities.Length == 0))
            {
                weaponAbilities = new List<BaseWeaponAbility>()
                {
                    weaponAbility,
                }.ToArray();
                hasChanges = true;
            }
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            if (MigrateAudioClips(ref launchClip, ref launchClipSettings))
                hasChanges = true;
            if (MigrateAudioClips(ref reloadClip, ref reloadClipSettings))
                hasChanges = true;
            if (MigrateAudioClips(ref reloadedClip, ref reloadedClipSettings))
                hasChanges = true;
            if (MigrateAudioClips(ref emptyClip, ref emptyClipSettings))
                hasChanges = true;
#endif
            if (increaseSkillLevels != null && increaseSkillLevels.Length > 0)
            {
                List<SkillIncremental> skills = new List<SkillIncremental>();
                foreach (SkillLevel increaseSkillLevel in increaseSkillLevels)
                {
                    if (increaseSkillLevel.skill == null)
                        continue;
                    skills.Add(new SkillIncremental()
                    {
                        skill = increaseSkillLevel.skill,
                        level = new IncrementalInt()
                        {
                            baseAmount = increaseSkillLevel.level
                        },
                    });
                }
                increaseSkills = skills.ToArray();
                increaseSkillLevels = null;
                hasChanges = true;
            }
#pragma warning disable CS0618 // Type or member is obsolete
            if (maxSocket > 0)
            {
                availableSocketEnhancerTypes = new SocketEnhancerType[maxSocket];
                for (byte i = 0; i < maxSocket; ++i)
                {
                    availableSocketEnhancerTypes[i] = SocketEnhancerType.Type1;
                }
                maxSocket = 0;
                hasChanges = true;
            }
#pragma warning restore CS0618 // Type or member is obsolete
            return hasChanges || base.Validate();
        }

        private bool MigrateAudioClips(ref AudioClip singleClip, ref AudioClipWithVolumeSettings[] destinationSettings)
        {
            if (singleClip == null)
                return false;

            bool hasChanges = false;

            List<AudioClip> clips = new List<AudioClip>();
            if (singleClip != null && !clips.Contains(singleClip))
            {
                clips.Add(singleClip);
                singleClip = null;
                hasChanges = true;
            }
            if (!hasChanges)
                return false;

            List<AudioClipWithVolumeSettings> clipSettings = new List<AudioClipWithVolumeSettings>();
            if (destinationSettings != null && destinationSettings.Length > 0)
                clipSettings.AddRange(destinationSettings);
            for (int i = 0; i < clips.Count; ++i)
            {
                clipSettings.Add(new AudioClipWithVolumeSettings()
                {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    audioClip = clips[i],
#endif
                    minRandomVolume = 1f,
                    maxRandomVolume = 1f,
                });
            }
            destinationSettings = clipSettings.ToArray();
            return true;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddBuildingEntities(buildingEntity);
            GameInstance.AddMonsterCharacterEntities(petEntity);
            GameInstance.AddVehicleEntities(mountEntity);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceBuildingEntities(addressableBuildingEntity);
            GameInstance.AddAssetReferenceMonsterCharacterEntities(addressablePetEntity);
            GameInstance.AddAssetReferenceVehicleEntities(addressableMountEntity);
#endif
            GameInstance.AddAttributes(increaseAttributes);
            GameInstance.AddAttributes(increaseAttributesRate);
            GameInstance.AddDamageElements(increaseResistances);
            GameInstance.AddDamageElements(increaseArmors);
            GameInstance.AddDamageElements(increaseArmorsRate);
            GameInstance.AddDamageElements(increaseDamages);
            GameInstance.AddDamageElements(increaseDamagesRate);
            GameInstance.AddDamageElements(damageAmount);
            GameInstance.AddSkills(increaseSkills);
            GameInstance.AddSkills(skillLevel);
            GameInstance.AddStatusEffects(increaseStatusEffectResistances);
            GameInstance.AddStatusEffects(selfStatusEffectsWhenAttacking);
            GameInstance.AddStatusEffects(enemyStatusEffectsWhenAttacking);
            GameInstance.AddStatusEffects(selfStatusEffectsWhenAttacked);
            GameInstance.AddStatusEffects(enemyStatusEffectsWhenAttacked);
            GameInstance.AddEquipmentSets(equipmentSet);
            GameInstance.AddArmorTypes(armorType);
            GameInstance.AddWeaponTypes(weaponType);
            GameInstance.AddAmmoTypes(ammoType);
            randomBonus.PrepareRelatesData();
            buff.PrepareRelatesData();
            // Data migration
            GameInstance.MigrateEquipmentEntities(equipmentModels);
            GameInstance.MigrateEquipmentEntities(offHandEquipmentModels);
        }

        public Item GenerateDefaultItem(WeaponType type)
        {
            name = GameDataConst.DEFAULT_WEAPON_ID;
            defaultTitle = GameDataConst.DEFAULT_WEAPON_TITLE;
            itemType = LegacyItemType.Weapon;
            weaponType = type;
            // Default damage amount
            IncrementalMinMaxFloat damageAmountMinMax = new IncrementalMinMaxFloat();
            damageAmountMinMax.baseAmount = new MinMaxFloat() { min = 1, max = 1 };
            damageAmountMinMax.amountIncreaseEachLevel = new MinMaxFloat() { min = 0, max = 0 };
            damageAmount = new DamageIncremental()
            {
                amount = damageAmountMinMax,
            };
            // Default harvest damage amount
            harvestDamageAmount = new IncrementalMinMaxFloat()
            {
                baseAmount = new MinMaxFloat() { min = 1, max = 1 },
                amountIncreaseEachLevel = new MinMaxFloat() { min = 0, max = 0 }
            };
            return this;
        }

        public bool UseItem(BaseCharacterEntity character, int itemIndex, CharacterItem characterItem)
        {
            switch (itemType)
            {
                case LegacyItemType.Potion:
                    return UseItemPotion(character, itemIndex, characterItem);
                case LegacyItemType.Pet:
                    return UseItemPet(character, itemIndex, characterItem);
                case LegacyItemType.Mount:
                    return UseItemMount(character, characterItem);
                case LegacyItemType.AttributeIncrease:
                    return UseItemAttributeIncrease(character as BasePlayerCharacterEntity, itemIndex);
                case LegacyItemType.AttributeReset:
                    return UseItemAttributeReset(character as BasePlayerCharacterEntity, itemIndex);
                case LegacyItemType.SkillLearn:
                    return UseItemSkillLearn(character as BasePlayerCharacterEntity, itemIndex);
                case LegacyItemType.SkillReset:
                    return UseItemSkillReset(character as BasePlayerCharacterEntity, itemIndex);
            }
            return true;
        }

        protected bool UseItemPotion(BaseCharacterEntity character, int itemIndex, CharacterItem characterItem)
        {
            if (!character.CanUseItem() || !character.DecreaseItemsByIndex(itemIndex, 1, false))
                return false;
            character.FillEmptySlots();
            character.ApplyBuff(DataId, BuffType.PotionBuff, characterItem.level, character.GetInfo(), CharacterItem.Empty);
            return true;
        }

        protected bool UseItemPet(BaseCharacterEntity character, int itemIndex, CharacterItem characterItem)
        {
            if (!character.CanUseItem())
                return false;
            // Clear all summoned pets
            bool doNotSummonNewOne = false;
            CharacterSummon tempSummon;
            for (int i = character.Summons.Count - 1; i >= 0; --i)
            {
                tempSummon = character.Summons[i];
                if (tempSummon.type != SummonType.PetItem)
                    continue;
                character.Summons.RemoveAt(i);
                tempSummon.UnSummon(character);
                if (!doNotSummonNewOne && string.Equals(characterItem.id, tempSummon.sourceId))
                    doNotSummonNewOne = true;
            }
            if (doNotSummonNewOne)
                return true;
            // Summon new pet
            CharacterSummon newSummon = CharacterSummon.Create(SummonType.PetItem, characterItem.id, DataId);
            newSummon.Summon(character, characterItem.level, 0f, characterItem.exp);
            character.Summons.Add(newSummon);
            return true;
        }

        protected bool UseItemMount(BaseCharacterEntity character, CharacterItem characterItem)
        {
            if (!character.CanUseItem())
                return false;
            character.SpawnMount(MountType.MountItem, characterItem.id, MountDuration.GetAmount(characterItem.level));
            return true;
        }

        protected bool UseItemAttributeIncrease(BasePlayerCharacterEntity character, int itemIndex)
        {
            if (!character.CanUseItem() || attributeAmount.attribute == null)
                return false;

            UITextKeys gameMessage;
            if (!character.AddAttribute(out gameMessage, attributeAmount.attribute.DataId, (int)attributeAmount.amount, itemIndex))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(character.ConnectionId, gameMessage);
                return false;
            }

            return true;
        }

        protected bool UseItemAttributeReset(BasePlayerCharacterEntity character, int itemIndex)
        {
            if (!character.CanUseItem())
                return false;

            return character.ResetAttributes(itemIndex);
        }

        protected bool UseItemSkillLearn(BasePlayerCharacterEntity character, int itemIndex)
        {
            if (!character.CanUseItem() || SkillData == null)
                return false;

            UITextKeys gameMessage;
            if (!character.AddSkill(out gameMessage, SkillData.DataId, SkillLevel, itemIndex))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(character.ConnectionId, gameMessage);
                return false;
            }

            return true;
        }

        protected bool UseItemSkillReset(BasePlayerCharacterEntity character, int itemIndex)
        {
            if (!character.CanUseItem())
                return false;

            return character.ResetSkills(itemIndex);
        }

        public bool HasCustomAimControls()
        {
            switch (itemType)
            {
                case LegacyItemType.Potion:
                    return false;
                case LegacyItemType.Building:
                    return true;
                case LegacyItemType.Pet:
                    return false;
                case LegacyItemType.Mount:
                    return false;
                case LegacyItemType.Skill:
                    return skillLevel.skill.HasCustomAimControls();
            }
            return false;
        }

        public AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            switch (itemType)
            {
                case LegacyItemType.Potion:
                    return default;
                case LegacyItemType.Building:
#if !DISABLE_ADDRESSABLES
                    BuildingEntity tempBuildingEntity = AddressableBuildingEntity.GetOrLoadAssetOrUsePrefab(BuildingEntity);
#else
                    BuildingEntity tempBuildingEntity = BuildingEntity;
#endif
                    if (tempBuildingEntity != null)
                        return BasePlayerCharacterController.Singleton.BuildAimController.UpdateAimControls(aimAxes, tempBuildingEntity);
                    return default;
                case LegacyItemType.Pet:
                    return default;
                case LegacyItemType.Mount:
                    return default;
                case LegacyItemType.Skill:
                    return SkillData.UpdateAimControls(aimAxes, SkillLevel);
            }
            return default;
        }

        public void FinishAimControls(bool isCancel)
        {
            switch (itemType)
            {
                case LegacyItemType.Potion:
                    break;
                case LegacyItemType.Building:
                    BasePlayerCharacterController.Singleton.BuildAimController.FinishAimControls(isCancel);
                    break;
                case LegacyItemType.Pet:
                    break;
                case LegacyItemType.Mount:
                    break;
                case LegacyItemType.Skill:
                    skillLevel.skill.FinishAimControls(isCancel);
                    break;
            }
        }

        public bool IsChanneledAbility()
        {
            switch (itemType)
            {
                case LegacyItemType.Potion:
                    return false;
                case LegacyItemType.Building:
                    return false;
                case LegacyItemType.Pet:
                    return false;
                case LegacyItemType.Mount:
                    return false;
                case LegacyItemType.Skill:
                    return skillLevel.skill.IsChanneledAbility();
            }
            return false;
        }
    }
}
