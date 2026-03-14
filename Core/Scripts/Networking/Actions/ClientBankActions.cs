using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class ClientBankActions
    {
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDepositUserGoldMessage> onResponseDepositUserGold;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseWithdrawUserGoldMessage> onResponseWithdrawUserGold;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDepositGuildGoldMessage> onResponseDepositGuildGold;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseWithdrawGuildGoldMessage> onResponseWithdrawGuildGold;

        public static void Clean()
        {
            onResponseDepositUserGold = null;
            onResponseWithdrawUserGold = null;
            onResponseDepositGuildGold = null;
            onResponseWithdrawGuildGold = null;
        }

        public static void ResponseDepositUserGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDepositUserGoldMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDepositUserGold != null)
                onResponseDepositUserGold.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseWithdrawUserGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseWithdrawUserGoldMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseWithdrawUserGold != null)
                onResponseWithdrawUserGold.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseDepositGuildGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDepositGuildGoldMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDepositGuildGold != null)
                onResponseDepositGuildGold.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseWithdrawGuildGold(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseWithdrawGuildGoldMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseWithdrawGuildGold != null)
                onResponseWithdrawGuildGold.Invoke(requestHandler, responseCode, response);
        }
    }
}
