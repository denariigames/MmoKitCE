using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class ClientInventoryActions
    {
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSwapOrMergeItemMessage> onResponseSwapOrMergeItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseEquipArmorMessage> onResponseEquipArmor;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseEquipWeaponMessage> onResponseEquipWeapon;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseUnEquipArmorMessage> onResponseUnEquipArmor;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseUnEquipWeaponMessage> onResponseUnEquipWeapon;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSwitchEquipWeaponSetMessage> onResponseSwitchEquipWeaponSet;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDismantleItemMessage> onResponseDismantleItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDismantleItemsMessage> onResponseDismantleItems;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseEnhanceSocketItemMessage> onResponseEnhanceSocketItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseRefineItemMessage> onResponseRefineItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseRemoveEnhancerFromItemMessage> onResponseRemoveEnhancerFromItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseRepairItemMessage> onResponseRepairItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseRepairEquipItemsMessage> onResponseRepairEquipItems;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSellItemMessage> onResponseSellItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSellItemsMessage> onResponseSellItems;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSortItemsMessage> onResponseSortItems;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseChangeAmmoItemMessage> onResponseChangeAmmoItem;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseRemoveAmmoFromItemMessage> onResponseRemoveAmmoFromItem;

        public static void Clean()
        {
            onResponseSwapOrMergeItem = null;
            onResponseEquipArmor = null;
            onResponseEquipWeapon = null;
            onResponseUnEquipArmor = null;
            onResponseUnEquipWeapon = null;
            onResponseSwitchEquipWeaponSet = null;
            onResponseDismantleItem = null;
            onResponseDismantleItems = null;
            onResponseEnhanceSocketItem = null;
            onResponseRefineItem = null;
            onResponseRemoveEnhancerFromItem = null;
            onResponseRepairItem = null;
            onResponseRepairEquipItems = null;
            onResponseSellItem = null;
            onResponseSellItems = null;
            onResponseSortItems = null;
        }

        public static void ResponseSwapOrMergeItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSwapOrMergeItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSwapOrMergeItem != null)
                onResponseSwapOrMergeItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseEquipArmor(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseEquipArmorMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseEquipArmor != null)
                onResponseEquipArmor.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseEquipWeapon(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseEquipWeaponMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseEquipWeapon != null)
                onResponseEquipWeapon.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseUnEquipArmor(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseUnEquipArmorMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseUnEquipArmor != null)
                onResponseUnEquipArmor.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseUnEquipWeapon(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseUnEquipWeaponMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseUnEquipWeapon != null)
                onResponseUnEquipWeapon.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseSwitchEquipWeaponSet(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSwitchEquipWeaponSetMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSwitchEquipWeaponSet != null)
                onResponseSwitchEquipWeaponSet.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseDismantleItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDismantleItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDismantleItem != null)
                onResponseDismantleItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseDismantleItems(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDismantleItemsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDismantleItems != null)
                onResponseDismantleItems.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseEnhanceSocketItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseEnhanceSocketItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseEnhanceSocketItem != null)
                onResponseEnhanceSocketItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseRefineItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseRefineItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseRefineItem != null)
                onResponseRefineItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseRemoveEnhancerFromItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseRemoveEnhancerFromItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseRemoveEnhancerFromItem != null)
                onResponseRemoveEnhancerFromItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseRepairItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseRepairItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseRepairItem != null)
                onResponseRepairItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseRepairEquipItems(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseRepairEquipItemsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseRepairEquipItems != null)
                onResponseRepairEquipItems.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseSellItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSellItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSellItem != null)
                onResponseSellItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseSellItems(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSellItemsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSellItems != null)
                onResponseSellItems.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseSortItems(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSortItemsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSortItems != null)
                onResponseSortItems.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseChangeAmmoItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangeAmmoItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseChangeAmmoItem != null)
                onResponseChangeAmmoItem.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseRemoveAmmoFromItem(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseRemoveAmmoFromItemMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseRemoveAmmoFromItem != null)
                onResponseRemoveAmmoFromItem.Invoke(requestHandler, responseCode, response);
        }
    }
}
