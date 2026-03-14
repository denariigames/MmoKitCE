using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class UIBaseBank : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        public UILocaleKeySetting formatKeyFee = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_FEE);

        [Header("UI Elements")]
        public TextWrapper uiTextAmount;
        public TextWrapper uiTextFee;
        public UIInputDialog uiDepositInput;
        public UIInputDialog uiWithdrawInput;

        private void OnEnable()
        {
            ResetDepositUIs();
            ResetWithdrawUIs();
            ResetFeeUIs();
        }

        public void ResetDepositUIs()
        {
            if (uiDepositInput == null)
                return;               
            uiDepositInput.SetupForIntegerInput(OnDepositConfirm, 0, null, 0);
            if (uiDepositInput.uiInputField != null)
            {
                uiDepositInput.uiInputField.onValueChanged.RemoveListener(OnDepositValueChangedProceeding);
                uiDepositInput.uiInputField.onValueChanged.AddListener(OnDepositValueChangedProceeding);
            }
        }

        public void ResetWithdrawUIs()
        {
            if (uiWithdrawInput == null)
                return;
            uiWithdrawInput.SetupForIntegerInput(OnWithdrawConfirm, 0, null, 0);
            if (uiWithdrawInput.uiInputField != null)
            {
                uiWithdrawInput.uiInputField.onValueChanged.RemoveListener(OnWithdrawValueChangedProceeding);
                uiWithdrawInput.uiInputField.onValueChanged.AddListener(OnWithdrawValueChangedProceeding);
            }
        }

        public void ResetFeeUIs()
        {
            if (uiTextFee == null)
                return;
            uiTextFee.text = ZString.Format(LanguageManager.GetText(formatKeyFee), "0");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextAmount = null;
            uiTextFee = null;
            uiDepositInput = null;
            uiWithdrawInput = null;
        }

        private void Update()
        {
            if (uiTextAmount != null)
            {
                uiTextAmount.text = ZString.Format(
                    LanguageManager.GetText(formatKeyAmount),
                    GetAmount().ToString("N0"));
            }
        }

        public void OnClickDeposit()
        {
            ResetFeeUIs();
            if (uiDepositInput != null)
            {
                ResetDepositUIs();
                uiDepositInput.Show();
            }
            else
            {
                RemoveAllInputDialogEvents();
                AddInputDialogDepositEvents();
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_BANK_DEPOSIT.ToString()), LanguageManager.GetText(UITextKeys.UI_BANK_DEPOSIT_DESCRIPTION.ToString()), OnDepositConfirmProceeding, 0, null, 0);
            }
        }

        public void OnClickWithdraw()
        {
            ResetFeeUIs();
            if (uiWithdrawInput != null)
            {
                ResetWithdrawUIs();
                uiWithdrawInput.Show();
            }
            else
            {
                RemoveAllInputDialogEvents();
                AddInputDialogWithdrawEvents();
                UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_BANK_WITHDRAW.ToString()), LanguageManager.GetText(UITextKeys.UI_BANK_WITHDRAW_DESCRIPTION.ToString()), OnWithdrawConfirmProceeding, 0, null, 0);
            }
        }

        private void RemoveAllInputDialogEvents()
        {
            if (UISceneGlobal.Singleton.uiInputDialog == null || UISceneGlobal.Singleton.uiInputDialog.uiInputField == null)
                return;
            UISceneGlobal.Singleton.uiInputDialog.uiInputField.onValueChanged.RemoveListener(OnDepositValueChangedProceeding);
            UISceneGlobal.Singleton.uiInputDialog.uiInputField.onValueChanged.RemoveListener(OnWithdrawValueChangedProceeding);
        }

        private void AddInputDialogDepositEvents()
        {
            if (UISceneGlobal.Singleton.uiInputDialog == null || UISceneGlobal.Singleton.uiInputDialog.uiInputField == null)
                return;
            UISceneGlobal.Singleton.uiInputDialog.uiInputField.onValueChanged.AddListener(OnDepositValueChangedProceeding);
        }

        private void AddInputDialogWithdrawEvents()
        {
            if (UISceneGlobal.Singleton.uiInputDialog == null || UISceneGlobal.Singleton.uiInputDialog.uiInputField == null)
                return;
            UISceneGlobal.Singleton.uiInputDialog.uiInputField.onValueChanged.AddListener(OnWithdrawValueChangedProceeding);
        }

        private void OnDepositConfirmProceeding(int amount)
        {
            RemoveAllInputDialogEvents();
            OnDepositConfirm(amount);
        }

        private void OnWithdrawConfirmProceeding(int amount)
        {
            RemoveAllInputDialogEvents();
            OnWithdrawConfirm(amount);
        }

        private void OnDepositValueChangedProceeding(string value)
        {
            if (!int.TryParse(value, out int amount))
                amount = 0;

            if (uiTextFee != null)
            {
                uiTextFee.text = ZString.Format(LanguageManager.GetText(formatKeyFee), GetDepositFee(amount));
            }
        }

        private void OnWithdrawValueChangedProceeding(string value)
        {
            if (!int.TryParse(value, out int amount))
                amount = 0;

            if (uiTextFee != null)
            {
                uiTextFee.text = ZString.Format(LanguageManager.GetText(formatKeyFee), GetWithdrawFee(amount));
            }
        }

        public abstract void OnDepositConfirm(int amount);
        public abstract void OnWithdrawConfirm(int amount);
        public abstract int GetAmount();
        public abstract int GetDepositFee(int amount);
        public abstract int GetWithdrawFee(int amount);
    }
}
