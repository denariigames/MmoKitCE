using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class ClientCashShopActions
    {
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseCashShopInfoMessage> onResponseCashShopInfo;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseCashPackageInfoMessage> onResponseCashPackageInfo;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseCashShopBuyMessage> onResponseCashShopBuy;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseCashPackageBuyValidationMessage> onResponseCashPackageBuyValidation;

        public static void Clean()
        {
            onResponseCashShopInfo = null;
            onResponseCashPackageInfo = null;
            onResponseCashShopBuy = null;
            onResponseCashPackageBuyValidation = null;
        }

        public static void ResponseCashShopInfo(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashShopInfoMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseCashShopInfo != null)
                onResponseCashShopInfo.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseCashPackageInfo(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashPackageInfoMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseCashPackageInfo != null)
                onResponseCashPackageInfo.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseCashShopBuy(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashShopBuyMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseCashShopBuy != null)
                onResponseCashShopBuy.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseCashPackageBuyValidation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashPackageBuyValidationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseCashPackageBuyValidation != null)
                onResponseCashPackageBuyValidation.Invoke(requestHandler, responseCode, response);
        }
    }
}
