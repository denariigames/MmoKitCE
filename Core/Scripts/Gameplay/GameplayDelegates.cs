using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MultiplayerARPG
{
    public delegate void IsUpdateEntityComponentsDelegate(
        BaseGameEntity target,
        bool isUpdate);

    public delegate void NetworkDestroyDelegate(
        BaseGameEntity target,
        byte reasons);

    public delegate void ReceiveDamageDelegate(
        DamageableEntity target,
        HitBoxPosition position,
        Vector3 fromPosition,
        EntityInfo instigator,
        Dictionary<DamageElement, MinMaxFloat> damageAmounts,
        CharacterItem weapon,
        BaseSkill skill,
        int skillLevel);

    public delegate void ReceivedDamageDelegate(
        DamageableEntity target,
        HitBoxPosition position,
        Vector3 fromPosition,
        EntityInfo instigator,
        CombatAmountType combatAmountType,
        int totalDamage,
        CharacterItem weapon,
        BaseSkill skill,
        int skillLevel,
        CharacterBuff buff,
        bool isDamageOverTime);

    public delegate void NotifyEnemySpottedDelegate(
        BaseCharacterEntity target,
        BaseCharacterEntity enemy);

    public delegate void NotifyEnemySpottedByAllyDelegate(
        BaseCharacterEntity target,
        BaseCharacterEntity ally,
        BaseCharacterEntity enemy);

    public delegate void AppliedRecoveryAmountDelegate(
        BaseCharacterEntity target,
        EntityInfo causer,
        int amount);

    public delegate void AttackRoutineDelegate(
        BaseCharacterEntity target,
        bool isLeftHand,
        CharacterItem weapon,
        int simulateSeed,
        byte triggerIndex,
        DamageInfo damageInfo,
        List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
        AimPosition aimPosition);

    public delegate void UseSkillRoutineDelegate(
        BaseCharacterEntity target,
        BaseSkill skill,
        int level,
        bool isLeftHand,
        CharacterItem weapon,
        int simulateSeed,
        byte triggerIndex,
        List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
        uint targetObjectId,
        AimPosition aimPosition);

    public delegate void LaunchDamageEntityDelegate(
        BaseCharacterEntity target,
        bool isLeftHand,
        CharacterItem weapon,
        int simulateSeed,
        byte triggerIndex,
        byte spreadIndex,
        List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
        BaseSkill skill,
        int skillLevel,
        AimPosition aimPosition);

    public delegate void ApplyBuffDelegate(
        BaseCharacterEntity target,
        CharacterBuff buff);

    public delegate void RemoveBuffDelegate(
        BaseCharacterEntity target,
        CharacterBuff buff,
        BuffRemoveReasons reason);

    public delegate void CharacterStatsDelegate(
        ref CharacterStats a,
        CharacterStats b);

    public delegate void CharacterStatsAndNumberDelegate(
        ref CharacterStats a,
        float b);

    public delegate void RandomCharacterStatsDelegate(
        System.Random random,
        ItemRandomBonus randomBonus,
        bool isRateStats,
        ref CharacterStats stats,
        ref int appliedAmount);

    public delegate void CanAttackDelegate(
        BaseGameEntity target,
        ref bool canAttack);

    public delegate void CanUseSkillDelegate(
        BaseGameEntity target,
        ref bool canUseSkill);

    public delegate void CanReloadDelegate(
        BaseGameEntity target,
        ref bool canReload);

    public delegate void CanMoveDelegate(
        BaseGameEntity target,
        ref bool canMove);

    public delegate void CanSprintDelegate(
        BaseGameEntity target,
        ref bool canSprint);

    public delegate void CanWalkDelegate(
        BaseGameEntity target,
        ref bool canWalk);

    public delegate void CanCrouchDelegate(
        BaseGameEntity target,
        ref bool canCrouch);

    public delegate void CanCrawlDelegate(
        BaseGameEntity target,
        ref bool canCrawl);

    public delegate void CanJumpDelegate(
        BaseGameEntity target,
        ref bool canJump);

    public delegate void CanDashDelegate(
        BaseGameEntity target,
        ref bool canDash);

    public delegate void CanTurnDelegate(
        BaseGameEntity target,
        ref bool canTurn);

    public delegate void JumpForceAppliedDelegate(
        BaseGameEntity target,
        float verticalVelocity);

    public delegate void UpdateEquipmentModelsDelegate(
        BaseCharacterModel target,
        CancellationTokenSource cancellationTokenSource,
        Dictionary<string, EquipmentModel> showingModels,
        Dictionary<string, EquipmentModel> storingModels,
        HashSet<string> unequippingSockets);

    public delegate void OnEquipmentModelInstantiateDelegate(
        EquipmentModel target,
        GameObject instantiatedObject,
        BaseEquipmentEntity instantiatedEntity,
        EquipmentInstantiatedObjectGroup instantiatedObjectGroup,
        EquipmentContainer equipmentContainer);

    public delegate void OnCharacterEquipmentModelInstantiateDelegate(
        BaseCharacterModel target,
        EquipmentModel model,
        GameObject instantiatedObject,
        BaseEquipmentEntity instantiatedEntity,
        EquipmentInstantiatedObjectGroup instantiatedObjectGroup,
        EquipmentContainer equipmentContainer);

    public delegate void CalculatedItemBuffDelegate(
        CalculatedItemBuff calculatedItemBuff);

    public delegate void CalculatedBuffDelegate(
        CalculatedBuff calculatedBuff);

    public delegate void OnDropItemDelegate(
        BaseItem item,
        int level,
        int amount);

    #region Game Entity
    public delegate void GameEntityDelegate(
        BaseGameEntity target);

    public delegate void GameEntityInt8ChangeDelegate(
        BaseGameEntity target,
        sbyte oldValue,
        sbyte newValue);

    public delegate void GameEntityInt16ChangeDelegate(
        BaseGameEntity target,
        short oldValue,
        short newValue);

    public delegate void GameEntityInt32ChangeDelegate(
        BaseGameEntity target,
        int oldValue,
        int newValue);

    public delegate void GameEntityInt64ChangeDelegate(
        BaseGameEntity target,
        long oldValue,
        long newValue);

    public delegate void GameEntityUInt8ChangeDelegate(
        BaseGameEntity target,
        byte oldValue,
        byte newValue);

    public delegate void GameEntityUInt16ChangeDelegate(
        BaseGameEntity target,
        ushort oldValue,
        ushort newValue);

    public delegate void GameEntityUInt32ChangeDelegate(
        BaseGameEntity target,
        uint oldValue,
        uint newValue);

    public delegate void GameEntityUInt64ChangeDelegate(
        BaseGameEntity target,
        ulong oldValue,
        ulong newValue);

    public delegate void GameEntitySingleChangeDelegate(
        BaseGameEntity target,
        float oldValue,
        float newValue);

    public delegate void GameEntityDoubleChangeDelegate(
        BaseGameEntity target,
        double oldValue,
        double newValue);

    public delegate void GameEntityBooleanChangeDelegate(
        BaseGameEntity target,
        bool oldValue,
        bool newValue);

    public delegate void GameEntityStringChangeDelegate(
        BaseGameEntity target,
        string oldValue,
        string newValue);
    #endregion

    #region Damageable Entity
    public delegate void DamageableEntityDelegate(
        DamageableEntity target);

    public delegate void DamageableEntityInt8ChangeDelegate(
        DamageableEntity target,
        sbyte oldValue,
        sbyte newValue);

    public delegate void DamageableEntityInt16ChangeDelegate(
        DamageableEntity target,
        short oldValue,
        short newValue);

    public delegate void DamageableEntityInt32ChangeDelegate(
        DamageableEntity target,
        int oldValue,
        int newValue);

    public delegate void DamageableEntityInt64ChangeDelegate(
        DamageableEntity target,
        long oldValue,
        long newValue);

    public delegate void DamageableEntityUInt8ChangeDelegate(
        DamageableEntity target,
        byte oldValue,
        byte newValue);

    public delegate void DamageableEntityUInt16ChangeDelegate(
        DamageableEntity target,
        ushort oldValue,
        ushort newValue);

    public delegate void DamageableEntityUInt32ChangeDelegate(
        DamageableEntity target,
        uint oldValue,
        uint newValue);

    public delegate void DamageableEntityUInt64ChangeDelegate(
        DamageableEntity target,
        ulong oldValue,
        ulong newValue);

    public delegate void DamageableEntitySingleChangeDelegate(
        DamageableEntity target,
        float oldValue,
        float newValue);

    public delegate void DamageableEntityDoubleChangeDelegate(
        DamageableEntity target,
        double oldValue,
        double newValue);

    public delegate void DamageableEntityBooleanChangeDelegate(
        DamageableEntity target,
        bool oldValue,
        bool newValue);

    public delegate void DamageableEntityStringChangeDelegate(
        DamageableEntity target,
        string oldValue,
        string newValue);
    #endregion

    #region Character Entity
    public delegate void CharacterEntityDelegate(
        BaseCharacterEntity target);

    public delegate void CharacterEntityInt8ChangeDelegate(
        BaseCharacterEntity target,
        sbyte oldValue,
        sbyte newValue);

    public delegate void CharacterEntityInt16ChangeDelegate(
        BaseCharacterEntity target,
        short oldValue,
        short newValue);

    public delegate void CharacterEntityInt32ChangeDelegate(
        BaseCharacterEntity target,
        int oldValue,
        int newValue);

    public delegate void CharacterEntityInt64ChangeDelegate(
        BaseCharacterEntity target,
        long oldValue,
        long newValue);

    public delegate void CharacterEntityUInt8ChangeDelegate(
        BaseCharacterEntity target,
        byte oldValue,
        byte newValue);

    public delegate void CharacterEntityUInt16ChangeDelegate(
        BaseCharacterEntity target,
        ushort oldValue,
        ushort newValue);

    public delegate void CharacterEntityUInt32ChangeDelegate(
        BaseCharacterEntity target,
        uint oldValue,
        uint newValue);

    public delegate void CharacterEntityUInt64ChangeDelegate(
        BaseCharacterEntity target,
        ulong oldValue,
        ulong newValue);

    public delegate void CharacterEntitySingleChangeDelegate(
        BaseCharacterEntity target,
        float oldValue,
        float newValue);

    public delegate void CharacterEntityDoubleChangeDelegate(
        BaseCharacterEntity target,
        double oldValue,
        double newValue);

    public delegate void CharacterEntityBooleanChangeDelegate(
        BaseCharacterEntity target,
        bool oldValue,
        bool newValue);

    public delegate void CharacterEntityStringChangeDelegate(
        BaseCharacterEntity target,
        string oldValue,
        string newValue);

    public delegate void CharacterEntityVector3ChangeDelegate(
        BaseCharacterEntity target,
        Vector3 oldValue,
        Vector3 newValue);

    public delegate void CharacterEntityAimPositionChangeDelegate(
        BaseCharacterEntity target,
        AimPosition oldValue,
        AimPosition newValue);

    public delegate void CharacterEntityMountChangeDelegate(
        BaseCharacterEntity target,
        CharacterMount oldValue,
        CharacterMount newValue);

    public delegate void CharacterEntitySummonerChangeDelegate(
        BaseCharacterEntity target,
        CharacterSummoner oldValue,
        CharacterSummoner newValue);
    #endregion
}
