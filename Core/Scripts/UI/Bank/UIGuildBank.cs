using LiteNetLibManager;

namespace MultiplayerARPG
{
    public class UIGuildBank : UIBaseBank
    {
        public override int GetAmount()
        {
            if (GameInstance.JoinedGuild == null)
                return 0;
            return GameInstance.JoinedGuild.gold;
        }

        public override int GetDepositFee(int amount)
        {
            return GameInstance.Singleton.GameplayRule.GetGuildBankDepositFee(amount);
        }

        public override int GetWithdrawFee(int amount)
        {
            return GameInstance.Singleton.GameplayRule.GetGuildBankWithdrawFee(amount);
        }

        public override void OnDepositConfirm(int amount)
        {
            GameInstance.ClientBankHandlers.RequestDepositGuildGold(new RequestDepositGuildGoldMessage()
            {
                gold = amount,
            }, ResponseDepositGuildGold);
        }

        public void ResponseDepositGuildGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDepositGuildGoldMessage response)
        {
            ClientBankActions.ResponseDepositGuildGold(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message))
                return;
            ResetDepositUIs();
            ResetFeeUIs();
        }

        public override void OnWithdrawConfirm(int amount)
        {
            GameInstance.ClientBankHandlers.RequestWithdrawGuildGold(new RequestWithdrawGuildGoldMessage()
            {
                gold = amount,
            }, ResponseWithdrawGuildGold);
        }

        public void ResponseWithdrawGuildGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseWithdrawGuildGoldMessage response)
        {
            ClientBankActions.ResponseWithdrawGuildGold(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message))
                return;
            ResetWithdrawUIs();
            ResetFeeUIs();
        }
    }
}
