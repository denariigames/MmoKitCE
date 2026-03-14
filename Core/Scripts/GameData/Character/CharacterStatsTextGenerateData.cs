using Cysharp.Text;
using Insthync.DevExtension;
using System.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class CharacterStatsTextGenerateData
    {
        public CharacterStats data;
        public bool isRate;
        public bool isBonus;
        public string hpStatsFormat;
        public string hpRecoveryStatsFormat;
        public string hpLeechRateStatsFormat;
        public string mpStatsFormat;
        public string mpRecoveryStatsFormat;
        public string mpLeechRateStatsFormat;
        public string staminaStatsFormat;
        public string staminaRecoveryStatsFormat;
        public string staminaLeechRateStatsFormat;
        public string foodStatsFormat;
        public string waterStatsFormat;
        public string accuracyStatsFormat;
        public string evasionStatsFormat;
        public string criRateStatsFormat;
        public string criDmgRateStatsFormat;
        public string blockRateStatsFormat;
        public string blockDmgRateStatsFormat;
        public string moveSpeedStatsFormat;
        public string sprintSpeedStatsFormat;
        public string atkSpeedStatsFormat;
        public string weightLimitStatsFormat;
        public string slotLimitStatsFormat;
        public string goldRateStatsFormat;
        public string expRateStatsFormat;
        public string itemDropRateStatsFormat;
        public string jumpHeightStatsFormat;
        public string headDamageAbsorbsStatsFormat;
        public string bodyDamageAbsorbsStatsFormat;
        public string fallDamageAbsorbsStatsFormat;
        public string gravityRateStatsFormat;
        public string protectedSlotLimitFormat;
        public string ammoCapacityFormat;
        public string recoilModifierFormat;
        public string recoilRateFormat;
        public string rateOfFireFormat;
        public string reloadDurationFormat;
        public string fireSpreadRangeRateFormat;
        public string fireSpreadFormat;
        public string decreaseFoodDecreationFormat;
        public string decreaseWaterDecreationFormat;
        public string decreaseStaminaDecreationFormat;
        public string buyItemPriceRateFormat;
        public string sellItemPriceRateFormat;
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
        public TextWrapper uiTextDecreaseFoodDecreation;
        public TextWrapper uiTextDecreaseWaterDecreation;
        public TextWrapper uiTextDecreaseStaminaDecreation;
        public TextWrapper uiTextBuyItemPriceRate;
        public TextWrapper uiTextSellItemPriceRate;

        public string numberFormatSimple = "N0";
        public string numberFormatRate = "N2";

        public string GetText(Color bonusIncreaseColor, Color bonusDecreaseColor)
        {
            StringBuilder statsStringBuilder = new StringBuilder();
            // Hp
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(hpStatsFormat), data.hp, uiTextHp, bonusIncreaseColor, bonusDecreaseColor);

            // Hp Recovery
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(hpRecoveryStatsFormat), data.hpRecovery, uiTextHpRecovery, bonusIncreaseColor, bonusDecreaseColor);

            // Hp Leech Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(hpLeechRateStatsFormat), data.hpLeechRate, uiTextHpLeechRate, bonusIncreaseColor, bonusDecreaseColor);

            // Mp
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(mpStatsFormat), data.mp, uiTextMp, bonusIncreaseColor, bonusDecreaseColor);

            // Mp Recovery
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(mpRecoveryStatsFormat), data.mpRecovery, uiTextMpRecovery, bonusIncreaseColor, bonusDecreaseColor);

            // Mp Leech Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(mpLeechRateStatsFormat), data.mpLeechRate, uiTextMpLeechRate, bonusIncreaseColor, bonusDecreaseColor);

            // Stamina
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(staminaStatsFormat), data.stamina, uiTextStamina, bonusIncreaseColor, bonusDecreaseColor);

            // Stamina Recovery
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(staminaRecoveryStatsFormat), data.staminaRecovery, uiTextStaminaRecovery, bonusIncreaseColor, bonusDecreaseColor);

            // Stamina Leech Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(staminaLeechRateStatsFormat), data.staminaLeechRate, uiTextStaminaLeechRate, bonusIncreaseColor, bonusDecreaseColor);

            // Food
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(foodStatsFormat), data.food, uiTextFood, bonusIncreaseColor, bonusDecreaseColor);

            // Water
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(waterStatsFormat), data.water, uiTextWater, bonusIncreaseColor, bonusDecreaseColor);

            // Accuracy
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(accuracyStatsFormat), data.accuracy, uiTextAccuracy, bonusIncreaseColor, bonusDecreaseColor);

            // Evasion
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(evasionStatsFormat), data.evasion, uiTextEvasion, bonusIncreaseColor, bonusDecreaseColor);

            // Cri Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(criRateStatsFormat), data.criRate, uiTextCriRate, bonusIncreaseColor, bonusDecreaseColor);

            // Cri Dmg Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(criDmgRateStatsFormat), data.criDmgRate, uiTextCriDmgRate, bonusIncreaseColor, bonusDecreaseColor);

            // Block Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(blockRateStatsFormat), data.blockRate, uiTextBlockRate, bonusIncreaseColor, bonusDecreaseColor);

            // Block Dmg Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(blockDmgRateStatsFormat), data.blockDmgRate, uiTextBlockDmgRate, bonusIncreaseColor, bonusDecreaseColor);

            // Move Speed
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(moveSpeedStatsFormat), data.moveSpeed, uiTextMoveSpeed, bonusIncreaseColor, bonusDecreaseColor);

            // Sprint Speed
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(sprintSpeedStatsFormat), data.sprintSpeed, uiTextSprintSpeed, bonusIncreaseColor, bonusDecreaseColor);

            // Attack Speed
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(atkSpeedStatsFormat), data.atkSpeed, uiTextAtkSpeed, bonusIncreaseColor, bonusDecreaseColor);

            // Weight
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(weightLimitStatsFormat), data.weightLimit, uiTextWeightLimit, bonusIncreaseColor, bonusDecreaseColor);

            // Slot
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(slotLimitStatsFormat), data.slotLimit, uiTextSlotLimit, bonusIncreaseColor, bonusDecreaseColor);

            // Gold Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(goldRateStatsFormat), data.goldRate, uiTextGoldRate, bonusIncreaseColor, bonusDecreaseColor);

            // Exp Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(expRateStatsFormat), data.expRate, uiTextExpRate, bonusIncreaseColor, bonusDecreaseColor);

            // Item Drop Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(itemDropRateStatsFormat), data.itemDropRate, uiTextItemDropRate, bonusIncreaseColor, bonusDecreaseColor);

            // Jump Height
            GetSingleStatsText(statsStringBuilder, isRate || false, LanguageManager.GetText(jumpHeightStatsFormat), data.jumpHeight, uiTextJumpHeight, bonusIncreaseColor, bonusDecreaseColor);

            // Head Damage Absorbs
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(headDamageAbsorbsStatsFormat), data.headDamageAbsorbs, uiTextHeadDamageAbsorbs, bonusIncreaseColor, bonusDecreaseColor);

            // Body Damage Absorbs
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(bodyDamageAbsorbsStatsFormat), data.bodyDamageAbsorbs, uiTextBodyDamageAbsorbs, bonusIncreaseColor, bonusDecreaseColor);

            // Fall Damage Absorbs
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(fallDamageAbsorbsStatsFormat), data.fallDamageAbsorbs, uiTextFallDamageAbsorbs, bonusIncreaseColor, bonusDecreaseColor);

            // Gravity Rate
            GetSingleStatsText(statsStringBuilder, isRate || true, LanguageManager.GetText(gravityRateStatsFormat), data.gravityRate, uiTextGravityRate, bonusIncreaseColor, bonusDecreaseColor);

            // Protected Slot Limit
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(protectedSlotLimitFormat), data.protectedSlotLimit, uiTextProtectedSlotLimit, bonusIncreaseColor, bonusDecreaseColor);

            // Ammo Capacity
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(ammoCapacityFormat), data.ammoCapacity, uiTextAmmoCapacity, bonusIncreaseColor, bonusDecreaseColor);

            // Recoil Modifier
            GetSingleStatsText(statsStringBuilder, true, LanguageManager.GetText(recoilModifierFormat), data.recoilModifier, uiTextRecoilModifier, bonusIncreaseColor, bonusDecreaseColor);

            // Recoil Rate
            GetSingleStatsText(statsStringBuilder, true, LanguageManager.GetText(recoilRateFormat), data.recoilRate, uiTextRecoilRate, bonusIncreaseColor, bonusDecreaseColor);

            // Rate Of Fire
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(rateOfFireFormat), data.rateOfFire, uiTextRateOfFire, bonusIncreaseColor, bonusDecreaseColor);

            // Reload Duration
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(reloadDurationFormat), data.reloadDuration, uiTextReloadDuration, bonusIncreaseColor, bonusDecreaseColor);

            // Fire Spread Range Rate
            GetSingleStatsText(statsStringBuilder, true, LanguageManager.GetText(fireSpreadRangeRateFormat), data.fireSpreadRangeRate, uiTextFireSpreadRangeRate, bonusIncreaseColor, bonusDecreaseColor);

            // Fire Spread
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(fireSpreadFormat), data.fireSpread, uiTextFireSpread, bonusIncreaseColor, bonusDecreaseColor);

            // Decrease Food Decreation
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(decreaseFoodDecreationFormat), data.decreaseFoodDecreation, uiTextDecreaseFoodDecreation, bonusIncreaseColor, bonusDecreaseColor);

            // Decrease Water Decreation
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(decreaseWaterDecreationFormat), data.decreaseWaterDecreation, uiTextDecreaseWaterDecreation, bonusIncreaseColor, bonusDecreaseColor);

            // Decrease Stamina Decreation
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(decreaseStaminaDecreationFormat), data.decreaseStaminaDecreation, uiTextDecreaseStaminaDecreation, bonusIncreaseColor, bonusDecreaseColor);

            // Buy Item Price Rate
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(buyItemPriceRateFormat), data.buyItemPriceRate, uiTextBuyItemPriceRate, bonusIncreaseColor, bonusDecreaseColor);

            // Sell Item Price Rate
            GetSingleStatsText(statsStringBuilder, false, LanguageManager.GetText(sellItemPriceRateFormat), data.sellItemPriceRate, uiTextSellItemPriceRate, bonusIncreaseColor, bonusDecreaseColor);

            // Dev Extension
            // How to implement it?:
            // /*
            //  * - Add `customStat1` to `CharacterStats` partial class file
            //  * - Add `customStat1StatsFormat` to `CharacterStatsTextGenerateData`
            //  * - Add `uiTextCustomStat1` to `CharacterStatsTextGenerateData`
            //  */
            // [DevExtMethods("GetText")]
            // public void GetText_Ext(StringBuilder statsString)
            // {
            //   string tempValue;
            //   string statsStringPart;
            //   tempValue = isRate ? (data.customStat1 * 100).ToString("N2") : data.customStat1.ToString("N0");
            //   statsStringPart = ZString.Format(LanguageManager.GetText(customStat1StatsFormat), tempValue);
            //   if (data.customStat1 != 0)
            //   {
            //       if (statsString.Length > 0)
            //           statsString.Append('\n');
            //       statsString.Append(statsStringPart);
            //   }
            //   if (uiTextCustomStat1 != null)
            //       uiTextCustomStat1.text = statsStringPart;
            // }
            this.InvokeInstanceDevExtMethods("GetText", statsStringBuilder, bonusIncreaseColor, bonusDecreaseColor);

            return statsStringBuilder.ToString();
        }

        public void GetSingleStatsText(StringBuilder builder, bool isRateStats, string format, float value, TextWrapper textComponent, Color bonusIncreaseColor, Color bonusDecreaseColor)
        {
            // Determine the correct format string based on whether the stat is a rate
            string numberFormat = isRateStats ? numberFormatRate : numberFormatSimple;

            // Calculate the value to display, adjusting for rates if necessary
            string tempValue = isRateStats ? (value * 100).ToString(numberFormat) : value.ToString(numberFormat);

            // Construct the display string
            string statsStringPart = ZString.Concat(isBonus && value >= 0 ? "+" : string.Empty, ZString.Format(
                format,
                tempValue));

            // Append the stat text to the builder if the value is not zero
            if (value != 0)
            {
                if (builder.Length > 0)
                    builder.Append('\n');
                builder.Append(statsStringPart);
            }

            // Set the text component if it's provided
            if (textComponent != null)
            {
                if (isBonus)
                {
                    if (value >= 0)
                        textComponent.color = bonusIncreaseColor;
                    else
                        textComponent.color = bonusDecreaseColor;
                }
                textComponent.text = statsStringPart;
            }
        }

        public void GetBooleanStatsText(StringBuilder builder, string text, bool value, TextWrapper textComponent)
        {
            if (isRate)
                return;
            string statsStringPart = value ? text : string.Empty;
            if (value)
            {
                if (builder.Length > 0)
                    builder.Append('\n');
                builder.Append(statsStringPart);
            }
            if (textComponent != null)
                textComponent.text = statsStringPart;
        }
    }
}
