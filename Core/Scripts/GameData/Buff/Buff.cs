using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class Buff
    {
        public static readonly Buff Empty = new Buff();

        [Header("Generic Settings")]
        public string tag;
        public string[] restrictTags = new string[0];
        [Tooltip("If it is not enable, it will have 100% chance to apply the buff")]
        public bool enableApplyChance;
        [Tooltip("1 = 100% chance to apply the buff")]
        public IncrementalFloat applyChance = new IncrementalFloat();

        [Header("Settings for Passive and Active Skills")]
        [Tooltip("Increase character's stats.")]
        public CharacterStatsIncremental increaseStats = new CharacterStatsIncremental();
        [Tooltip("Increase character's stats rate.")]
        public CharacterStatsIncremental increaseStatsRate = new CharacterStatsIncremental();
        [Tooltip("Increase character's attributes.")]
        [ArrayElementTitle("attribute")]
        public AttributeIncremental[] increaseAttributes = new AttributeIncremental[0];
        [Tooltip("Increase character's attributes rate.")]
        [ArrayElementTitle("attribute")]
        public AttributeIncremental[] increaseAttributesRate = new AttributeIncremental[0];
        [Tooltip("Increase character's resistances.")]
        [ArrayElementTitle("damageElement")]
        public ResistanceIncremental[] increaseResistances = new ResistanceIncremental[0];
        [Tooltip("Increase character's armors.")]
        [ArrayElementTitle("damageElement")]
        public ArmorIncremental[] increaseArmors = new ArmorIncremental[0];
        [Tooltip("Increase character's armors rate.")]
        [ArrayElementTitle("damageElement")]
        public ArmorIncremental[] increaseArmorsRate = new ArmorIncremental[0];
        [Tooltip("Increase character's damages.")]
        [ArrayElementTitle("damageElement")]
        public DamageIncremental[] increaseDamages = new DamageIncremental[0];
        [Tooltip("Increase character's damages rate.")]
        [ArrayElementTitle("damageElement")]
        public DamageIncremental[] increaseDamagesRate = new DamageIncremental[0];
        [Tooltip("Increase character's skill level. Unlearn skills can be used if increased by this buff. Passive skills are excluded")]
        [ArrayElementTitle("skill")]
        public SkillIncremental[] increaseSkills = new SkillIncremental[0];
        [Tooltip("If this is `TRUE`, it will override character's attacking damage info by `overrideDamageInfo`")]
        public bool isOverrideDamageInfo;
        [Tooltip("If `isOverrideDamageInfo` is `TRUE`, it will override damage info to this instead")]
        public DamageInfo overrideDamageInfo = null;
        [Tooltip("If this is `TRUE`, it will override character's skills by `overrideSkills`")]
        public bool isOverrideSkills;
        [Tooltip("If `isOverrideSkills` is `TRUE`, it will override skills list to use this list instead")]
        [ArrayElementTitle("skill")]
        public SkillIncremental[] overrideSkills = null;
        [Tooltip("Mount settings for buff")]
        public BuffMount mount = new BuffMount();
        [Tooltip("Increase character's status effect resistance.")]
        [ArrayElementTitle("statusEffect")]
        public StatusEffectResistanceIncremental[] increaseStatusEffectResistances = new StatusEffectResistanceIncremental[0];
        [Header("Settings for Active Skills only")]
        [Tooltip("If duration less than or equals to 0, buff stats won't applied only recovery will be applied. This won't be applied to monster's summoner.")]
        public IncrementalFloat duration = new IncrementalFloat();
        [Tooltip("If this is `TRUE`, it won't use `duration` and buff won't be removed by duration")]
        public bool noDuration = false;
        [Tooltip("Recover character's current HP. This won't be applied to monster's summoner.")]
        public IncrementalInt recoveryHp = new IncrementalInt();
        [Tooltip("Recover character's current MP. This won't be applied to monster's summoner.")]
        public IncrementalInt recoveryMp = new IncrementalInt();
        [Tooltip("Recover character's current stamina. This won't be applied to monster's summoner.")]
        public IncrementalInt recoveryStamina = new IncrementalInt();
        [Tooltip("Recover character's current food. This won't be applied to monster's summoner.")]
        public IncrementalInt recoveryFood = new IncrementalInt();
        [Tooltip("Recover character's current water. This won't be applied to monster's summoner.")]
        public IncrementalInt recoveryWater = new IncrementalInt();
        [Tooltip("Set buffs that you want to remove when this buff is appied here.")]
        public BuffRemoval[] buffRemovals = new BuffRemoval[0];
        [Tooltip("Applies damage within duration to character. This won't be applied to monster's summoner.")]
        [ArrayElementTitle("damageElement")]
        public DamageIncremental[] damageOverTimes = new DamageIncremental[0];
        [Tooltip("`disallowMove`, `disallowAttack`, `disallowUseSkill`, `disallowUseItem` and `freezeAnimation` will be used if this is `None`")]
        public AilmentPresets ailment = AilmentPresets.None;
        [Tooltip("Disallow character to move while applied. This won't be applied to monster's summoner.")]
        public bool disallowMove = false;
        public bool disallowSprint = false;
        public bool disallowWalk = false;
        public bool disallowJump = false;
        public bool disallowDash = false;
        public bool disallowCrouch = false;
        public bool disallowCrawl = false;
        [Tooltip("Disallow character to attack while applied. This won't be applied to monster's summoner.")]
        public bool disallowAttack = false;
        [Tooltip("Disallow character to use skill while applied. This won't be applied to monster's summoner.")]
        public bool disallowUseSkill = false;
        [Tooltip("Disallow character to use item while applied. This won't be applied to monster's summoner.")]
        public bool disallowUseItem = false;
        [Tooltip("Freeze animation while this buff is applied")]
        public bool freezeAnimation = false;
        [Tooltip("1 = 100% chance to remove this buff when attacking")]
        public IncrementalFloat removeBuffWhenAttackChance = new IncrementalFloat();
        [Tooltip("1 = 100% chance to remove this buff when attacked")]
        public IncrementalFloat removeBuffWhenAttackedChance = new IncrementalFloat();
        [Tooltip("1 = 100% chance to remove this buff when using skill")]
        public IncrementalFloat removeBuffWhenUseSkillChance = new IncrementalFloat();
        [Tooltip("1 = 100% chance to remove this buff when using item")]
        public IncrementalFloat removeBuffWhenUseItemChance = new IncrementalFloat();
        [Tooltip("1 = 100% chance to remove this buff when picking item up")]
        public IncrementalFloat removeBuffWhenPickupItemChance = new IncrementalFloat();
        [Tooltip("Hide character. This won't be applied to monster's summoner.")]
        public bool isHide = false;
        [Tooltip("Reveals hidding characters")]
        public bool isRevealsHide = false;
        [Tooltip("Can't see other characters")]
        public bool isBlind = false;
        [Tooltip("Mute character movement sound while applied. This won't be applied to monster's summoner.")]
        public bool muteFootstepSound = false;
        [Tooltip("Status effects that can be applied to the attacker when attacking.")]
        public StatusEffectApplying[] selfStatusEffectsWhenAttacking = new StatusEffectApplying[0];
        [Tooltip("Status effects that can be applied to the enemy when attacking.")]
        public StatusEffectApplying[] enemyStatusEffectsWhenAttacking = new StatusEffectApplying[0];
        [Tooltip("Status effects that can be applied to the attacker when attacked.")]
        public StatusEffectApplying[] selfStatusEffectsWhenAttacked = new StatusEffectApplying[0];
        [Tooltip("Status effects that can be applied to the enemy when attacked.")]
        public StatusEffectApplying[] enemyStatusEffectsWhenAttacked = new StatusEffectApplying[0];
        [Tooltip("If this is `TRUE` it will not be removed when the character dies")]
        public bool doNotRemoveOnDead = false;
        [Tooltip("If this is `TRUE` it will extend duration when applying buff, not remove and re-apply")]
        public bool isExtendDuration = false;
        [Tooltip("Max stack to applies buff, it won't be used while `isExtendDuration` is `TRUE`")]
        public IncrementalInt maxStack = new IncrementalInt();
        [Tooltip("Game effects which appearing on character while applied. This won't be applied to monster's summoner.")]
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableEffects))]
#endif
        public GameEffect[] effects = new GameEffect[0];
#endif
        public GameEffect[] Effects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return effects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceGameEffect[] addressableEffects = new AssetReferenceGameEffect[0];
        public AssetReferenceGameEffect[] AddressableEffects
        {
            get { return addressableEffects; }
        }
#endif

        public virtual bool TryGetMount(out BuffMount mount)
        {
            if (this.mount.MountEntity != null)
            {
                mount = this.mount;
                return true;
            }
#if !DISABLE_ADDRESSABLES
            else if (this.mount.AddressableMountEntity.IsDataValid())
            {
                mount = this.mount;
                return true;
            }
#endif
            mount = null;
            return false;
        }

        public void PrepareRelatesData()
        {
            GameInstance.AddAttributes(increaseAttributes);
            GameInstance.AddAttributes(increaseAttributesRate);
            GameInstance.AddDamageElements(increaseResistances);
            GameInstance.AddDamageElements(increaseArmors);
            GameInstance.AddDamageElements(increaseDamages);
            GameInstance.AddSkills(increaseSkills);
            GameInstance.AddSkills(overrideSkills);
            GameInstance.AddStatusEffects(selfStatusEffectsWhenAttacking);
            GameInstance.AddStatusEffects(enemyStatusEffectsWhenAttacking);
            GameInstance.AddStatusEffects(selfStatusEffectsWhenAttacked);
            GameInstance.AddStatusEffects(enemyStatusEffectsWhenAttacked);
            if (TryGetMount(out BuffMount mount))
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                GameInstance.AddVehicleEntities(mount.MountEntity);
#endif
#if !DISABLE_ADDRESSABLES
                GameInstance.AddAssetReferenceVehicleEntities(mount.AddressableMountEntity);
#endif
            }
        }
    }
}
