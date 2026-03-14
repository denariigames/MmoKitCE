using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientInventoryHandlers : MonoBehaviour, IClientInventoryHandlers
    {
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestSwapOrMergeItem(RequestSwapOrMergeItemMessage data, ResponseDelegate<ResponseSwapOrMergeItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SwapOrMergeItem, data, responseDelegate: callback);
        }

        public bool RequestEquipWeapon(RequestEquipWeaponMessage data, ResponseDelegate<ResponseEquipWeaponMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.EquipWeapon, data, responseDelegate: callback);
        }

        public bool RequestEquipArmor(RequestEquipArmorMessage data, ResponseDelegate<ResponseEquipArmorMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.EquipArmor, data, responseDelegate: callback);
        }

        public bool RequestUnEquipWeapon(RequestUnEquipWeaponMessage data, ResponseDelegate<ResponseUnEquipWeaponMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.UnEquipWeapon, data, responseDelegate: callback);
        }

        public bool RequestUnEquipArmor(RequestUnEquipArmorMessage data, ResponseDelegate<ResponseUnEquipArmorMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.UnEquipArmor, data, responseDelegate: callback);
        }

        public bool RequestSwitchEquipWeaponSet(RequestSwitchEquipWeaponSetMessage data, ResponseDelegate<ResponseSwitchEquipWeaponSetMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SwitchEquipWeaponSet, data, responseDelegate: callback);
        }

        public bool RequestDismantleItem(RequestDismantleItemMessage data, ResponseDelegate<ResponseDismantleItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DismantleItem, data, responseDelegate: callback);
        }

        public bool RequestDismantleItems(RequestDismantleItemsMessage data, ResponseDelegate<ResponseDismantleItemsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DismantleItems, data, responseDelegate: callback);
        }

        public bool RequestEnhanceSocketItem(RequestEnhanceSocketItemMessage data, ResponseDelegate<ResponseEnhanceSocketItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.EnhanceSocketItem, data, responseDelegate: callback);
        }

        public bool RequestRefineItem(RequestRefineItemMessage data, ResponseDelegate<ResponseRefineItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.RefineItem, data, responseDelegate: callback);
        }

        public bool RequestRemoveEnhancerFromItem(RequestRemoveEnhancerFromItemMessage data, ResponseDelegate<ResponseRemoveEnhancerFromItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.RemoveEnhancerFromItem, data, responseDelegate: callback);
        }

        public bool RequestRepairItem(RequestRepairItemMessage data, ResponseDelegate<ResponseRepairItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.RepairItem, data, responseDelegate: callback);
        }

        public bool RequestRepairEquipItems(ResponseDelegate<ResponseRepairEquipItemsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.RepairEquipItems, EmptyMessage.Value, responseDelegate: callback);
        }

        public bool RequestSellItem(RequestSellItemMessage data, ResponseDelegate<ResponseSellItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SellItem, data, responseDelegate: callback);
        }

        public bool RequestSellItems(RequestSellItemsMessage data, ResponseDelegate<ResponseSellItemsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SellItems, data, responseDelegate: callback);
        }

        public bool RequestSortItems(RequestSortItemsMessage data, ResponseDelegate<ResponseSortItemsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SortItems, data, responseDelegate: callback);
        }

        public bool RequestChangeAmmoItem(RequestChangeAmmoItemMessage data, ResponseDelegate<ResponseChangeAmmoItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.ChangeAmmoItem, data, responseDelegate: callback);
        }

        public bool RequestRemoveAmmoFromItem(RequestRemoveAmmoFromItemMessage data, ResponseDelegate<ResponseRemoveAmmoFromItemMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.RemoveAmmoFromItem, data, responseDelegate: callback);
        }
    }
}
