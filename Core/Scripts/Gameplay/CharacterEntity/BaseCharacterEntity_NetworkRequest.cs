using LiteNetLib;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        public bool CallCmdPickup(uint objectId)
        {
            if (!CanPickup())
                return false;
            RPC(CmdPickup, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, objectId);
            CallRpcPlayPickupAnimation();
            return true;
        }

        public bool CallCmdPickupItemFromContainer(uint objectId, int itemsContainerIndex, int amount)
        {
            if (!CanPickup())
                return false;
            RPC(CmdPickupItemFromContainer, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, objectId, itemsContainerIndex, amount);
            CallRpcPlayPickupAnimation();
            return true;
        }

        public bool CallCmdPickupAllItemsFromContainer(uint objectId)
        {
            if (!CanPickup())
                return false;
            RPC(CmdPickupAllItemsFromContainer, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, objectId);
            CallRpcPlayPickupAnimation();
            return true;
        }

        public bool CallCmdPickupNearbyItems()
        {
            if (!CanPickup())
                return false;
            RPC(CmdPickupNearbyItems, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
            CallRpcPlayPickupAnimation();
            return true;
        }

        public bool CallCmdDropItem(InventoryType inventoryType, int index, byte equipSlotIndex, int amount)
        {
            if (amount <= 0 || !CanDropItem())
                return false;
            CharacterItem droppingItem;
            switch (inventoryType)
            {
                case InventoryType.NonEquipItems:
                    if (index >= NonEquipItems.Count)
                        return false;
                    droppingItem = NonEquipItems[index];
                    break;
                case InventoryType.EquipItems:
                    if (index >= EquipItems.Count || !CanUnEquipItem())
                        return false;
                    droppingItem = EquipItems[index];
                    break;
                case InventoryType.EquipWeaponRight:
                    if (index >= SelectableWeaponSets.Count || !CanUnEquipItem())
                        return false;
                    droppingItem = SelectableWeaponSets[equipSlotIndex].rightHand;
                    break;
                case InventoryType.EquipWeaponLeft:
                    if (index >= SelectableWeaponSets.Count || !CanUnEquipItem())
                        return false;
                    droppingItem = SelectableWeaponSets[equipSlotIndex].leftHand;
                    break;
                default:
                    return false;
            }
            if (droppingItem.IsEmptySlot() || amount > droppingItem.amount)
                return false;
            RPC(CmdDropItem, inventoryType, index, equipSlotIndex, amount);
            return true;
        }

        public bool CallRpcOnDead()
        {
            RPC(RpcOnDead);
            return true;
        }

        public bool CallRpcOnRespawn()
        {
            RPC(RpcOnRespawn);
            return true;
        }

        public bool CallRpcOnLevelUp()
        {
            RPC(RpcOnLevelUp, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
            return true;
        }

        public bool CallCmdUnSummon(uint objectId)
        {
            RPC(CmdUnSummon, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, objectId);
            return true;
        }
    }
}
