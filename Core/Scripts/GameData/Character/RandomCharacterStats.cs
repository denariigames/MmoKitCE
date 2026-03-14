using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
#pragma warning disable CS0282 // There is no defined ordering between fields in multiple declarations of partial struct
    public partial struct RandomCharacterStats
#pragma warning restore CS0282 // There is no defined ordering between fields in multiple declarations of partial struct
    {
        [Header("HP")]
        public float minHp;
        public float maxHp;
        [Range(0, 1f)]
        public float hpApplyRate;

        [Header("HP Recovery")]
        public float minHpRecovery;
        public float maxHpRecovery;
        [Range(0, 1f)]
        public float hpRecoveryApplyRate;

        [Header("HP Leech Rate")]
        public float minHpLeechRate;
        public float maxHpLeechRate;
        [Range(0, 1f)]
        public float hpLeechRateApplyRate;

        [Header("MP")]
        public float minMp;
        public float maxMp;
        [Range(0, 1f)]
        public float mpApplyRate;

        [Header("MP Recovery")]
        public float minMpRecovery;
        public float maxMpRecovery;
        [Range(0, 1f)]
        public float mpRecoveryApplyRate;

        [Header("MP Leech Rate")]
        public float minMpLeechRate;
        public float maxMpLeechRate;
        [Range(0, 1f)]
        public float mpLeechRateApplyRate;

        [Header("Stamina")]
        public float minStamina;
        public float maxStamina;
        [Range(0, 1f)]
        public float staminaApplyRate;

        [Header("Stamina Recovery")]
        public float minStaminaRecovery;
        public float maxStaminaRecovery;
        [Range(0, 1f)]
        public float staminaRecoveryApplyRate;

        [Header("Stamina Leech Rate")]
        public float minStaminaLeechRate;
        public float maxStaminaLeechRate;
        [Range(0, 1f)]
        public float staminaLeechRateApplyRate;

        [Header("Food")]
        public float minFood;
        public float maxFood;
        [Range(0, 1f)]
        public float foodApplyRate;

        [Header("Water")]
        public float minWater;
        public float maxWater;
        [Range(0, 1f)]
        public float waterApplyRate;

        [Header("Accuracy")]
        public float minAccuracy;
        public float maxAccuracy;
        [Range(0, 1f)]
        public float accuracyApplyRate;

        [Header("Evasion")]
        public float minEvasion;
        public float maxEvasion;
        [Range(0, 1f)]
        public float evasionApplyRate;

        [Header("Cri Rate")]
        public float minCriRate;
        public float maxCriRate;
        [Range(0, 1f)]
        public float criRateApplyRate;

        [Header("Cri Dmg Rate")]
        public float minCriDmgRate;
        public float maxCriDmgRate;
        [Range(0, 1f)]
        public float criDmgRateApplyRate;

        [Header("Block Rate")]
        public float minBlockRate;
        public float maxBlockRate;
        [Range(0, 1f)]
        public float blockRateApplyRate;

        [Header("Block Dmg Rate")]
        public float minBlockDmgRate;
        public float maxBlockDmgRate;
        [Range(0, 1f)]
        public float blockDmgRateApplyRate;

        [Header("Move Speed")]
        public float minMoveSpeed;
        public float maxMoveSpeed;
        [Range(0, 1f)]
        public float moveSpeedApplyRate;

        [Header("Atk Speed")]
        public float minAtkSpeed;
        public float maxAtkSpeed;
        [Range(0, 1f)]
        public float atkSpeedApplyRate;

        [Header("Weight Limit")]
        public float minWeightLimit;
        public float maxWeightLimit;
        [Range(0, 1f)]
        public float weightLimitApplyRate;

        [Header("Slot Limit")]
        public float minSlotLimit;
        public float maxSlotLimit;
        [Range(0, 1f)]
        public float slotLimitApplyRate;

        [Header("Gold Rate")]
        public float minGoldRate;
        public float maxGoldRate;
        [Range(0, 1f)]
        public float goldRateApplyRate;

        [Header("Exp Rate")]
        public float minExpRate;
        public float maxExpRate;
        [Range(0, 1f)]
        public float expRateApplyRate;

        [Header("Item Drop Rate")]
        public float minItemDropRate;
        public float maxItemDropRate;
        [Range(0, 1f)]
        public float itemDropRateApplyRate;

        [Header("Jump Height")]
        public float minJumpHeight;
        public float maxJumpHeight;
        [Range(0, 1f)]
        public float jumpHeightApplyRate;

        [Header("Head Damage Absorbs")]
        public float minHeadDamageAbsorbs;
        public float maxHeadDamageAbsorbs;
        [Range(0, 1f)]
        public float headDamageAbsorbsApplyRate;

        [Header("Body Damage Absorbs")]
        public float minBodyDamageAbsorbs;
        public float maxBodyDamageAbsorbs;
        [Range(0, 1f)]
        public float bodyDamageAbsorbsApplyRate;

        [Header("Fall Damage Absorbs")]
        public float minFallDamageAbsorbs;
        public float maxFallDamageAbsorbs;
        [Range(0, 1f)]
        public float fallDamageAbsorbsApplyRate;

        [Header("Gravity Rate")]
        public float minGravityRate;
        public float maxGravityRate;
        [Range(0, 1f)]
        public float gravityRateApplyRate;

        public float GetRandomedHp(System.Random random)
        {
            return random.RandomFloat(minHp, maxHp);
        }

        public float GetRandomedHpRecovery(System.Random random)
        {
            return random.RandomFloat(minHpRecovery, maxHpRecovery);
        }

        public float GetRandomedHpLeechRate(System.Random random)
        {
            return random.RandomFloat(minHpLeechRate, maxHpLeechRate);
        }

        public float GetRandomedMp(System.Random random)
        {
            return random.RandomFloat(minMp, maxMp);
        }

        public float GetRandomedMpRecovery(System.Random random)
        {
            return random.RandomFloat(minMpRecovery, maxMpRecovery);
        }

        public float GetRandomedMpLeechRate(System.Random random)
        {
            return random.RandomFloat(minMpLeechRate, maxMpLeechRate);
        }

        public float GetRandomedStamina(System.Random random)
        {
            return random.RandomFloat(minStamina, maxStamina);
        }

        public float GetRandomedStaminaRecovery(System.Random random)
        {
            return random.RandomFloat(minStaminaRecovery, maxStaminaRecovery);
        }

        public float GetRandomedStaminaLeechRate(System.Random random)
        {
            return random.RandomFloat(minStaminaLeechRate, maxStaminaLeechRate);
        }

        public float GetRandomedFood(System.Random random)
        {
            return random.RandomFloat(minFood, maxFood);
        }

        public float GetRandomedWater(System.Random random)
        {
            return random.RandomFloat(minWater, maxWater);
        }

        public float GetRandomedAccuracy(System.Random random)
        {
            return random.RandomFloat(minAccuracy, maxAccuracy);
        }

        public float GetRandomedEvasion(System.Random random)
        {
            return random.RandomFloat(minEvasion, maxEvasion);
        }

        public float GetRandomedCriRate(System.Random random)
        {
            return random.RandomFloat(minCriRate, maxCriRate);
        }

        public float GetRandomedCriDmgRate(System.Random random)
        {
            return random.RandomFloat(minCriDmgRate, maxCriDmgRate);
        }

        public float GetRandomedBlockRate(System.Random random)
        {
            return random.RandomFloat(minBlockRate, maxBlockRate);
        }

        public float GetRandomedBlockDmgRate(System.Random random)
        {
            return random.RandomFloat(minBlockDmgRate, maxBlockDmgRate);
        }

        public float GetRandomedMoveSpeed(System.Random random)
        {
            return random.RandomFloat(minMoveSpeed, maxMoveSpeed);
        }

        public float GetRandomedAtkSpeed(System.Random random)
        {
            return random.RandomFloat(minAtkSpeed, maxAtkSpeed);
        }

        public float GetRandomedWeightLimit(System.Random random)
        {
            return random.RandomFloat(minWeightLimit, maxWeightLimit);
        }

        public float GetRandomedSlotLimit(System.Random random)
        {
            return random.RandomFloat(minSlotLimit, maxSlotLimit);
        }

        public float GetRandomedGoldRate(System.Random random)
        {
            return random.RandomFloat(minGoldRate, maxGoldRate);
        }

        public float GetRandomedExpRate(System.Random random)
        {
            return random.RandomFloat(minExpRate, maxExpRate);
        }

        public float GetRandomedItemDropRate(System.Random random)
        {
            return random.RandomFloat(minItemDropRate, maxItemDropRate);
        }

        public float GetRandomedJumpHeight(System.Random random)
        {
            return random.RandomFloat(minJumpHeight, maxJumpHeight);
        }

        public float GetRandomedHeadDamageAbsorbs(System.Random random)
        {
            return random.RandomFloat(minHeadDamageAbsorbs, maxHeadDamageAbsorbs);
        }

        public float GetRandomedBodyDamageAbsorbs(System.Random random)
        {
            return random.RandomFloat(minBodyDamageAbsorbs, maxBodyDamageAbsorbs);
        }

        public float GetRandomedFallDamageAbsorbs(System.Random random)
        {
            return random.RandomFloat(minFallDamageAbsorbs, maxFallDamageAbsorbs);
        }

        public float GetRandomedGravityRate(System.Random random)
        {
            return random.RandomFloat(minGravityRate, maxGravityRate);
        }

        public bool ApplyHp(System.Random random)
        {
            return random.NextDouble() <= hpApplyRate;
        }

        public bool ApplyHpRecovery(System.Random random)
        {
            return random.NextDouble() <= hpRecoveryApplyRate;
        }

        public bool ApplyHpLeechRate(System.Random random)
        {
            return random.NextDouble() <= hpLeechRateApplyRate;
        }

        public bool ApplyMp(System.Random random)
        {
            return random.NextDouble() <= mpApplyRate;
        }

        public bool ApplyMpRecovery(System.Random random)
        {
            return random.NextDouble() <= mpRecoveryApplyRate;
        }

        public bool ApplyMpLeechRate(System.Random random)
        {
            return random.NextDouble() <= mpLeechRateApplyRate;
        }

        public bool ApplyStamina(System.Random random)
        {
            return random.NextDouble() <= staminaApplyRate;
        }

        public bool ApplyStaminaRecovery(System.Random random)
        {
            return random.NextDouble() <= staminaRecoveryApplyRate;
        }

        public bool ApplyStaminaLeechRate(System.Random random)
        {
            return random.NextDouble() <= staminaLeechRateApplyRate;
        }

        public bool ApplyFood(System.Random random)
        {
            return random.NextDouble() <= foodApplyRate;
        }

        public bool ApplyWater(System.Random random)
        {
            return random.NextDouble() <= waterApplyRate;
        }

        public bool ApplyAccuracy(System.Random random)
        {
            return random.NextDouble() <= accuracyApplyRate;
        }

        public bool ApplyEvasion(System.Random random)
        {
            return random.NextDouble() <= evasionApplyRate;
        }

        public bool ApplyCriRate(System.Random random)
        {
            return random.NextDouble() <= criRateApplyRate;
        }

        public bool ApplyCriDmgRate(System.Random random)
        {
            return random.NextDouble() <= criDmgRateApplyRate;
        }

        public bool ApplyBlockRate(System.Random random)
        {
            return random.NextDouble() <= blockRateApplyRate;
        }

        public bool ApplyBlockDmgRate(System.Random random)
        {
            return random.NextDouble() <= blockDmgRateApplyRate;
        }

        public bool ApplyMoveSpeed(System.Random random)
        {
            return random.NextDouble() <= moveSpeedApplyRate;
        }

        public bool ApplyAtkSpeed(System.Random random)
        {
            return random.NextDouble() <= atkSpeedApplyRate;
        }

        public bool ApplyWeightLimit(System.Random random)
        {
            return random.NextDouble() <= weightLimitApplyRate;
        }

        public bool ApplySlotLimit(System.Random random)
        {
            return random.NextDouble() <= slotLimitApplyRate;
        }

        public bool ApplyGoldRate(System.Random random)
        {
            return random.NextDouble() <= goldRateApplyRate;
        }

        public bool ApplyExpRate(System.Random random)
        {
            return random.NextDouble() <= expRateApplyRate;
        }

        public bool ApplyItemDropRate(System.Random random)
        {
            return random.NextDouble() <= itemDropRateApplyRate;
        }

        public bool ApplyJumpHeight(System.Random random)
        {
            return random.NextDouble() <= jumpHeightApplyRate;
        }

        public bool ApplyHeadDamageAbsorbs(System.Random random)
        {
            return random.NextDouble() <= headDamageAbsorbsApplyRate;
        }

        public bool ApplyBodyDamageAbsorbs(System.Random random)
        {
            return random.NextDouble() <= bodyDamageAbsorbsApplyRate;
        }

        public bool ApplyFallDamageAbsorbs(System.Random random)
        {
            return random.NextDouble() <= fallDamageAbsorbsApplyRate;
        }

        public bool ApplyGravityRate(System.Random random)
        {
            return random.NextDouble() <= gravityRateApplyRate;
        }
    }
}
