using LiteNetLibManager;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class ClientInventoryHandlersExtensions
    {
        public static void RequestEquipItem(this IClientInventoryHandlers handlers, IPlayerCharacterData playerCharacter, int nonEquipIndex, byte equipWeaponSet, ResponseDelegate<ResponseEquipArmorMessage> responseEquipArmor, ResponseDelegate<ResponseEquipWeaponMessage> responseEquipWeapon)
        {
            if (nonEquipIndex < 0 || nonEquipIndex >= playerCharacter.NonEquipItems.Count)
                return;

            CharacterItem equippingItem = playerCharacter.NonEquipItems[nonEquipIndex];
            IArmorItem equippingArmorItem = equippingItem.GetArmorItem();
            IWeaponItem equippingWeaponItem = equippingItem.GetWeaponItem();
            IShieldItem equippingShieldItem = equippingItem.GetShieldItem();
            if (equippingWeaponItem != null)
            {
                List<byte> equippableSlotIndexes = equippingWeaponItem.GetEquippableSetIndexes();
                if (equippableSlotIndexes?.Count > 0 && !equippableSlotIndexes.Contains(equipWeaponSet))
                    equipWeaponSet = equippingWeaponItem.WeaponType.EquippableSetIndexes[0];

                if (equippingWeaponItem.GetEquipType() == WeaponItemEquipType.DualWieldable)
                {
                    IWeaponItem rightWeapon = playerCharacter.EquipWeapons.GetRightHandWeaponItem();

                    if ((rightWeapon != null && rightWeapon.GetEquipType() == WeaponItemEquipType.DualWieldable && equippingWeaponItem.GetDualWieldRestriction() != DualWieldRestriction.OffHandRestricted) ||
                        (rightWeapon == null && equippingWeaponItem.GetDualWieldRestriction() == DualWieldRestriction.MainHandRestricted))
                    {
                        // Has equipped weapon in main-hand slot or if selected weapon is main-head restricted, try equip selected weapon to off-hand slot
                        handlers.RequestEquipWeapon(new RequestEquipWeaponMessage()
                        {
                            nonEquipIndex = nonEquipIndex,
                            equipWeaponSet = equipWeaponSet,
                            isLeftHand = true,
                        }, responseEquipWeapon);
                    }
                    else
                    {
                        handlers.RequestEquipWeapon(new RequestEquipWeaponMessage()
                        {
                            nonEquipIndex = nonEquipIndex,
                            equipWeaponSet = equipWeaponSet,
                            isLeftHand = false,
                        }, responseEquipWeapon);
                    }
                }
                else if (equippingWeaponItem.GetEquipType() == WeaponItemEquipType.OffHandOnly)
                {
                    handlers.RequestEquipWeapon(new RequestEquipWeaponMessage()
                    {
                        nonEquipIndex = nonEquipIndex,
                        equipWeaponSet = equipWeaponSet,
                        isLeftHand = true,
                    }, responseEquipWeapon);
                }
                else
                {
                    handlers.RequestEquipWeapon(new RequestEquipWeaponMessage()
                    {
                        nonEquipIndex = nonEquipIndex,
                        equipWeaponSet = equipWeaponSet,
                        isLeftHand = false,
                    }, responseEquipWeapon);
                }
            }
            else if (equippingShieldItem != null)
            {
                // Shield can equip at left-hand only
                handlers.RequestEquipWeapon(new RequestEquipWeaponMessage()
                {
                    nonEquipIndex = nonEquipIndex,
                    equipWeaponSet = equipWeaponSet,
                    isLeftHand = true,
                }, responseEquipWeapon);
            }
            else if (equippingArmorItem != null)
            {
                // Find equip slot index
                // Example: if there is 2 ring slots
                // If first ring slot is empty, equip to first ring slot
                // The if first ring slot is not empty, equip to second ring slot
                // Do not equip to third ring slot because it's not allowed to do that
                byte equippingSlotIndex = (byte)(equippingArmorItem.ArmorType.EquippableSlots - 1);
                bool[] equippedSlots = new bool[equippingArmorItem.ArmorType.EquippableSlots];
                CharacterItem equippedItem;
                for (int i = 0; i < playerCharacter.EquipItems.Count; ++i)
                {
                    equippedItem = playerCharacter.EquipItems[i];
                    IArmorItem armorItem = equippedItem.GetArmorItem();
                    // If equipped item is same armor type, find which slot it is equipped
                    if (armorItem != null && armorItem.ArmorType == equippingArmorItem.ArmorType)
                        equippedSlots[equippedItem.equipSlotIndex] = true;
                }
                // Find free slot
                for (byte i = 0; i < equippedSlots.Length; ++i)
                {
                    if (!equippedSlots[i])
                    {
                        equippingSlotIndex = i;
                        break;
                    }
                }
                handlers.RequestEquipArmor(new RequestEquipArmorMessage()
                {
                    nonEquipIndex = nonEquipIndex,
                    equipSlotIndex = equippingSlotIndex,
                }, responseEquipArmor);
            }
        }

        public static void RequestEquipItem(this IClientInventoryHandlers handlers, int nonEquipIndex, InventoryType inventoryType, byte equipSlotIndex, ResponseDelegate<ResponseEquipArmorMessage> responseEquipArmor, ResponseDelegate<ResponseEquipWeaponMessage> responseEquipWeapon)
        {
            switch (inventoryType)
            {
                case InventoryType.EquipItems:
                    handlers.RequestEquipArmor(new RequestEquipArmorMessage()
                    {
                        nonEquipIndex = nonEquipIndex,
                        equipSlotIndex = equipSlotIndex,
                    }, responseEquipArmor);
                    break;
                case InventoryType.EquipWeaponRight:
                    handlers.RequestEquipWeapon(new RequestEquipWeaponMessage()
                    {
                        nonEquipIndex = nonEquipIndex,
                        equipWeaponSet = equipSlotIndex,
                        isLeftHand = false,
                    }, responseEquipWeapon);
                    break;
                case InventoryType.EquipWeaponLeft:
                    handlers.RequestEquipWeapon(new RequestEquipWeaponMessage()
                    {
                        nonEquipIndex = nonEquipIndex,
                        equipWeaponSet = equipSlotIndex,
                        isLeftHand = true,
                    }, responseEquipWeapon);
                    break;
            }
        }

        public static void RequestUnEquipItem(this IClientInventoryHandlers handlers, InventoryType inventoryType, int equipItemIndex, byte equipWeaponSet, int nonEquipIndex, ResponseDelegate<ResponseUnEquipArmorMessage> responseUnEquipArmor, ResponseDelegate<ResponseUnEquipWeaponMessage> responseUnEquipWeapon)
        {
            switch (inventoryType)
            {
                case InventoryType.EquipItems:
                    handlers.RequestUnEquipArmor(new RequestUnEquipArmorMessage()
                    {
                        equipIndex = equipItemIndex,
                        nonEquipIndex = nonEquipIndex,
                    }, responseUnEquipArmor);
                    break;
                case InventoryType.EquipWeaponRight:
                    handlers.RequestUnEquipWeapon(new RequestUnEquipWeaponMessage()
                    {
                        equipWeaponSet = equipWeaponSet,
                        isLeftHand = false,
                        nonEquipIndex = nonEquipIndex,
                    }, responseUnEquipWeapon);
                    break;
                case InventoryType.EquipWeaponLeft:
                    handlers.RequestUnEquipWeapon(new RequestUnEquipWeaponMessage()
                    {
                        equipWeaponSet = equipWeaponSet,
                        isLeftHand = true,
                        nonEquipIndex = nonEquipIndex,
                    }, responseUnEquipWeapon);
                    break;
            }
        }
    }
}
