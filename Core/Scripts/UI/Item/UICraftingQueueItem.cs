using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UICraftingQueueItem : UIDataForCharacter<CraftingQueueItem>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Amount}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Craft Duration}")]
        public UILocaleKeySetting formatKeyCraftDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CRAFT_DURATION);
        [Tooltip("Format => {0} = {Craft Remains Duration}")]
        public UILocaleKeySetting formatKeyCraftRemainsDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextAmount;
        public TextWrapper uiTextDuration;
        public TextWrapper uiTextRemainsDuration;
        public Image imageDurationGage;
        public UIItemCraft uiItemCraft;
        public InputFieldWrapper inputAmount;

        public UICraftingQueueItems CraftingQueueManager { get; set; }

        protected float _craftRemainsDuration;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextAmount = null;
            uiTextDuration = null;
            uiTextRemainsDuration = null;
            imageDurationGage = null;
            uiItemCraft = null;
            inputAmount = null;
            CraftingQueueManager = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _craftRemainsDuration = 0f;
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();

            if (_craftRemainsDuration > 0f)
            {
                _craftRemainsDuration -= Time.deltaTime;
                if (_craftRemainsDuration <= 0f)
                    _craftRemainsDuration = 0f;
            }
            else
            {
                _craftRemainsDuration = 0f;
            }

            // Only first queue will show remains duration
            if (IndexOfData > 0)
                _craftRemainsDuration = 0f;

            // Update UIs
            float craftDuration = 0;

            ItemCraftFormula formula;
            if (GameInstance.ItemCraftFormulas.TryGetValue(Data.dataId, out formula))
                craftDuration = formula.CraftDuration;

            if (uiTextAmount != null)
            {
                uiTextAmount.text = ZString.Format(
                    LanguageManager.GetText(formatKeyAmount),
                    Data.amount.ToString("N0"));
            }

            if (uiTextDuration != null)
            {
                uiTextDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCraftDuration),
                    craftDuration.ToString("N0"));
            }

            if (uiTextRemainsDuration != null)
            {
                uiTextRemainsDuration.SetGameObjectActive(_craftRemainsDuration > 0);
                uiTextRemainsDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeyCraftRemainsDuration),
                    _craftRemainsDuration.ToString("N0"));
            }

            if (imageDurationGage != null)
            {
                imageDurationGage.fillAmount = craftDuration <= 0 ? 0 : _craftRemainsDuration / craftDuration;
                imageDurationGage.gameObject.SetActive(imageDurationGage.fillAmount > 0f);
            }
        }

        protected override void UpdateUI()
        {
            base.UpdateUI();

            // Update remains duration
            if (_craftRemainsDuration <= 0f)
                _craftRemainsDuration = Data.craftRemainsDuration;
        }

        protected override void UpdateData()
        {
            // Update remains duration
            if (Mathf.Abs(Data.craftRemainsDuration - _craftRemainsDuration) > 1)
                _craftRemainsDuration = Data.craftRemainsDuration;

            ItemCraftFormula formula;
            GameInstance.ItemCraftFormulas.TryGetValue(Data.dataId, out formula);

            if (uiItemCraft != null)
            {
                if (formula == null)
                {
                    uiItemCraft.Hide();
                }
                else
                {
                    uiItemCraft.Show();
                    uiItemCraft.Data = formula.ItemCraft;
                }
            }
        }

        public void OnClickChange()
        {
            int amount;
            if (inputAmount == null || !int.TryParse(inputAmount.text, out amount))
                amount = 1;
            GameInstance.PlayingCharacterEntity.CallCmdChangeCraftingQueueItem(CraftingQueueManager.Source.ObjectId, IndexOfData, amount);
        }

        public void OnClickCancel()
        {
            GameInstance.PlayingCharacterEntity.CallCmdCancelCraftingQueueItem(CraftingQueueManager.Source.ObjectId, IndexOfData);
        }
    }
}
