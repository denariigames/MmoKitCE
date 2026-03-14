namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        public override void Clean()
        {
            base.Clean();
            meleeDamageTransform = null;
            missileDamageTransform = null;
            characterUiTransform = null;
            miniMapUiTransform = null;
            chatBubbleTransform = null;
            race = null;
            if (UICharacterEntity != null)
                Destroy(UICharacterEntity.gameObject);
            UICharacterEntity = null;
            if (UIChatBubble != null)
                Destroy(UIChatBubble.gameObject);
            UIChatBubble = null;
            AttackComponent = null;
            UseSkillComponent = null;
            ReloadComponent = null;
            ChargeComponent = null;
            RecoveryComponent = null;
            SkillAndBuffComponent = null;
            AttackPhysicFunctions = null;
            FindPhysicFunctions = null;
            ModelManager = null;
            // Caches
            this.RemoveCaches();
            CachedData = null;
            // Buff Functions
            _restrictBuffTags?.Clear();
            // Events
            onDead?.RemoveAllListeners();
            onDead = null;
            onRespawn?.RemoveAllListeners();
            onRespawn = null;
            onLevelUp?.RemoveAllListeners();
            onLevelUp = null;
            onRecached = null;
            onIdChange = null;
            onCharacterNameChange = null;
            onLevelChange = null;
            onExpChange = null;
            onIsInvincibleChange = null;
            onCurrentMpChange = null;
            onCurrentStaminaChange = null;
            onCurrentFoodChange = null;
            onCurrentWaterChange = null;
            onEquipWeaponSetChange = null;
            onIsWeaponsSheathedChange = null;
            onPitchChange = null;
            onAimPositionChange = null;
            onTargetEntityIdChange = null;
            onSelectableWeaponSetsOperation = null;
            onAttributesOperation = null;
            onSkillsOperation = null;
            onSkillUsagesOperation = null;
            onBuffsOperation = null;
            onEquipItemsOperation = null;
            onNonEquipItemsOperation = null;
            onSummonsOperation = null;
            onAttackRoutine = null;
            onUseSkillRoutine = null;
            onLaunchDamageEntity = null;
            onApplyBuff = null;
            onRemoveBuff = null;
            onBuffHpRecovery = null;
            onBuffHpDecrease = null;
            onBuffMpRecovery = null;
            onBuffMpDecrease = null;
            onBuffStaminaRecovery = null;
            onBuffStaminaDecrease = null;
            onBuffFoodRecovery = null;
            onBuffFoodDecrease = null;
            onBuffWaterRecovery = null;
            onBuffWaterDecrease = null;
            onNotifyEnemySpotted = null;
            onNotifyEnemySpottedByAlly = null;
        }
    }
}
