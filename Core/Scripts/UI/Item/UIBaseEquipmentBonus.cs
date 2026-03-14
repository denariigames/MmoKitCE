using Cysharp.Text;
using Insthync.DevExtension;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract partial class UIBaseEquipmentBonus<T> : UISelectionEntry<T>
    {
        [Header("String Formats (Stats)")]
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyHpStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HP);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyHpRecoveryStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HP_RECOVERY);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyHpLeechRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HP_LEECH_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyMpStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MP);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyMpRecoveryStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MP_RECOVERY);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyMpLeechRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MP_LEECH_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyStaminaStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STAMINA);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyStaminaRecoveryStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STAMINA_RECOVERY);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyStaminaLeechRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STAMINA_LEECH_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyFoodStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FOOD);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyWaterStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_WATER);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyAccuracyStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ACCURACY);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyEvasionStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EVASION);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyCriRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CRITICAL_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyCriDmgRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CRITICAL_DAMAGE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBlockRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BLOCK_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBlockDmgRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BLOCK_DAMAGE_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyMoveSpeedStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MOVE_SPEED);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeySprintSpeedStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SPRINT_SPEED);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyAtkSpeedStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ATTACK_SPEED);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyWeightLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_WEIGHT);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeySlotLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SLOT);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyGoldRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyExpRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EXP_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyItemDropRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_DROP_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyJumpHeightStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_JUMP_HEIGHT);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyHeadDamageAbsorbsStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HEAD_DAMAGE_ABSORBS);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBodyDamageAbsorbsStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BODY_DAMAGE_ABSORBS);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyFallDamageAbsorbsStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FALL_DAMAGE_ABSORBS);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyGravityRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GRAVITY_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyProtectedSlotLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_PROTECTED_SLOT_LIMIT);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyAmmoCapacityStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_AMMO_CAPACITY);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyRecoilModifierStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RECOIL_MODIFIER);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyRecoilRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RECOIL_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyRateOfFireStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RATE_OF_FIRE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyReloadDurationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RELOAD_DURATION);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyFireSpreadRangeRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FIRE_SPREAD_RANGE_RATE);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyFireSpreadStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FIRE_SPREAD);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyDecreaseFoodDecreationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DECREASE_FOOD_DECREATION);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyDecreaseWaterDecreationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DECREASE_WATER_DECREATION);
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyDecreaseStaminaDecreationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DECREASE_STAMINA_DECREATION);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBuyItemPriceRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUY_ITEM_PRICE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeySellItemPriceRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_ITEM_PRICE_RATE);

        [Header("String Formats (Stats Rate)")]
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyHpRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HP_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyHpRecoveryRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HP_RECOVERY_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyHpLeechRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HP_LEECH_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyMpRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MP_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyMpRecoveryRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MP_RECOVERY_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyMpLeechRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MP_LEECH_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyStaminaRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STAMINA_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyStaminaRecoveryRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STAMINA_RECOVERY_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyStaminaLeechRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_STAMINA_LEECH_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyFoodRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FOOD_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyWaterRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_WATER_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyAccuracyRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ACCURACY_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyEvasionRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EVASION_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyCriRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CRITICAL_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyCriDmgRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CRITICAL_DAMAGE_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBlockRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BLOCK_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBlockDmgRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BLOCK_DAMAGE_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyMoveSpeedRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MOVE_SPEED_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeySprintSpeedRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SPRINT_SPEED_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyAtkSpeedRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ATTACK_SPEED_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyWeightLimitRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_WEIGHT_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeySlotLimitRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SLOT_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyGoldRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyExpRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EXP_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyItemDropRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ITEM_DROP_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyJumpHeightRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_JUMP_HEIGHT_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyHeadDamageAbsorbsRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_HEAD_DAMAGE_ABSORBS_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBodyDamageAbsorbsRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BODY_DAMAGE_ABSORBS_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyFallDamageAbsorbsRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FALL_DAMAGE_ABSORBS_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyGravityRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GRAVITY_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyProtectedSlotLimitRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_PROTECTED_SLOT_LIMIT_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyAmmoCapacityRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_AMMO_CAPACITY_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyRecoilModifierRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RECOIL_MODIFIER_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyRecoilRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RECOIL_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyRateOfFireRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RATE_OF_FIRE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyReloadDurationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RELOAD_DURATION_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyFireSpreadRangeRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FIRE_SPREAD_RANGE_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyFireSpreadRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FIRE_SPREAD_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyDecreaseFoodDecreationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DECREASE_FOOD_DECREATION_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyDecreaseWaterDecreationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DECREASE_WATER_DECREATION_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyDecreaseStaminaDecreationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DECREASE_STAMINA_DECREATION_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyBuyItemPriceRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_BUY_ITEM_PRICE_RATE_RATE);
        [Tooltip("Format => {0} = {Amount * 100}")]
        public UILocaleKeySetting formatKeySellItemPriceRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_ITEM_PRICE_RATE_RATE);

        [Header("String Formats (Attribute/Damage Element/Skill)")]
        [Tooltip("Format => {0} = {Attribute Title}, {1} = {Amount}")]
        public UILocaleKeySetting formatKeyAttributeAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ATTRIBUTE_AMOUNT);
        [Tooltip("Format => {0} = {Attribute Title}, {1} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyAttributeAmountRate = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ATTRIBUTE_RATE);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Amount * 100}")]
        public UILocaleKeySetting formatKeyResistanceAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_RESISTANCE_AMOUNT);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Min Damage}, {2} = {Max Damage}")]
        public UILocaleKeySetting formatKeyDamageAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_WITH_ELEMENTAL);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Min Damage * 100}, {2} = {Max Damage * 100}")]
        public UILocaleKeySetting formatKeyDamageAmountRate = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_WITH_ELEMENTAL_RATE);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Target Amount}")]
        public UILocaleKeySetting formatKeyArmorAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ARMOR_AMOUNT);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Target Amount * 100}")]
        public UILocaleKeySetting formatKeyArmorAmountRate = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_ARMOR_RATE);
        [Tooltip("Format => {0} = {Skill Title}, {1} = {Level}")]
        public UILocaleKeySetting formatKeySkillLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SKILL_LEVEL);

        [Header("UI Elements")]
        public TextWrapper uiTextAllBonus;
        public Color bonusIncreaseColor = Color.green;
        public Color bonusDecreaseColor = Color.red;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextAllBonus = null;
        }

        public string GetEquipmentBonusText(EquipmentBonus equipmentBonus)
        {
            using (Utf16ValueStringBuilder result = ZString.CreateStringBuilder(false))
            {
                CharacterStatsTextGenerateData generateTextData;
                // Dev Extension
                // How to implement it?:
                // /*
                //  * - Add `customStat1` to `CharacterStats` partial class file
                //  * - Add `customStat1StatsFormat` to `CharacterStatsTextGenerateData`
                //  * - Add `uiTextCustomStat1` to `CharacterStatsTextGenerateData`
                //  * - Add `formatKeyCustomStat1Stats` to `UIBaseEquipmentBonus` partial class file
                //  * - Add `formatKeyCustomStat1RateStats` to `UIBaseEquipmentBonus` partial class file
                //  * - Add `uiTextCustomStat1` to `UIBaseEquipmentBonus`
                //  */
                // [DevExtMethods("SetStatsGenerateTextData")]
                // public void SetStatsGenerateTextData_Ext(CharacterStatsTextGenerateData generateTextData)
                // {
                //   generateTextData.customStat1StatsFormat = formatKeyCustomStat1Stats;
                //   generateTextData.uiTextCustomStat1 = uiTextCustomStat1;
                // }
                // [DevExtMethods("SetRateStatsGenerateTextData")]
                // public void SetRateStatsGenerateTextData_Ext(CharacterStatsTextGenerateData generateTextData)
                // {
                //   generateTextData.customStat1StatsFormat = formatKeyCustomStat1RateStats;
                //   generateTextData.uiTextCustomStat1 = uiTextCustomStat1;
                // }
                // Rate stats
                generateTextData = new CharacterStatsTextGenerateData()
                {
                    data = equipmentBonus.StatsRate,
                    isRate = true,
                    isBonus = true,
                    hpStatsFormat = formatKeyHpRateStats,
                    hpRecoveryStatsFormat = formatKeyHpRecoveryRateStats,
                    hpLeechRateStatsFormat = formatKeyHpLeechRateRateStats,
                    mpStatsFormat = formatKeyMpRateStats,
                    mpRecoveryStatsFormat = formatKeyMpRecoveryRateStats,
                    mpLeechRateStatsFormat = formatKeyMpLeechRateRateStats,
                    staminaStatsFormat = formatKeyStaminaRateStats,
                    staminaRecoveryStatsFormat = formatKeyStaminaRecoveryRateStats,
                    staminaLeechRateStatsFormat = formatKeyStaminaLeechRateRateStats,
                    foodStatsFormat = formatKeyFoodRateStats,
                    waterStatsFormat = formatKeyWaterRateStats,
                    accuracyStatsFormat = formatKeyAccuracyRateStats,
                    evasionStatsFormat = formatKeyEvasionRateStats,
                    criRateStatsFormat = formatKeyCriRateRateStats,
                    criDmgRateStatsFormat = formatKeyCriDmgRateRateStats,
                    blockRateStatsFormat = formatKeyBlockRateRateStats,
                    blockDmgRateStatsFormat = formatKeyBlockDmgRateRateStats,
                    moveSpeedStatsFormat = formatKeyMoveSpeedRateStats,
                    sprintSpeedStatsFormat = formatKeySprintSpeedRateStats,
                    atkSpeedStatsFormat = formatKeyAtkSpeedRateStats,
                    weightLimitStatsFormat = formatKeyWeightLimitRateStats,
                    slotLimitStatsFormat = formatKeySlotLimitRateStats,
                    goldRateStatsFormat = formatKeyGoldRateRateStats,
                    expRateStatsFormat = formatKeyExpRateRateStats,
                    itemDropRateStatsFormat = formatKeyItemDropRateRateStats,
                    jumpHeightStatsFormat = formatKeyJumpHeightRateStats,
                    headDamageAbsorbsStatsFormat = formatKeyHeadDamageAbsorbsRateStats,
                    bodyDamageAbsorbsStatsFormat = formatKeyBodyDamageAbsorbsRateStats,
                    fallDamageAbsorbsStatsFormat = formatKeyFallDamageAbsorbsRateStats,
                    gravityRateStatsFormat = formatKeyGravityRateRateStats,
                    protectedSlotLimitFormat = formatKeyProtectedSlotLimitRateStats,
                    ammoCapacityFormat = formatKeyAmmoCapacityRateStats,
                    recoilModifierFormat = formatKeyRecoilModifierRateStats,
                    recoilRateFormat = formatKeyRecoilRateRateStats,
                    rateOfFireFormat = formatKeyRateOfFireRateStats,
                    reloadDurationFormat = formatKeyReloadDurationRateStats,
                    fireSpreadRangeRateFormat = formatKeyFireSpreadRangeRateRateStats,
                    fireSpreadFormat = formatKeyFireSpreadRateStats,
                    decreaseFoodDecreationFormat = formatKeyDecreaseFoodDecreationRateStats,
                    decreaseWaterDecreationFormat = formatKeyDecreaseWaterDecreationRateStats,
                    decreaseStaminaDecreationFormat = formatKeyDecreaseStaminaDecreationRateStats,
                    buyItemPriceRateFormat = formatKeyBuyItemPriceRateRateStats,
                    sellItemPriceRateFormat = formatKeySellItemPriceRateRateStats,
                };
                this.InvokeInstanceDevExtMethods("SetRateStatsGenerateTextData", generateTextData);
                string rateStatsText = generateTextData.GetText(bonusIncreaseColor, bonusDecreaseColor);

                // Non-rate stats
                generateTextData = new CharacterStatsTextGenerateData()
                {
                    data = equipmentBonus.Stats,
                    isRate = false,
                    isBonus = true,
                    hpStatsFormat = formatKeyHpStats,
                    hpRecoveryStatsFormat = formatKeyHpRecoveryStats,
                    hpLeechRateStatsFormat = formatKeyHpLeechRateStats,
                    mpStatsFormat = formatKeyMpStats,
                    mpRecoveryStatsFormat = formatKeyMpRecoveryStats,
                    mpLeechRateStatsFormat = formatKeyMpLeechRateStats,
                    staminaStatsFormat = formatKeyStaminaStats,
                    staminaRecoveryStatsFormat = formatKeyStaminaRecoveryStats,
                    staminaLeechRateStatsFormat = formatKeyStaminaLeechRateStats,
                    foodStatsFormat = formatKeyFoodStats,
                    waterStatsFormat = formatKeyWaterStats,
                    accuracyStatsFormat = formatKeyAccuracyStats,
                    evasionStatsFormat = formatKeyEvasionStats,
                    criRateStatsFormat = formatKeyCriRateStats,
                    criDmgRateStatsFormat = formatKeyCriDmgRateStats,
                    blockRateStatsFormat = formatKeyBlockRateStats,
                    blockDmgRateStatsFormat = formatKeyBlockDmgRateStats,
                    moveSpeedStatsFormat = formatKeyMoveSpeedStats,
                    sprintSpeedStatsFormat = formatKeySprintSpeedStats,
                    atkSpeedStatsFormat = formatKeyAtkSpeedStats,
                    weightLimitStatsFormat = formatKeyWeightLimitStats,
                    slotLimitStatsFormat = formatKeySlotLimitStats,
                    goldRateStatsFormat = formatKeyGoldRateStats,
                    expRateStatsFormat = formatKeyExpRateStats,
                    itemDropRateStatsFormat = formatKeyItemDropRateStats,
                    jumpHeightStatsFormat = formatKeyJumpHeightStats,
                    headDamageAbsorbsStatsFormat = formatKeyHeadDamageAbsorbsStats,
                    bodyDamageAbsorbsStatsFormat = formatKeyBodyDamageAbsorbsStats,
                    fallDamageAbsorbsStatsFormat = formatKeyFallDamageAbsorbsStats,
                    gravityRateStatsFormat = formatKeyGravityRateStats,
                    protectedSlotLimitFormat = formatKeyProtectedSlotLimitStats,
                    ammoCapacityFormat = formatKeyAmmoCapacityStats,
                    recoilModifierFormat = formatKeyRecoilModifierStats,
                    recoilRateFormat = formatKeyRecoilRateStats,
                    rateOfFireFormat = formatKeyRateOfFireStats,
                    reloadDurationFormat = formatKeyReloadDurationStats,
                    fireSpreadRangeRateFormat = formatKeyFireSpreadRangeRateStats,
                    fireSpreadFormat = formatKeyFireSpreadStats,
                    decreaseFoodDecreationFormat = formatKeyDecreaseFoodDecreationStats,
                    decreaseWaterDecreationFormat = formatKeyDecreaseWaterDecreationStats,
                    decreaseStaminaDecreationFormat = formatKeyDecreaseStaminaDecreationStats,
                    buyItemPriceRateFormat = formatKeyBuyItemPriceRateStats,
                    sellItemPriceRateFormat = formatKeySellItemPriceRateStats,
                };
                this.InvokeInstanceDevExtMethods("SetStatsGenerateTextData", generateTextData);
                string statsText = generateTextData.GetText(bonusIncreaseColor, bonusDecreaseColor);

                if (!string.IsNullOrEmpty(statsText))
                    result.Append(statsText);

                if (!string.IsNullOrEmpty(rateStatsText))
                {
                    if (result.Length > 0)
                        result.Append('\n');
                    result.Append(rateStatsText);
                }

                // Attributes
                foreach (KeyValuePair<Attribute, float> entry in equipmentBonus.Attributes)
                {
                    if (entry.Key == null || entry.Value == 0)
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeyAttributeAmount),
                        entry.Key.Title,
                        entry.Value.ToBonusString("N0"));
                }
                foreach (KeyValuePair<Attribute, float> entry in equipmentBonus.AttributesRate)
                {
                    if (entry.Key == null || entry.Value == 0)
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeyAttributeAmountRate),
                        entry.Key.Title,
                        (entry.Value * 100).ToBonusString("N2"));
                }

                // Resistances
                foreach (KeyValuePair<DamageElement, float> entry in equipmentBonus.Resistances)
                {
                    if (entry.Key == null || entry.Value == 0)
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeyResistanceAmount),
                        entry.Key.Title,
                        (entry.Value * 100).ToBonusString("N2"));
                }

                // Damages
                foreach (KeyValuePair<DamageElement, MinMaxFloat> entry in equipmentBonus.Damages)
                {
                    if (entry.Key == null || (entry.Value.min == 0 && entry.Value.max == 0))
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeyDamageAmount),
                        entry.Key.Title,
                        entry.Value.min.ToBonusString("N0"),
                        entry.Value.max.ToString("N0"));
                }
                foreach (KeyValuePair<DamageElement, MinMaxFloat> entry in equipmentBonus.DamagesRate)
                {
                    if (entry.Key == null || (entry.Value.min == 0 && entry.Value.max == 0))
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeyDamageAmountRate),
                        entry.Key.Title,
                        (entry.Value.min * 100).ToBonusString("N2"),
                        (entry.Value.max * 100).ToString("N2"));
                }

                // Armors
                foreach (KeyValuePair<DamageElement, float> entry in equipmentBonus.Armors)
                {
                    if (entry.Value == 0)
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeyArmorAmount),
                        entry.Key.Title,
                        entry.Value.ToBonusString("N0"));
                }
                foreach (KeyValuePair<DamageElement, float> entry in equipmentBonus.ArmorsRate)
                {
                    if (entry.Value == 0)
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeyArmorAmountRate),
                        entry.Key.Title,
                        (entry.Value * 100).ToBonusString("N2"));
                }

                // Skills
                foreach (KeyValuePair<BaseSkill, int> entry in equipmentBonus.Skills)
                {
                    if (entry.Key == null || entry.Value == 0)
                        continue;
                    if (result.Length > 0)
                        result.Append('\n');
                    result.AppendFormat(
                        LanguageManager.GetText(formatKeySkillLevel),
                        entry.Key.Title,
                        entry.Value.ToBonusString("N0"));
                }

                return result.ToString();
            }
        }
    }
}
