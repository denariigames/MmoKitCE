using LiteNetLibManager;

namespace MultiplayerARPG
{
    public class UIPlayerBank : UIBaseBank
    {
        public override int GetAmount()
        {
            if (GameInstance.PlayingCharacter == null)
                return 0;
            return GameInstance.PlayingCharacter.UserGold;
        }

        public override int GetDepositFee(int amount)
        {
            return GameInstance.Singleton.GameplayRule.GetPlayerBankDepositFee(amount);
        }

        public override int GetWithdrawFee(int amount)
        {
            return GameInstance.Singleton.GameplayRule.GetPlayerBankWithdrawFee(amount);
        }

        public override void OnDepositConfirm(int amount)
        {
            GameInstance.ClientBankHandlers.RequestDepositUserGold(new RequestDepositUserGoldMessage()
            {
                gold = amount,
            }, ResponseDepositUserGold);
        }

        public void ResponseDepositUserGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDepositUserGoldMessage response)
        {
            ClientBankActions.ResponseDepositUserGold(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message))
                return;
            ResetDepositUIs();
            ResetFeeUIs();
        }

        public override void OnWithdrawConfirm(int amount)
        {
            GameInstance.ClientBankHandlers.RequestWithdrawUserGold(new RequestWithdrawUserGoldMessage()
            {
                gold = amount,
            }, ResponseWithdrawUserGold);
        }

        public void ResponseWithdrawUserGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseWithdrawUserGoldMessage response)
        {
            ClientBankActions.ResponseWithdrawUserGold(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message))
                return;
            ResetWithdrawUIs();
            ResetFeeUIs();
        }
    }
}
