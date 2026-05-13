using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        // Note: You may use `Awake` dev extension to setup an events and `OnDestroy` to desetup an events
        // Generic events
        [Category("Events")]
        public UnityEvent onDead = new UnityEvent();
        public UnityEvent onRespawn = new UnityEvent();
        public UnityEvent onLevelUp = new UnityEvent();
        // Caching
        public event CharacterEntityDelegate onRecached;
        // Sync variables
        public event CharacterEntityStringChangeDelegate onIdChange;
        public event CharacterEntityStringChangeDelegate onCharacterNameChange;
        public event CharacterEntityInt32ChangeDelegate onLevelChange;
        public event CharacterEntityInt32ChangeDelegate onExpChange;
        public event CharacterEntityBooleanChangeDelegate onIsInvincibleChange;
        public event CharacterEntityInt32ChangeDelegate onCurrentMpChange;
        public event CharacterEntityInt32ChangeDelegate onCurrentStaminaChange;
        public event CharacterEntityInt32ChangeDelegate onCurrentFoodChange;
        public event CharacterEntityInt32ChangeDelegate onCurrentWaterChange;
        public event CharacterEntityUInt8ChangeDelegate onEquipWeaponSetChange;
        public event CharacterEntityBooleanChangeDelegate onIsWeaponsSheathedChange;
        public event CharacterEntityUInt16ChangeDelegate onPitchChange;
        public event CharacterEntityVector3ChangeDelegate onLookPositionChange;
        public event CharacterEntityAimPositionChangeDelegate onAimPositionChange;
        public event CharacterEntityUInt32ChangeDelegate onTargetEntityIdChange;
        public event CharacterEntityMountChangeDelegate onMountChange;
        public event CharacterEntitySummonerChangeDelegate onSummonerChange;
        // Sync lists
        public event LiteNetLibSyncList<EquipWeapons>.OnOperationDelegate onSelectableWeaponSetsOperation;
        public event LiteNetLibSyncList<CharacterAttribute>.OnOperationDelegate onAttributesOperation;
        public event LiteNetLibSyncList<CharacterSkill>.OnOperationDelegate onSkillsOperation;
        public event LiteNetLibSyncList<CharacterSkillUsage>.OnOperationDelegate onSkillUsagesOperation;
        public event LiteNetLibSyncList<CharacterBuff>.OnOperationDelegate onBuffsOperation;
        public event LiteNetLibSyncList<CharacterItem>.OnOperationDelegate onEquipItemsOperation;
        public event LiteNetLibSyncList<CharacterItem>.OnOperationDelegate onNonEquipItemsOperation;
        public event LiteNetLibSyncList<CharacterSummon>.OnOperationDelegate onSummonsOperation;
        // Action events
        public event AttackRoutineDelegate onAttackRoutine;
        public event UseSkillRoutineDelegate onUseSkillRoutine;
        public event LaunchDamageEntityDelegate onLaunchDamageEntity;
        public event CanAttackDelegate onCanAttackValidated;
        public event CanUseSkillDelegate onCanUseSkillValidated;
        public event CanUseSkillDelegate onCanUseSkillItemValidated;
        public event CanReloadDelegate onCanReloadValidated;
        // Buff events
        public event ApplyBuffDelegate onApplyBuff;
        public event RemoveBuffDelegate onRemoveBuff;
        public event AppliedRecoveryAmountDelegate onBuffHpRecovery;
        public event AppliedRecoveryAmountDelegate onBuffHpDecrease;
        public event AppliedRecoveryAmountDelegate onBuffMpRecovery;
        public event AppliedRecoveryAmountDelegate onBuffMpDecrease;
        public event AppliedRecoveryAmountDelegate onBuffStaminaRecovery;
        public event AppliedRecoveryAmountDelegate onBuffStaminaDecrease;
        public event AppliedRecoveryAmountDelegate onBuffFoodRecovery;
        public event AppliedRecoveryAmountDelegate onBuffFoodDecrease;
        public event AppliedRecoveryAmountDelegate onBuffWaterRecovery;
        public event AppliedRecoveryAmountDelegate onBuffWaterDecrease;
        // Enemy spotted events
        public event NotifyEnemySpottedDelegate onNotifyEnemySpotted;
        public event NotifyEnemySpottedByAllyDelegate onNotifyEnemySpottedByAlly;

        public virtual void OnAttackRoutine(
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            DamageInfo damageInfo,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            AimPosition aimPosition)
        {
            if (onAttackRoutine != null)
                onAttackRoutine.Invoke(this, isLeftHand, weapon, simulateSeed, triggerIndex, damageInfo, damageAmounts, aimPosition);
        }

        public virtual void OnUseSkillRoutine(
            BaseSkill skill,
            int level,
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            uint targetObjectId,
            AimPosition aimPosition)
        {
            if (onUseSkillRoutine != null)
                onUseSkillRoutine.Invoke(this, skill, level, isLeftHand, weapon, simulateSeed, triggerIndex, damageAmounts, targetObjectId, aimPosition);
        }

        public virtual void OnLaunchDamageEntity(
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            BaseSkill skill,
            int skillLevel,
            AimPosition aimPosition)
        {
            if (onLaunchDamageEntity != null)
                onLaunchDamageEntity.Invoke(this, isLeftHand, weapon, simulateSeed, triggerIndex, spreadIndex, damageAmounts, skill, skillLevel, aimPosition);
        }

        public virtual void OnRewardItem(RewardGivenType givenType, BaseItem item, int amount)
        {

        }

        public virtual void OnRewardItem(RewardGivenType givenType, CharacterItem item)
        {

        }

        public virtual void OnRewardGold(RewardGivenType givenType, int amount)
        {

        }

        public virtual void OnRewardExp(RewardGivenType givenType, int exp, bool isLevelUp)
        {

        }

        public virtual void OnRewardCurrency(RewardGivenType givenType, Currency currency, int amount)
        {

        }

        public virtual void OnRewardCurrency(RewardGivenType givenType, CharacterCurrency currency)
        {

        }
    }
}
