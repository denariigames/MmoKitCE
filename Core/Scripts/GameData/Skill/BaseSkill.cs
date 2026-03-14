using Cysharp.Text;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public abstract partial class BaseSkill : BaseGameData, ICustomAimController
    {
        [Category("Skill Settings")]
        [Min(1)]
        public int maxLevel = 1;
        public float battlePointScore = 0;
        public bool cannotReset = false;

        [Category(2, "Activation Settings")]
        [Range(0f, 1f)]
        [Tooltip("This is move speed rate while using this skill")]
        public float moveSpeedRateWhileUsingSkill = 0f;
        public MovementRestriction movementRestrictionWhileUsingSkill = new MovementRestriction();
        public ActionRestriction useSkillRestriction = new ActionRestriction();
        public IncrementalInt consumeHp = new IncrementalInt();
        public IncrementalInt consumeMp = new IncrementalInt();
        public IncrementalInt consumeStamina = new IncrementalInt();
        public IncrementalFloat consumeHpRate = new IncrementalFloat();
        public IncrementalFloat consumeMpRate = new IncrementalFloat();
        public IncrementalFloat consumeStaminaRate = new IncrementalFloat();
        public IncrementalFloat coolDownDuration = new IncrementalFloat();

        [Category(2, "Skill Casting")]
        [Header("Casting Effects")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableSkillCastEffects))]
#endif
        private GameEffect[] skillCastEffects = new GameEffect[0];
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceGameEffect[] addressableSkillCastEffects = new AssetReferenceGameEffect[0];
#endif
        public IncrementalFloat castDuration = new IncrementalFloat();
        public bool canBeInterruptedWhileCasting;

        [Header("Casted Effects")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableSkillActivateEffects))]
#endif
        private GameEffect[] skillActivateEffects = new GameEffect[0];
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceGameEffect[] addressableSkillActivateEffects = new AssetReferenceGameEffect[0];
#endif

        [Category(11, "Requirement")]
        [Header("Requirements to Levelup (Tools)")]
        public SkillRequirement requirement = new SkillRequirement()
        {
            skillPoint = new IncrementalFloat()
            {
                baseAmount = 1,
            },
        };
#if UNITY_EDITOR
        [InspectorButton(nameof(MakeRequirementEachLevels), "Make Requirement Each Levels")]
        public bool btnMakeRequirementEachLevels;
#endif

        [Header("Requirements to Levelup (Data)")]
        public List<SkillRequirementEntry> requirementEachLevels = new List<SkillRequirementEntry>();

        [Header("Required Equipments")]
        [Tooltip("If this is `TRUE`, character have to equip shield to use skill")]
        public bool requireShield = false;

        [Tooltip("Characters will be able to use skill if this list is empty or equipping one in this list")]
        public WeaponType[] availableWeapons = new WeaponType[0];

        [Tooltip("Characters will be able to use skill if this list is empty or equipping one in this list")]
        public ArmorType[] availableArmors = new ArmorType[0];

        [Header("Required Vehicles")]
        [Tooltip("Characters will be able to use skill if this list is empty or driving one in this list")]
        public VehicleType[] availableVehicles = new VehicleType[0];

        [Header("Requirements to Use")]
        [Tooltip("If this list is empty it won't decrease items from inventory. It will decrease one kind of item in this list when using skill, not all items in this list")]
        public ItemAmount[] requireItems = new ItemAmount[0];
        [Tooltip("If `Require Ammo Type` is `Based On Weapon` it will decrease ammo based on ammo type which set to the weapon, amount to decrease ammo can be set to `Require Ammo Amount`. If weapon has no require ammo, it will not able to use skill. If `Require Ammo Type` is `Based On Skill`, it will decrease ammo based on `Require Ammos` setting")]
        public RequireAmmoType requireAmmoType = RequireAmmoType.None;
        [FormerlySerializedAs("useAmmoAmount")]
        [Tooltip("It will be used while `Require Ammo Type` is `Based On Weapon` to decrease ammo")]
        public int requireAmmoAmount = 0;
        [Tooltip("If this list is empty it won't decrease ammo items from inventory. It will decrease one kind of item in this list when using skill, not all items in this list")]
        public AmmoTypeAmount[] requireAmmos = new AmmoTypeAmount[0];

        public virtual string TypeTitle
        {
            get
            {
                switch (SkillType)
                {
                    case SkillType.Active:
                        return LanguageManager.GetText(UISkillTypeKeys.UI_SKILL_TYPE_ACTIVE.ToString());
                    default:
                        return LanguageManager.GetText(UISkillTypeKeys.UI_SKILL_TYPE_PASSIVE.ToString());
                }
            }
        }

        [System.NonSerialized]
        private HashSet<WeaponType> _cacheAvailableWeapons = null;
        public HashSet<WeaponType> CacheAvailableWeapons
        {
            get
            {
                if (_cacheAvailableWeapons == null)
                {
                    _cacheAvailableWeapons = new HashSet<WeaponType>();
                    if (availableWeapons == null || availableWeapons.Length == 0)
                        return _cacheAvailableWeapons;
                    foreach (WeaponType availableWeapon in availableWeapons)
                    {
                        if (availableWeapon == null) continue;
                        _cacheAvailableWeapons.Add(availableWeapon);
                    }
                }
                return _cacheAvailableWeapons;
            }
        }

        [System.NonSerialized]
        private HashSet<ArmorType> _cacheAvailableArmors = null;
        public HashSet<ArmorType> CacheAvailableArmors
        {
            get
            {
                if (_cacheAvailableArmors == null)
                {
                    _cacheAvailableArmors = new HashSet<ArmorType>();
                    if (availableArmors == null || availableArmors.Length == 0)
                        return _cacheAvailableArmors;
                    foreach (ArmorType requireArmor in availableArmors)
                    {
                        if (requireArmor == null) continue;
                        _cacheAvailableArmors.Add(requireArmor);
                    }
                }
                return _cacheAvailableArmors;
            }
        }

        [System.NonSerialized]
        private HashSet<VehicleType> _cacheAvailableVehicles = null;
        public HashSet<VehicleType> CacheAvailableVehicles
        {
            get
            {
                if (_cacheAvailableVehicles == null)
                {
                    _cacheAvailableVehicles = new HashSet<VehicleType>();
                    if (availableVehicles == null || availableVehicles.Length == 0)
                        return _cacheAvailableVehicles;
                    foreach (VehicleType requireVehicle in availableVehicles)
                    {
                        if (requireVehicle == null) continue;
                        _cacheAvailableVehicles.Add(requireVehicle);
                    }
                }
                return _cacheAvailableVehicles;
            }
        }

        [System.NonSerialized]
        private bool _alreadySetAvailableWeaponsText = false;
        [System.NonSerialized]
        private string _availableWeaponsText = null;
        public string AvailableWeaponsText
        {
            get
            {
                if (!_alreadySetAvailableWeaponsText)
                {
                    using (Utf16ValueStringBuilder str = ZString.CreateStringBuilder(true))
                    {
                        foreach (WeaponType availableWeapon in CacheAvailableWeapons)
                        {
                            if (availableWeapon == null)
                                continue;
                            if (str.Length > 0)
                                str.Append('/');
                            str.Append(availableWeapon.Title);
                        }
                        _availableWeaponsText = str.ToString();
                    }
                    _alreadySetAvailableWeaponsText = true;
                }
                return _availableWeaponsText;
            }
        }

        [System.NonSerialized]
        private bool _alreadySetAvailableArmorsText = false;
        [System.NonSerialized]
        private string _availableArmorsText = null;
        public string AvailableArmorsText
        {
            get
            {
                if (!_alreadySetAvailableArmorsText)
                {
                    using (Utf16ValueStringBuilder str = ZString.CreateStringBuilder(true))
                    {
                        foreach (ArmorType requireArmor in availableArmors)
                        {
                            if (requireArmor == null)
                                continue;
                            if (str.Length > 0)
                                str.Append('/');
                            str.Append(requireArmor.Title);
                        }
                        _availableArmorsText = str.ToString();
                    }
                    _alreadySetAvailableArmorsText = true;
                }
                return _availableArmorsText;
            }
        }

        [System.NonSerialized]
        private bool _alreadySetAvailableVehiclesText = false;
        [System.NonSerialized]
        private string availableVehiclesText = null;
        public string AvailableVehiclesText
        {
            get
            {
                if (!_alreadySetAvailableVehiclesText)
                {
                    using (Utf16ValueStringBuilder str = ZString.CreateStringBuilder(true))
                    {
                        foreach (VehicleType requireVehicle in availableVehicles)
                        {
                            if (requireVehicle == null)
                                continue;
                            if (str.Length > 0)
                                str.Append('/');
                            str.Append(requireVehicle.Title);
                        }
                        availableVehiclesText = str.ToString();
                    }
                    _alreadySetAvailableVehiclesText = true;
                }
                return availableVehiclesText;
            }
        }

        public IHitRegistrationManager HitRegistrationManager { get { return BaseGameNetworkManager.Singleton.HitRegistrationManager; } }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (TryGetBuff(out Buff buff))
            {
                buff.PrepareRelatesData();
            }
            if (TryGetDebuff(out Buff debuff))
            {
                debuff.PrepareRelatesData();
            }
            if (TryGetSummon(out SkillSummon summon))
            {
#if !DISABLE_ADDRESSABLES
                GameInstance.AddAssetReferenceMonsterCharacterEntities(summon.AddressableMonsterCharacterEntity);
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                GameInstance.AddMonsterCharacterEntities(summon.MonsterCharacterEntity);
#endif
            }
            if (TryGetMount(out SkillMount mount))
            {
#if !DISABLE_ADDRESSABLES
                GameInstance.AddAssetReferenceVehicleEntities(mount.AddressableMountEntity);
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                GameInstance.AddVehicleEntities(mount.MountEntity);
#endif
            }
            if (TryGetItemCraft(out ItemCraft itemCraft))
            {
                GameInstance.AddItems(itemCraft.CraftingItem);
                GameInstance.AddItems(itemCraft.RequireItems);
                GameInstance.AddCurrencies(itemCraft.RequireCurrencies);
            }
            if (TryGetAttackStatusEffectApplyings(out StatusEffectApplying[] attackStatusEffectApplyings))
            {
                GameInstance.AddStatusEffects(attackStatusEffectApplyings);
            }
            if (requirementEachLevels.Count <= 0)
            {
                MakeRequirementEachLevels();
            }
        }

        public GameEffect[] SkillCastEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return skillCastEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceGameEffect[] AddressableSkillCastEffects
        {
            get { return addressableSkillCastEffects; }
        }
#endif

        public float GetCastDuration(int level)
        {
            return castDuration.GetAmount(level);
        }

        public GameEffect[] SkillActivateEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return skillActivateEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceGameEffect[] AddressableSkillActivateEffects
        {
            get { return addressableSkillActivateEffects; }
        }
#endif

        public virtual GameEffect[] DamageHitEffects
        {
            get { return null; }
        }

#if !DISABLE_ADDRESSABLES
        public virtual AssetReferenceGameEffect[] AddressableDamageHitEffects
        {
            get { return null; }
        }
#endif

        public int GetConsumeHp(int level)
        {
            return consumeHp.GetAmount(level);
        }

        public int GetConsumeMp(int level)
        {
            return consumeMp.GetAmount(level);
        }

        public int GetConsumeStamina(int level)
        {
            return consumeStamina.GetAmount(level);
        }

        public float GetConsumeHpRate(int level)
        {
            return consumeHpRate.GetAmount(level);
        }

        public float GetConsumeMpRate(int level)
        {
            return consumeMpRate.GetAmount(level);
        }

        public float GetConsumeStaminaRate(int level)
        {
            return consumeStaminaRate.GetAmount(level);
        }

        public int GetTotalConsumeHp(int level, ICharacterData character)
        {
            return (int)(character.GetCaches().MaxHp * GetConsumeHpRate(level)) + GetConsumeHp(level);
        }

        public int GetTotalConsumeMp(int level, ICharacterData character)
        {
            return (int)(character.GetCaches().MaxMp * GetConsumeMpRate(level)) + GetConsumeMp(level);
        }

        public int GetTotalConsumeStamina(int level, ICharacterData character)
        {
            return (int)(character.GetCaches().MaxStamina * GetConsumeStaminaRate(level)) + GetConsumeStamina(level);
        }

        public float GetCoolDownDuration(int level)
        {
            float duration = coolDownDuration.GetAmount(level);
            if (duration < 0f)
                duration = 0f;
            return duration;
        }

        public bool IsDisallowToLevelUp(int level)
        {
            if (level >= requirementEachLevels.Count)
                return requirementEachLevels[requirementEachLevels.Count - 1].disallow;
            return requirementEachLevels[level].disallow;
        }

        public int GetRequireCharacterLevel(int level)
        {
            if (level >= requirementEachLevels.Count)
                return requirementEachLevels[requirementEachLevels.Count - 1].characterLevel;
            return requirementEachLevels[level].characterLevel;
        }

        public float GetRequireCharacterSkillPoint(int level)
        {
            if (level >= requirementEachLevels.Count)
                return requirementEachLevels[requirementEachLevels.Count - 1].skillPoint;
            return requirementEachLevels[level].skillPoint;
        }

        public int GetRequireCharacterGold(int level)
        {
            if (level >= requirementEachLevels.Count)
                return requirementEachLevels[requirementEachLevels.Count - 1].gold;
            return requirementEachLevels[level].gold;
        }

        public void GetRequireAttributeAmounts(int level, Dictionary<Attribute, float> result)
        {
            result.Clear();
            if (level >= requirementEachLevels.Count)
                GameDataHelpers.CombineAttributes(requirementEachLevels[requirementEachLevels.Count - 1].attributeAmounts, result, 1f);
            GameDataHelpers.CombineAttributes(requirementEachLevels[level].attributeAmounts, result, 1f);
        }

        public void GetRequireSkillLevels(int level, Dictionary<BaseSkill, int> result)
        {
            result.Clear();
            if (level >= requirementEachLevels.Count)
                GameDataHelpers.CombineSkills(requirementEachLevels[requirementEachLevels.Count - 1].skillLevels, result, 1f);
            GameDataHelpers.CombineSkills(requirementEachLevels[level].skillLevels, result, 1f);
        }

        public void GetRequireCurrencyAmounts(int level, Dictionary<Currency, int> result)
        {
            result.Clear();
            if (level >= requirementEachLevels.Count)
                GameDataHelpers.CombineCurrencies(requirementEachLevels[requirementEachLevels.Count - 1].currencyAmounts, result, 1f);
            GameDataHelpers.CombineCurrencies(requirementEachLevels[level].currencyAmounts, result, 1f);
        }

        public void GetRequireItemAmounts(int level, Dictionary<BaseItem, int> result)
        {
            result.Clear();
            if (level >= requirementEachLevels.Count)
                GameDataHelpers.CombineItems(requirementEachLevels[requirementEachLevels.Count - 1].itemAmounts, result);
            GameDataHelpers.CombineItems(requirementEachLevels[level].itemAmounts, result);
        }

        public bool IsAvailable(ICharacterData character)
        {
            return character.GetCaches().Skills.TryGetValue(this, out int skillLevel) && skillLevel > 0;
        }

        public abstract SkillType SkillType { get; }
        public virtual bool IsAttack { get { return false; } }
        public virtual bool RequiredTarget { get { return false; } }
        public virtual HarvestType HarvestType { get { return HarvestType.None; } }
        public virtual IncrementalMinMaxFloat HarvestDamageAmount { get { return new IncrementalMinMaxFloat(); } }

        #region ICustomAimController implements
        public virtual bool HasCustomAimControls()
        {
            return false;
        }

        public virtual AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return default;
        }

        public virtual void FinishAimControls(bool isCancel)
        {
        }

        public virtual bool IsChanneledAbility()
        {
            return false;
        }
        #endregion

        public bool IsActive
        {
            get { return SkillType == SkillType.Active; }
        }

        public bool IsPassive
        {
            get { return SkillType == SkillType.Passive; }
        }

        public Dictionary<DamageElement, MinMaxFloat> GetAttackDamages(ICharacterData skillUser, int skillLevel, bool isLeftHand)
        {
            Dictionary<DamageElement, MinMaxFloat> damageAmounts = new Dictionary<DamageElement, MinMaxFloat>();

            if (!IsAttack)
                return damageAmounts;

            // Base attack damage amount will sum with other variables later
            if (TryGetBaseAttackDamageAmount(skillUser, skillLevel, isLeftHand, out KeyValuePair<DamageElement, MinMaxFloat> baseDamageAmount))
                GameDataHelpers.CombineDamages(damageAmounts, baseDamageAmount);

            // Sum damage with weapon damage inflictions
            if (TryGetAttackWeaponDamageInflictions(skillUser, skillLevel, out Dictionary<DamageElement, float> damageInflictions))
            {
                // Prepare weapon damage amount
                KeyValuePair<DamageElement, MinMaxFloat> weaponDamageAmount;
                if (isLeftHand && skillUser.GetCaches().LeftHandWeaponDamage.HasValue)
                    weaponDamageAmount = skillUser.GetCaches().LeftHandWeaponDamage.Value;
                else
                    weaponDamageAmount = skillUser.GetCaches().RightHandWeaponDamage.Value;

                foreach (DamageElement element in damageInflictions.Keys)
                {
                    if (element == null)
                        continue;
                    GameDataHelpers.CombineDamages(damageAmounts, new KeyValuePair<DamageElement, MinMaxFloat>(element, weaponDamageAmount.Value * damageInflictions[element]));
                }
            }

            // Multiply weapon damage with damage multiplicator
            if (TryGetAttackWeaponDamageMultiplicator(skillUser, skillLevel, out float multiplicator))
            {
                // Apply the multiplicator to the base weapon damage amount for either left or right hand
                KeyValuePair<DamageElement, MinMaxFloat> weaponDamageAmount;
                if (isLeftHand && skillUser.GetCaches().LeftHandWeaponDamage.HasValue)
                    weaponDamageAmount = skillUser.GetCaches().LeftHandWeaponDamage.Value;
                else
                    weaponDamageAmount = skillUser.GetCaches().RightHandWeaponDamage.Value;

                // Multiply both min and max damage by the multiplicator
                GameDataHelpers.CombineDamages(damageAmounts, new KeyValuePair<DamageElement, MinMaxFloat>(weaponDamageAmount.Key, weaponDamageAmount.Value * multiplicator));
            }

            // Sum damage with additional damage amounts
            if (TryGetAttackAdditionalDamageAmounts(skillUser, skillLevel, out Dictionary<DamageElement, MinMaxFloat> additionalDamageAmounts))
                GameDataHelpers.CombineDamages(damageAmounts, additionalDamageAmounts);

            // Sum damage with buffs
            if (IsIncreaseAttackDamageAmountsWithBuffs(skillUser, skillLevel))
            {
                GameDataHelpers.CombineDamages(damageAmounts, skillUser.GetCaches().IncreaseDamages);
                using (CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get(out Dictionary<DamageElement, MinMaxFloat> multiplyDamages))
                {
                    GameDataHelpers.CombineDamages(multiplyDamages, damageAmounts);
                    GameDataHelpers.MultiplyDamages(multiplyDamages, skillUser.GetCaches().IncreaseDamagesRate);
                    GameDataHelpers.CombineDamages(damageAmounts, multiplyDamages);
                }
            }

            return damageAmounts;
        }

        public virtual bool IsIncreaseAttackDamageAmountsWithBuffs(ICharacterData skillUser, int skillLevel)
        {
            return false;
        }

        public virtual bool TryGetBaseAttackDamageAmount(ICharacterData skillUser, int skillLevel, bool isLeftHand, out KeyValuePair<DamageElement, MinMaxFloat> baseDamageAmount)
        {
            baseDamageAmount = default;
            return false;
        }

        public virtual bool TryGetAttackWeaponDamageInflictions(ICharacterData skillUser, int skillLevel, out Dictionary<DamageElement, float> weaponDamageInflictions)
        {
            weaponDamageInflictions = null;
            return false;
        }

        public virtual bool TryGetAttackWeaponDamageMultiplicator(ICharacterData skillUser, int skillLevel, out float weaponDamageMultiplicator)
        {
            weaponDamageMultiplicator = 0;
            return false;
        }

        public virtual bool TryGetAttackAdditionalDamageAmounts(ICharacterData skillUser, int skillLevel, out Dictionary<DamageElement, MinMaxFloat> additionalDamageAmounts)
        {
            additionalDamageAmounts = null;
            return false;
        }

        public virtual bool TryGetBuff(out Buff buff)
        {
            buff = null;
            return false;
        }

        public virtual bool TryGetDebuff(out Buff debuff)
        {
            debuff = null;
            return false;
        }

        public virtual bool TryGetSummon(out SkillSummon summon)
        {
            summon = null;
            return false;
        }

        public virtual bool TryGetMount(out SkillMount mount)
        {
            mount = null;
            return false;
        }

        public virtual bool TryGetItemCraft(out ItemCraft itemCraft)
        {
            itemCraft = null;
            return false;
        }

        public virtual bool TryGetAttackStatusEffectApplyings(out StatusEffectApplying[] attackStatusEffects)
        {
            attackStatusEffects = null;
            return false;
        }

        public abstract float GetCastDistance(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand);

        public virtual float GetCastFov(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand)
        {
            return 360f;
        }

        public virtual bool DecreaseResources(
            BaseCharacterEntity skillUser,
            CharacterItem weapon,
            bool isLeftHand,
            out Dictionary<DamageElement, MinMaxFloat> increaseDamageAmounts)
        {
            increaseDamageAmounts = null;
            if (skillUser is BasePlayerCharacterEntity)
            {
                // Not enough items
                if (!DecreaseItems(skillUser))
                    return false;
                // Not enough ammos
                if (!DecreaseAmmos(skillUser, isLeftHand, out increaseDamageAmounts))
                    return false;
            }
            return true;
        }

        public virtual List<Dictionary<DamageElement, MinMaxFloat>> PrepareDamageAmounts(
            BaseCharacterEntity skillUser,
            bool isLeftHand,
            Dictionary<DamageElement, MinMaxFloat> baseDamageAmounts,
            int triggerCount)
        {
            List<Dictionary<DamageElement, MinMaxFloat>> result;
            switch (requireAmmoType)
            {
                case RequireAmmoType.BasedOnWeapon:
                    return skillUser.PrepareDamageAmounts(isLeftHand, baseDamageAmounts, triggerCount, requireAmmoAmount, true);
                case RequireAmmoType.BasedOnSkill:
                    result = new List<Dictionary<DamageElement, MinMaxFloat>>();
                    Dictionary<DamageElement, MinMaxFloat> tempIncreaseDamageAmounts;
                    for (int i = 0; i < triggerCount; ++i)
                    {
                        if (!DecreaseAmmos(skillUser, isLeftHand, out tempIncreaseDamageAmounts, false))
                            break;
                        using (CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get(out Dictionary<DamageElement, MinMaxFloat> newDamageAmounts))
                        {
                            foreach (KeyValuePair<DamageElement, MinMaxFloat> damageAmount in baseDamageAmounts)
                            {
                                newDamageAmounts[damageAmount.Key] = damageAmount.Value;
                            }
                            GameDataHelpers.CombineDamages(newDamageAmounts, tempIncreaseDamageAmounts);
                            result.Add(newDamageAmounts);
                        }
                    }
                    return result;
            }
            result = new List<Dictionary<DamageElement, MinMaxFloat>>();
            for (int i = 0; i < triggerCount; ++i)
            {
                result.Add(baseDamageAmounts);
            }
            return result;
        }

        /// <summary>
        /// Apply skill
        /// </summary>
        /// <param name="skillUser"></param>
        /// <param name="skillLevel"></param>
        /// <param name="weaponHandlingState"></param>
        /// <param name="weapon"></param>
        /// <param name="simulateSeed"></param>
        /// <param name="triggerIndex"></param>
        /// <param name="damageAmounts"></param>
        /// <param name="targetObjectId"></param>
        /// <param name="aimPosition"></param>
        /// <returns></returns>
        public void ApplySkill(
            BaseCharacterEntity skillUser,
            int skillLevel,
            WeaponHandlingState weaponHandlingState,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            uint targetObjectId,
            AimPosition aimPosition)
        {

            skillLevel = skillUser is BaseMonsterCharacterEntity ? 1 : skillLevel;
            if (skillUser == null || skillUser.IsDead() || (skillUser is not BaseMonsterCharacterEntity && skillLevel <= 0))
                return;
            ApplySkillItemCraft(skillUser);
            ApplySkillSummon(skillUser, skillLevel);
            ApplySkillMount(skillUser, skillLevel);
            ApplySkillImplement(
                skillUser,
                skillLevel,
                weaponHandlingState,
                weapon,
                simulateSeed,
                triggerIndex,
                0,
                damageAmounts,
                targetObjectId,
                aimPosition);
        }

        protected virtual void ApplySkillItemCraft(BaseCharacterEntity skillUser)
        {
            if (!skillUser.IsServer)
                return;

            if (!(skillUser is BasePlayerCharacterEntity playerCharacterEntity))
                return;

            if (!TryGetItemCraft(out ItemCraft itemCraft) || itemCraft.CraftingItem == null)
                return;

            // Item crafted
            itemCraft.CraftItem(playerCharacterEntity);
        }

        protected virtual void ApplySkillSummon(BaseCharacterEntity skillUser, int skillLevel)
        {
            if (!skillUser.IsServer)
                return;

            if (!TryGetSummon(out SkillSummon summon))
                return;

            int i;
            int amountEachTime = summon.AmountEachTime.GetAmount(skillLevel);
            for (i = 0; i < amountEachTime; ++i)
            {
                CharacterSummon newSummon = CharacterSummon.Create(SummonType.Skill, string.Empty, DataId);
                newSummon.Summon(skillUser, summon.Level.GetAmount(skillLevel), summon.Duration.GetAmount(skillLevel));
                skillUser.Summons.Add(newSummon);
            }
            int count = 0;
            for (i = 0; i < skillUser.Summons.Count; ++i)
            {
                if (skillUser.Summons[i].dataId == DataId)
                    ++count;
            }
            int maxStack = summon.MaxStack.GetAmount(skillLevel);
            int unSummonAmount = count > maxStack ? count - maxStack : 0;
            CharacterSummon tempSummon;
            for (i = unSummonAmount; i > 0; --i)
            {
                int summonIndex = skillUser.IndexOfSummon(SummonType.Skill, DataId);
                tempSummon = skillUser.Summons[summonIndex];
                if (summonIndex >= 0)
                {
                    skillUser.Summons.RemoveAt(summonIndex);
                    tempSummon.UnSummon(skillUser);
                }
            }
        }

        protected virtual void ApplySkillMount(BaseCharacterEntity skillUser, int skillLevel)
        {
            if (!skillUser.IsServer)
                return;

            if (!TryGetMount(out SkillMount mount))
                return;

            skillUser.SpawnMount(
                MountType.Skill, Id,
                mount.Duration.GetAmount(skillLevel),
                mount.Level.GetAmount(skillLevel));
        }

        /// <summary>
        /// Apply skill
        /// </summary>
        /// <param name="skillUser"></param>
        /// <param name="skillLevel"></param>
        /// <param name="weaponHandlingState"></param>
        /// <param name="weapon"></param>
        /// <param name="simulateSeed"></param>
        /// <param name="triggerIndex"></param>
        /// <param name="spreadIndex"></param>
        /// <param name="damageAmounts"></param>
        /// <param name="targetObjectId"></param>
        /// <param name="aimPosition"></param>
        /// <returns></returns>
        protected abstract void ApplySkillImplement(
            BaseCharacterEntity skillUser,
            int skillLevel,
            WeaponHandlingState weaponHandlingState,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            uint targetObjectId,
            AimPosition aimPosition);

        /// <summary>
        /// Return TRUE if this will override default attack function
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="skillLevel"></param>
        /// <param name="isLeftHand"></param>
        /// <param name="weapon"></param>
        /// <param name="simulateSeed"></param>
        /// <param name="triggerIndex"></param>
        /// <param name="damageAmounts"></param>
        /// <param name="aimPosition"></param>
        /// <returns></returns>
        public virtual bool OnAttack(
            BaseCharacterEntity attacker,
            int skillLevel,
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            AimPosition aimPosition)
        {
            return false;
        }

        /// <summary>
        /// This will be called when skill attack hit `target`, do something such as apply debuff to `target` here
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <param name="instigator"></param>
        /// <param name="weapon"></param>
        /// <param name="target"></param>
        public virtual void OnSkillAttackHit(int skillLevel, EntityInfo instigator, CharacterItem weapon, BaseCharacterEntity target)
        {
            if (TryGetDebuff(out _))
                target.ApplyBuff(DataId, BuffType.SkillDebuff, skillLevel, instigator, weapon);
            if (TryGetAttackStatusEffectApplyings(out StatusEffectApplying[] statusEffectApplyings))
                statusEffectApplyings.ApplyStatusEffect(skillLevel, instigator, weapon, target);
        }

        public virtual bool CanLevelUp(IPlayerCharacterData character, int level, out UITextKeys gameMessage, bool checkSkillPoint = true, bool checkGold = true)
        {
            if (character == null)
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_CHARACTER_DATA;
                return false;
            }

            BaseCharacter data = character.GetDatabase();
            if (data == null || !data.GetLearnableSkillDataIds().Contains(DataId))
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_CHARACTER_DATA;
                return false;
            }

            if (IsDisallowToLevelUp(level))
            {
                gameMessage = UITextKeys.UI_ERROR_DISALLOW_SKILL_LEVEL_UP;
                return false;
            }

            if (character.Level < GetRequireCharacterLevel(level))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_LEVEL;
                return false;
            }

            // Calculate with skill level when character's level is `1` only
            if (maxLevel > 0)
            {
                using (CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get(out Dictionary<BaseSkill, int> tempData))
                {
                    character.GetDatabase().GetSkillLevels(1, tempData);
                    if (level + tempData[this] >= maxLevel)
                    {
                        gameMessage = UITextKeys.UI_ERROR_SKILL_REACHED_MAX_LEVEL;
                        return false;
                    }
                }
            }

            if (checkSkillPoint && character.SkillPoint < GetRequireCharacterSkillPoint(level))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_SKILL_POINT;
                return false;
            }

            if (checkGold && character.Gold < GetRequireCharacterGold(level))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_GOLD;
                return false;
            }

            // Check is it pass attribute requirement or not
            using (CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Get(out Dictionary<Attribute, float> requireAttributeAmounts))
            {
                GetRequireAttributeAmounts(level, requireAttributeAmounts);
                if (!character.HasEnoughAttributeAmounts(requireAttributeAmounts, false, out gameMessage, out _, willReleaseAttributes: true))
                    return false;
            }

            // Check is it pass skill level requirement or not
            using (CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get(out Dictionary<BaseSkill, int> requireSkillLevels))
            {
                GetRequireSkillLevels(level, requireSkillLevels);
                if (!character.HasEnoughSkillLevels(requireSkillLevels, false, out gameMessage, out _, willReleaseSkills: true))
                    return false;
            }

            // Check is it pass currency requirement or not
            using (CollectionPool<Dictionary<Currency, int>, KeyValuePair<Currency, int>>.Get(out Dictionary<Currency, int> requireCurrencyAmounts))
            {
                GetRequireCurrencyAmounts(level, requireCurrencyAmounts);
                if (!character.HasEnoughCurrencyAmounts(requireCurrencyAmounts, out gameMessage, out _))
                    return false;
            }

            // Check is it pass item requirement or not
            using (CollectionPool<Dictionary<BaseItem, int>, KeyValuePair<BaseItem, int>>.Get(out Dictionary<BaseItem, int> requireItemAmounts))
            {
                GetRequireItemAmounts(level, requireItemAmounts);
                if (!character.HasEnoughNonEquipItemAmounts(requireItemAmounts, out gameMessage, out _))
                    return false;
            }

            return true;
        }

        public virtual bool CanUse(BaseCharacterEntity character, int level, bool isLeftHand, uint targetObjectId, out UITextKeys gameMessage, bool isItem = false)
        {
            gameMessage = UITextKeys.NONE;
            if (character == null)
                return false;

            if (level <= 0)
            {
                gameMessage = UITextKeys.UI_ERROR_SKILL_LEVEL_IS_ZERO;
                return false;
            }

            BasePlayerCharacterEntity playerCharacter = character as BasePlayerCharacterEntity;
            if (playerCharacter != null)
            {
                // Only player character will check is skill is learned
                if (!isItem && !IsAvailable(character))
                {
                    gameMessage = UITextKeys.UI_ERROR_SKILL_IS_NOT_LEARNED;
                    return false;
                }

                if (TryGetItemCraft(out ItemCraft itemCraft) && !itemCraft.CanCraft(playerCharacter, out gameMessage))
                {
                    // Cannot craft the item
                    return false;
                }

                // Only player character will be checked for available weapons
                if (requireShield)
                {
                    IShieldItem leftShieldItem = character.EquipWeapons.GetLeftHandShieldItem();
                    if (leftShieldItem == null)
                    {
                        gameMessage = UITextKeys.UI_ERROR_CANNOT_USE_SKILL_WITHOUT_SHIELD;
                        return false;
                    }
                }

                if (CacheAvailableWeapons.Count > 0)
                {
                    bool available = false;
                    IWeaponItem rightWeaponItem = character.EquipWeapons.GetRightHandWeaponItem();
                    IWeaponItem leftWeaponItem = character.EquipWeapons.GetLeftHandWeaponItem();
                    if (rightWeaponItem != null && CacheAvailableWeapons.Contains(rightWeaponItem.WeaponType))
                    {
                        available = true;
                    }
                    else if (leftWeaponItem != null && CacheAvailableWeapons.Contains(leftWeaponItem.WeaponType))
                    {
                        available = true;
                    }
                    else if (rightWeaponItem == null && leftWeaponItem == null &&
                        CacheAvailableWeapons.Contains(GameInstance.Singleton.DefaultWeaponItem.WeaponType))
                    {
                        available = true;
                    }
                    if (!available)
                    {
                        gameMessage = UITextKeys.UI_ERROR_CANNOT_USE_SKILL_BY_CURRENT_WEAPON;
                        return false;
                    }
                }

                if (CacheAvailableArmors.Count > 0)
                {
                    bool available = false;
                    IArmorItem armorItem;
                    foreach (CharacterItem characterItem in character.EquipItems)
                    {
                        armorItem = characterItem.GetArmorItem();
                        if (armorItem != null && CacheAvailableArmors.Contains(armorItem.ArmorType))
                        {
                            available = true;
                            break;
                        }
                    }
                    if (!available)
                    {
                        gameMessage = UITextKeys.UI_ERROR_CANNOT_USE_SKILL_BY_CURRENT_ARMOR;
                        return false;
                    }
                }

                if (CacheAvailableVehicles.Count > 0)
                {
                    if (character.PassengingVehicleType == null ||
                        !character.PassengingVehicleEntity.IsDriver(character.PassengingVehicleSeatIndex) ||
                        !CacheAvailableVehicles.Contains(character.PassengingVehicleType))
                    {
                        gameMessage = UITextKeys.UI_ERROR_CANNOT_USE_SKILL_BY_CURRENT_VEHICLE;
                        return false;
                    }
                }

                if (!HasEnoughItems(character, out _, out _))
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_ITEMS;
                    return false;
                }

                if (!HasEnoughAmmos(character, isLeftHand, out _, out _))
                {
                    gameMessage = UITextKeys.UI_ERROR_NO_AMMO;
                    return false;
                }
            }

            if (character.CurrentHp < GetTotalConsumeHp(level, character))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_HP;
                return false;
            }

            if (character.CurrentMp < GetTotalConsumeMp(level, character))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_MP;
                return false;
            }

            if (character.CurrentStamina < GetTotalConsumeStamina(level, character))
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_STAMINA;
                return false;
            }

            if (!isItem)
            {
                if (character.IndexOfSkillUsage(SkillUsageType.Skill, DataId) >= 0)
                {
                    gameMessage = UITextKeys.UI_ERROR_SKILL_IS_COOLING_DOWN;
                    return false;
                }
            }

            if (RequiredTarget)
            {
                BaseCharacterEntity targetEntity;
                if (!character.CurrentGameManager.TryGetEntityByObjectId(targetObjectId, out targetEntity))
                {
                    gameMessage = UITextKeys.UI_ERROR_NO_SKILL_TARGET;
                    return false;
                }
                else if (!character.IsGameEntityInDistance(targetEntity, GetCastDistance(character, level, isLeftHand)))
                {
                    gameMessage = UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Find one which has enough amount from require items
        /// </summary>
        /// <param name="character"></param>
        /// <param name="itemDataId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        protected bool HasEnoughItems(BaseCharacterEntity character, out int itemDataId, out int amount)
        {
            itemDataId = 0;
            amount = 0;
            if (requireItems == null || requireItems.Length == 0)
                return true;
            foreach (ItemAmount requireItem in requireItems)
            {
                if (character.CountNonEquipItems(requireItem.item.DataId) >= requireItem.amount)
                {
                    itemDataId = requireItem.item.DataId;
                    amount = requireItem.amount;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find one which has enough amount from require ammos
        /// </summary>
        /// <param name="character"></param>
        /// <param name="isLeftHand"></param>
        /// <param name="ammoType"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        protected bool HasEnoughAmmos(BaseCharacterEntity character, bool isLeftHand, out AmmoType ammoType, out int amount)
        {
            ammoType = null;
            amount = 0;
            switch (requireAmmoType)
            {
                case RequireAmmoType.BasedOnWeapon:
                    return character.ValidateAmmo(character.GetAvailableWeapon(ref isLeftHand), requireAmmoAmount, false);
                case RequireAmmoType.BasedOnSkill:
                    if (requireAmmos == null || requireAmmos.Length == 0)
                        return true;
                    foreach (AmmoTypeAmount requireAmmo in requireAmmos)
                    {
                        if (character.CountAllAmmos(requireAmmo.ammoType) >= requireAmmo.amount)
                        {
                            ammoType = requireAmmo.ammoType;
                            amount = requireAmmo.amount;
                            return true;
                        }
                    }
                    return false;
            }
            return true;
        }

        protected bool DecreaseItems(BaseCharacterEntity character)
        {
            if (HasEnoughItems(character, out int itemDataId, out int amount))
            {
                if (itemDataId == 0 || amount == 0)
                {
                    // No required items, don't decrease items
                    return true;
                }
                if (character.DecreaseItems(itemDataId, amount))
                {
                    character.FillEmptySlots();
                    return true;
                }
            }
            return false;
        }

        protected bool DecreaseAmmos(BaseCharacterEntity character, bool isLeftHand, out Dictionary<DamageElement, MinMaxFloat> increaseDamageAmounts, bool applyChanges = true)
        {
            increaseDamageAmounts = null;
            AmmoType ammoType;
            int amount;
            switch (requireAmmoType)
            {
                case RequireAmmoType.BasedOnWeapon:
                    return character.DecreaseAmmos(isLeftHand, requireAmmoAmount, out increaseDamageAmounts, false, applyChanges);
                case RequireAmmoType.BasedOnSkill:
                    if (HasEnoughAmmos(character, isLeftHand, out ammoType, out amount))
                    {
                        if (ammoType == null || amount == 0)
                        {
                            // No required ammos, don't decrease ammos
                            return true;
                        }
                        if (character.DecreaseAmmos(ammoType, amount, out increaseDamageAmounts, applyChanges))
                        {
                            character.FillEmptySlots();
                            return true;
                        }
                    }
                    return false;
            }
            return true;
        }

        public DamageInfo GetDamageInfo(BaseCharacterEntity skillUser, bool isLeftHand)
        {
            if (TryGetDamageInfo(skillUser, isLeftHand, out DamageInfo damageInfo))
                return damageInfo;
            return null;
        }

        public virtual bool TryGetDamageInfo(BaseCharacterEntity skillUser, bool isLeftHand, out DamageInfo damageInfo)
        {
            damageInfo = null;
            return false;
        }

        public virtual Transform GetApplyTransform(BaseCharacterEntity skillUser, bool isLeftHand)
        {
            return skillUser.MeleeDamageTransform;
        }

        public virtual Vector3 GetDefaultAttackAimPosition(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand, IDamageableEntity target)
        {
            return target.OpponentAimTransform.position;
        }

        public void MakeRequirementEachLevels()
        {
            requirementEachLevels.Clear();
            for (int i = 1; i <= maxLevel; ++i)
            {
                requirementEachLevels.Add(new SkillRequirementEntry()
                {
                    disallow = requirement.disallow,
                    characterLevel = requirement.characterLevel.GetAmount(i),
                    skillPoint = requirement.skillPoint.GetAmount(i),
                    gold = requirement.gold.GetAmount(i),
                    attributeAmounts = requirement.attributeAmounts,
                    skillLevels = requirement.skillLevels,
                    currencyAmounts = requirement.currencyAmounts,
                    itemAmounts = requirement.itemAmounts,
                });
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
                EditorUtility.SetDirty(this);
#endif
        }
    }
}
