using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IServerInventoryMessageHandlers
    {
        UniTaskVoid HandleRequestSwapOrMergeItem(
            RequestHandlerData requestHandler, RequestSwapOrMergeItemMessage request,
            RequestProceedResultDelegate<ResponseSwapOrMergeItemMessage> result);

        UniTaskVoid HandleRequestEquipWeapon(
            RequestHandlerData requestHandler, RequestEquipWeaponMessage request,
            RequestProceedResultDelegate<ResponseEquipWeaponMessage> result);

        UniTaskVoid HandleRequestEquipArmor(
            RequestHandlerData requestHandler, RequestEquipArmorMessage request,
            RequestProceedResultDelegate<ResponseEquipArmorMessage> result);

        UniTaskVoid HandleRequestUnEquipWeapon(
            RequestHandlerData requestHandler, RequestUnEquipWeaponMessage request,
            RequestProceedResultDelegate<ResponseUnEquipWeaponMessage> result);

        UniTaskVoid HandleRequestUnEquipArmor(
            RequestHandlerData requestHandler, RequestUnEquipArmorMessage request,
            RequestProceedResultDelegate<ResponseUnEquipArmorMessage> result);

        UniTaskVoid HandleRequestSwitchEquipWeaponSet(
            RequestHandlerData requestHandler, RequestSwitchEquipWeaponSetMessage request,
            RequestProceedResultDelegate<ResponseSwitchEquipWeaponSetMessage> result);

        UniTaskVoid HandleRequestDismantleItem(
            RequestHandlerData requestHandler, RequestDismantleItemMessage request,
            RequestProceedResultDelegate<ResponseDismantleItemMessage> result);

        UniTaskVoid HandleRequestDismantleItems(
            RequestHandlerData requestHandler, RequestDismantleItemsMessage request,
            RequestProceedResultDelegate<ResponseDismantleItemsMessage> result);

        UniTaskVoid HandleRequestEnhanceSocketItem(
            RequestHandlerData requestHandler, RequestEnhanceSocketItemMessage request,
            RequestProceedResultDelegate<ResponseEnhanceSocketItemMessage> result);

        UniTaskVoid HandleRequestRefineItem(
            RequestHandlerData requestHandler, RequestRefineItemMessage request,
            RequestProceedResultDelegate<ResponseRefineItemMessage> result);

        UniTaskVoid HandleRequestRemoveEnhancerFromItem(
            RequestHandlerData requestHandler, RequestRemoveEnhancerFromItemMessage request,
            RequestProceedResultDelegate<ResponseRemoveEnhancerFromItemMessage> result);

        UniTaskVoid HandleRequestRepairItem(
            RequestHandlerData requestHandler, RequestRepairItemMessage request,
            RequestProceedResultDelegate<ResponseRepairItemMessage> result);

        UniTaskVoid HandleRequestRepairEquipItems(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseRepairEquipItemsMessage> result);

        UniTaskVoid HandleRequestSellItem(
            RequestHandlerData requestHandler, RequestSellItemMessage request,
            RequestProceedResultDelegate<ResponseSellItemMessage> result);

        UniTaskVoid HandleRequestSellItems(
            RequestHandlerData requestHandler, RequestSellItemsMessage request,
            RequestProceedResultDelegate<ResponseSellItemsMessage> result);

        UniTaskVoid HandleRequestSortItems(
            RequestHandlerData requestHandler, RequestSortItemsMessage request,
            RequestProceedResultDelegate<ResponseSortItemsMessage> result);

        UniTaskVoid HandleRequestChangeAmmoItem(
            RequestHandlerData requestHandler, RequestChangeAmmoItemMessage request,
            RequestProceedResultDelegate<ResponseChangeAmmoItemMessage> result);

        UniTaskVoid HandleRequestRemoveAmmoFromItem(
            RequestHandlerData requestHandler, RequestRemoveAmmoFromItemMessage request,
            RequestProceedResultDelegate<ResponseRemoveAmmoFromItemMessage> result);
    }
}
