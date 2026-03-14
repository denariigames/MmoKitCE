using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientInventoryHandlers
    {
        bool RequestSwapOrMergeItem(RequestSwapOrMergeItemMessage data, ResponseDelegate<ResponseSwapOrMergeItemMessage> callback);
        bool RequestEquipWeapon(RequestEquipWeaponMessage data, ResponseDelegate<ResponseEquipWeaponMessage> callback);
        bool RequestEquipArmor(RequestEquipArmorMessage data, ResponseDelegate<ResponseEquipArmorMessage> callback);
        bool RequestUnEquipWeapon(RequestUnEquipWeaponMessage data, ResponseDelegate<ResponseUnEquipWeaponMessage> callback);
        bool RequestUnEquipArmor(RequestUnEquipArmorMessage data, ResponseDelegate<ResponseUnEquipArmorMessage> callback);
        bool RequestSwitchEquipWeaponSet(RequestSwitchEquipWeaponSetMessage data, ResponseDelegate<ResponseSwitchEquipWeaponSetMessage> callback);
        bool RequestDismantleItem(RequestDismantleItemMessage data, ResponseDelegate<ResponseDismantleItemMessage> callback);
        bool RequestDismantleItems(RequestDismantleItemsMessage data, ResponseDelegate<ResponseDismantleItemsMessage> callback);
        bool RequestEnhanceSocketItem(RequestEnhanceSocketItemMessage data, ResponseDelegate<ResponseEnhanceSocketItemMessage> callback);
        bool RequestRefineItem(RequestRefineItemMessage data, ResponseDelegate<ResponseRefineItemMessage> callback);
        bool RequestRemoveEnhancerFromItem(RequestRemoveEnhancerFromItemMessage data, ResponseDelegate<ResponseRemoveEnhancerFromItemMessage> callback);
        bool RequestRepairItem(RequestRepairItemMessage data, ResponseDelegate<ResponseRepairItemMessage> callback);
        bool RequestRepairEquipItems(ResponseDelegate<ResponseRepairEquipItemsMessage> callback);
        bool RequestSellItem(RequestSellItemMessage data, ResponseDelegate<ResponseSellItemMessage> callback);
        bool RequestSellItems(RequestSellItemsMessage data, ResponseDelegate<ResponseSellItemsMessage> callback);
        bool RequestSortItems(RequestSortItemsMessage data, ResponseDelegate<ResponseSortItemsMessage> callback);
        bool RequestChangeAmmoItem(RequestChangeAmmoItemMessage data, ResponseDelegate<ResponseChangeAmmoItemMessage> callback);
        bool RequestRemoveAmmoFromItem(RequestRemoveAmmoFromItemMessage data, ResponseDelegate<ResponseRemoveAmmoFromItemMessage> callback);
    }
}
