using Insthync.DevExtension;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public partial class UICharacterStats : UISelectionEntry<CharacterStats>
    {
        public enum DisplayType
        {
            Simple,
            Rate
        }

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

        [Header("UI Elements")]
        public TextWrapper uiTextStats;
        public TextWrapper uiTextHp;
        public TextWrapper uiTextHpRecovery;
        public TextWrapper uiTextHpLeechRate;
        public TextWrapper uiTextMp;
        public TextWrapper uiTextMpRecovery;
        public TextWrapper uiTextMpLeechRate;
        public TextWrapper uiTextStamina;
        public TextWrapper uiTextStaminaRecovery;
        public TextWrapper uiTextStaminaLeechRate;
        public TextWrapper uiTextFood;
        public TextWrapper uiTextWater;
        public TextWrapper uiTextAccuracy;
        public TextWrapper uiTextEvasion;
        public TextWrapper uiTextCriRate;
        public TextWrapper uiTextCriDmgRate;
        public TextWrapper uiTextBlockRate;
        public TextWrapper uiTextBlockDmgRate;
        public TextWrapper uiTextMoveSpeed;
        public TextWrapper uiTextSprintSpeed;
        public TextWrapper uiTextAtkSpeed;
        public TextWrapper uiTextWeightLimit;
        public TextWrapper uiTextSlotLimit;
        public TextWrapper uiTextGoldRate;
        public TextWrapper uiTextExpRate;
        public TextWrapper uiTextItemDropRate;
        public TextWrapper uiTextJumpHeight;
        public TextWrapper uiTextHeadDamageAbsorbs;
        public TextWrapper uiTextBodyDamageAbsorbs;
        public TextWrapper uiTextFallDamageAbsorbs;
        public TextWrapper uiTextGravityRate;
        public TextWrapper uiTextProtectedSlotLimit;
        public TextWrapper uiTextAmmoCapacity;
        public TextWrapper uiTextRecoilModifier;
        public TextWrapper uiTextRecoilRate;
        public TextWrapper uiTextRateOfFire;
        public TextWrapper uiTextReloadDuration;
        public TextWrapper uiTextFireSpreadRangeRate;
        public TextWrapper uiTextFireSpread;
        public TextWrapper uiTextNightVision;
        public TextWrapper uiTextFlashLight;
        public TextWrapper uiTextDecreaseFoodDecreation;
        public TextWrapper uiTextDecreaseWaterDecreation;
        public TextWrapper uiTextDecreaseStaminaDecreation;
        public TextWrapper uiTextIncreaseDamageType1;
        public TextWrapper uiTextIncreaseDamageType2;
        public TextWrapper uiTextIncreaseDamageType3;
        public TextWrapper uiTextIncreaseDamageType4;
        public TextWrapper uiTextAtkSpeedType1;
        public TextWrapper uiTextAtkSpeedType2;
        public TextWrapper uiTextAtkSpeedType3;
        public TextWrapper uiTextAtkSpeedType4;
        public TextWrapper uiTextBuyItemPriceRate;
        public TextWrapper uiTextSellItemPriceRate;
        public DisplayType displayType;
        public bool isBonus;
        public Color bonusIncreaseColor = Color.green;
        public Color bonusDecreaseColor = Color.red;

        [Header("Options")]
        public string numberFormatSimple = "N0";
        public string numberFormatRate = "N2";

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextStats = null;
            uiTextHp = null;
            uiTextHpRecovery = null;
            uiTextHpLeechRate = null;
            uiTextMp = null;
            uiTextMpRecovery = null;
            uiTextMpLeechRate = null;
            uiTextStamina = null;
            uiTextStaminaRecovery = null;
            uiTextStaminaLeechRate = null;
            uiTextFood = null;
            uiTextWater = null;
            uiTextAccuracy = null;
            uiTextEvasion = null;
            uiTextCriRate = null;
            uiTextCriDmgRate = null;
            uiTextBlockRate = null;
            uiTextBlockDmgRate = null;
            uiTextMoveSpeed = null;
            uiTextSprintSpeed = null;
            uiTextAtkSpeed = null;
            uiTextWeightLimit = null;
            uiTextSlotLimit = null;
            uiTextGoldRate = null;
            uiTextExpRate = null;
            uiTextItemDropRate = null;
            uiTextJumpHeight = null;
            uiTextHeadDamageAbsorbs = null;
            uiTextBodyDamageAbsorbs = null;
            uiTextFallDamageAbsorbs = null;
            uiTextGravityRate = null;
            uiTextProtectedSlotLimit = null;
            uiTextAmmoCapacity = null;
            uiTextRecoilModifier = null;
            uiTextRecoilRate = null;
            uiTextRateOfFire = null;
            uiTextReloadDuration = null;
            uiTextFireSpreadRangeRate = null;
            uiTextFireSpread = null;
            uiTextNightVision = null;
            uiTextFlashLight = null;
            uiTextDecreaseFoodDecreation = null;
            uiTextDecreaseWaterDecreation = null;
            uiTextDecreaseStaminaDecreation = null;
            uiTextIncreaseDamageType1 = null;
            uiTextIncreaseDamageType2 = null;
            uiTextIncreaseDamageType3 = null;
            uiTextIncreaseDamageType4 = null;
            uiTextAtkSpeedType1 = null;
            uiTextAtkSpeedType2 = null;
            uiTextAtkSpeedType3 = null;
            uiTextAtkSpeedType4 = null;
            uiTextBuyItemPriceRate = null;
            uiTextSellItemPriceRate = null;
        }

        protected override void UpdateData()
        {
            CharacterStatsTextGenerateData generateTextData;
            string statsString;

            // Dev Extension
            // How to implement it?:
            // /*
            //  * - Add `customStat1` to `CharacterStats` partial class file
            //  * - Add `customStat1StatsFormat` to `CharacterStatsTextGenerateData`
            //  * - Add `uiTextCustomStat1` to `CharacterStatsTextGenerateData`
            //  * - Add `formatKeyCustomStat1Stats` to `UICharacterStats` partial class file
            //  * - Add `formatKeyCustomStat1RateStats` to `UICharacterStats` partial class file
            //  * - Add `uiTextCustomStat1` to `UICharacterStats`
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
            switch (displayType)
            {
                case DisplayType.Rate:
                    generateTextData = new CharacterStatsTextGenerateData()
                    {
                        data = Data,
                        isRate = true,
                        isBonus = isBonus,
                        numberFormatSimple = numberFormatSimple,
                        numberFormatRate = numberFormatRate,
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
                        uiTextHp = uiTextHp,
                        uiTextHpRecovery = uiTextHpRecovery,
                        uiTextHpLeechRate = uiTextHpLeechRate,
                        uiTextMp = uiTextMp,
                        uiTextMpRecovery = uiTextMpRecovery,
                        uiTextMpLeechRate = uiTextMpLeechRate,
                        uiTextStamina = uiTextStamina,
                        uiTextStaminaRecovery = uiTextStaminaRecovery,
                        uiTextStaminaLeechRate = uiTextStaminaLeechRate,
                        uiTextFood = uiTextFood,
                        uiTextWater = uiTextWater,
                        uiTextAccuracy = uiTextAccuracy,
                        uiTextEvasion = uiTextEvasion,
                        uiTextCriRate = uiTextCriRate,
                        uiTextCriDmgRate = uiTextCriDmgRate,
                        uiTextBlockRate = uiTextBlockRate,
                        uiTextBlockDmgRate = uiTextBlockDmgRate,
                        uiTextMoveSpeed = uiTextMoveSpeed,
                        uiTextSprintSpeed = uiTextSprintSpeed,
                        uiTextAtkSpeed = uiTextAtkSpeed,
                        uiTextWeightLimit = uiTextWeightLimit,
                        uiTextSlotLimit = uiTextSlotLimit,
                        uiTextGoldRate = uiTextGoldRate,
                        uiTextExpRate = uiTextExpRate,
                        uiTextItemDropRate = uiTextItemDropRate,
                        uiTextJumpHeight = uiTextJumpHeight,
                        uiTextHeadDamageAbsorbs = uiTextHeadDamageAbsorbs,
                        uiTextBodyDamageAbsorbs = uiTextBodyDamageAbsorbs,
                        uiTextFallDamageAbsorbs = uiTextFallDamageAbsorbs,
                        uiTextGravityRate = uiTextGravityRate,
                        uiTextProtectedSlotLimit = uiTextProtectedSlotLimit,
                        uiTextAmmoCapacity = uiTextAmmoCapacity,
                        uiTextRecoilModifier = uiTextRecoilModifier,
                        uiTextRecoilRate = uiTextRecoilRate,
                        uiTextRateOfFire = uiTextRateOfFire,
                        uiTextReloadDuration = uiTextReloadDuration,
                        uiTextFireSpreadRangeRate = uiTextFireSpreadRangeRate,
                        uiTextFireSpread = uiTextFireSpread,
                        uiTextDecreaseFoodDecreation = uiTextDecreaseFoodDecreation,
                        uiTextDecreaseWaterDecreation = uiTextDecreaseWaterDecreation,
                        uiTextDecreaseStaminaDecreation = uiTextDecreaseStaminaDecreation,
                        uiTextBuyItemPriceRate = uiTextBuyItemPriceRate,
                        uiTextSellItemPriceRate = uiTextSellItemPriceRate,
                    };
                    this.InvokeInstanceDevExtMethods("SetRateStatsGenerateTextData", generateTextData);
                    statsString = generateTextData.GetText(bonusIncreaseColor, bonusDecreaseColor);
                    break;
                default:
                    generateTextData = new CharacterStatsTextGenerateData()
                    {
                        data = Data,
                        isRate = false,
                        isBonus = isBonus,
                        numberFormatSimple = numberFormatSimple,
                        numberFormatRate = numberFormatRate,
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
                        uiTextHp = uiTextHp,
                        uiTextHpRecovery = uiTextHpRecovery,
                        uiTextHpLeechRate = uiTextHpLeechRate,
                        uiTextMp = uiTextMp,
                        uiTextMpRecovery = uiTextMpRecovery,
                        uiTextMpLeechRate = uiTextMpLeechRate,
                        uiTextStamina = uiTextStamina,
                        uiTextStaminaRecovery = uiTextStaminaRecovery,
                        uiTextStaminaLeechRate = uiTextStaminaLeechRate,
                        uiTextFood = uiTextFood,
                        uiTextWater = uiTextWater,
                        uiTextAccuracy = uiTextAccuracy,
                        uiTextEvasion = uiTextEvasion,
                        uiTextCriRate = uiTextCriRate,
                        uiTextCriDmgRate = uiTextCriDmgRate,
                        uiTextBlockRate = uiTextBlockRate,
                        uiTextBlockDmgRate = uiTextBlockDmgRate,
                        uiTextMoveSpeed = uiTextMoveSpeed,
                        uiTextSprintSpeed = uiTextSprintSpeed,
                        uiTextAtkSpeed = uiTextAtkSpeed,
                        uiTextWeightLimit = uiTextWeightLimit,
                        uiTextSlotLimit = uiTextSlotLimit,
                        uiTextGoldRate = uiTextGoldRate,
                        uiTextExpRate = uiTextExpRate,
                        uiTextItemDropRate = uiTextItemDropRate,
                        uiTextJumpHeight = uiTextJumpHeight,
                        uiTextHeadDamageAbsorbs = uiTextHeadDamageAbsorbs,
                        uiTextBodyDamageAbsorbs = uiTextBodyDamageAbsorbs,
                        uiTextFallDamageAbsorbs = uiTextFallDamageAbsorbs,
                        uiTextGravityRate = uiTextGravityRate,
                        uiTextProtectedSlotLimit = uiTextProtectedSlotLimit,
                        uiTextAmmoCapacity = uiTextAmmoCapacity,
                        uiTextRecoilModifier = uiTextRecoilModifier,
                        uiTextRecoilRate = uiTextRecoilRate,
                        uiTextRateOfFire = uiTextRateOfFire,
                        uiTextReloadDuration = uiTextReloadDuration,
                        uiTextFireSpreadRangeRate = uiTextFireSpreadRangeRate,
                        uiTextFireSpread = uiTextFireSpread,
                        uiTextDecreaseFoodDecreation = uiTextDecreaseFoodDecreation,
                        uiTextDecreaseWaterDecreation = uiTextDecreaseWaterDecreation,
                        uiTextDecreaseStaminaDecreation = uiTextDecreaseStaminaDecreation,
                        uiTextBuyItemPriceRate = uiTextBuyItemPriceRate,
                        uiTextSellItemPriceRate = uiTextSellItemPriceRate,
                    };
                    this.InvokeInstanceDevExtMethods("SetStatsGenerateTextData", generateTextData);
                    statsString = generateTextData.GetText(bonusIncreaseColor, bonusDecreaseColor);
                    break;
            }

            // All stats text
            if (uiTextStats != null)
            {
                uiTextStats.SetGameObjectActive(!string.IsNullOrEmpty(statsString));
                uiTextStats.text = statsString;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Set All Formats To Be Simple")]
        public void SetAllFormatsToBeSimple()
        {
            formatKeyHpStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyHpRecoveryStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyHpLeechRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyMpStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyMpRecoveryStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyMpLeechRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyStaminaStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyStaminaRecoveryStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyStaminaLeechRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyFoodStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyWaterStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyAccuracyStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyEvasionStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyCriRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyCriDmgRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyBlockRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyBlockDmgRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyMoveSpeedStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeySprintSpeedStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyAtkSpeedStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyWeightLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeySlotLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyGoldRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyExpRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyItemDropRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyJumpHeightStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyHeadDamageAbsorbsStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyBodyDamageAbsorbsStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyFallDamageAbsorbsStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyGravityRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyProtectedSlotLimitStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyAmmoCapacityStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyRecoilRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyRateOfFireStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyReloadDurationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyFireSpreadRangeRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyFireSpreadStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyDecreaseFoodDecreationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyDecreaseWaterDecreationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyDecreaseStaminaDecreationStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
            formatKeyBuyItemPriceRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeySellItemPriceRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);

            formatKeyHpRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyHpRecoveryRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyHpLeechRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyMpRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyMpRecoveryRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyMpLeechRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyStaminaRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyStaminaRecoveryRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyStaminaLeechRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyFoodRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyWaterRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyAccuracyRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyEvasionRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyCriRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyCriDmgRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyBlockRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyBlockDmgRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyMoveSpeedRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeySprintSpeedRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyAtkSpeedRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyWeightLimitRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeySlotLimitRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyGoldRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyExpRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyItemDropRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyJumpHeightRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyHeadDamageAbsorbsRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyBodyDamageAbsorbsRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyFallDamageAbsorbsRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyGravityRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyProtectedSlotLimitRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyAmmoCapacityRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyRecoilRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyRateOfFireRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyReloadDurationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyFireSpreadRangeRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyFireSpreadRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyDecreaseFoodDecreationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyDecreaseWaterDecreationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyDecreaseStaminaDecreationRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeyBuyItemPriceRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            formatKeySellItemPriceRateRateStats = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
