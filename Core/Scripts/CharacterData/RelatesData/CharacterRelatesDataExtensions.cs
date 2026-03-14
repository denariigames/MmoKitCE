using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MultiplayerARPG
{
    public static partial class CharacterRelatesDataExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterStats data)
        {
            return
                data.hp == 0f &&
                data.hpRecovery == 0f &&
                data.hpLeechRate == 0f &&
                data.mp == 0f &&
                data.mpRecovery == 0f &&
                data.mpLeechRate == 0f &&
                data.stamina == 0f &&
                data.staminaRecovery == 0f &&
                data.staminaLeechRate == 0f &&
                data.food == 0f &&
                data.water == 0f &&
                data.accuracy == 0f &&
                data.evasion == 0f &&
                data.criRate == 0f &&
                data.criDmgRate == 0f &&
                data.blockRate == 0f &&
                data.blockDmgRate == 0f &&
                data.moveSpeed == 0f &&
                data.sprintSpeed == 0f &&
                data.atkSpeed == 0f &&
                data.weightLimit == 0f &&
                data.slotLimit == 0f &&
                data.goldRate == 0f &&
                data.expRate == 0f &&
                data.itemDropRate == 0f &&
                data.jumpHeight == 0f &&
                data.headDamageAbsorbs == 0f &&
                data.bodyDamageAbsorbs == 0f &&
                data.fallDamageAbsorbs == 0f &&
                data.gravityRate == 0f &&
                data.protectedSlotLimit == 0f &&
                data.ammoCapacity == 0f &&
                data.recoilModifier == 0f &&
                data.recoilRate == 0f &&
                data.rateOfFire == 0f &&
                data.reloadDuration == 0f &&
                data.fireSpreadRangeRate == 0f &&
                data.fireSpread == 0f &&
                data.decreaseFoodDecreation == 0f &&
                data.decreaseWaterDecreation == 0f &&
                data.decreaseStaminaDecreation == 0f &&
                data.buyItemPriceRate == 0f &&
                data.sellItemPriceRate == 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterAttribute data)
        {
            return data.dataId == 0 || data.amount == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterBuff data)
        {
            return data.dataId == 0 || data.level <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterHotkey data)
        {
            return data.type == HotkeyType.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterItem data)
        {
            return data.dataId == 0 || data.amount <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmptySlot(this CharacterItem data)
        {
            return data.IsEmpty() || data.GetItem() == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NotEmptySlot(this CharacterItem data)
        {
            return !data.IsEmptySlot();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterQuest data)
        {
            return data.dataId == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterSkill data)
        {
            return data.dataId == 0 || data.level <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterSkillUsage data)
        {
            return data.dataId == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this CharacterSummon data)
        {
            return data.dataId == 0 || data.level <= 0;
        }

        public static bool IsDiffer(this CharacterItem data, CharacterItem anotherData,
            bool checkLevel = false,
            bool checkSockets = false,
            bool checkRandomSeed = false,
            bool checkAmmoDataId = false,
            bool checkAmmoAmount = false,
            bool checkDurability = false,
            bool checkAmount = false)
        {
            if (checkLevel && data.level != anotherData.level)
                return true;
            if (checkSockets && IsDifferSockets(data, anotherData))
                return true;
            if (checkRandomSeed && data.randomSeed != anotherData.randomSeed)
                return true;
            if (checkAmmoDataId && data.ammoDataId != anotherData.ammoDataId)
                return true;
            if (checkAmmoAmount && data.ammo != anotherData.ammo)
                return true;
            if (checkDurability && !Mathf.Approximately(data.durability, anotherData.durability))
                return true;
            if (checkAmount && data.amount != anotherData.amount)
                return true;
            return !string.Equals(data.id, anotherData.id) || data.dataId != anotherData.dataId;
        }

        public static bool IsDifferSockets(this CharacterItem data, CharacterItem anotherData)
        {
            int len1 = 0;
            int len2 = 0;
            if (data.sockets != null)
                len1 = data.sockets.Count;
            if (anotherData.sockets != null)
                len2 = anotherData.sockets.Count;
            if (len1 != len2)
                return true;
            if (len1 == 0)
                return false;
            for (int i = 0; i < data.sockets.Count; ++i)
            {
                if (data.sockets[i] != anotherData.sockets[i])
                    return true;
            }
            return false;
        }

        public static bool IsDiffer(this EquipWeapons data, EquipWeapons anotherData,
            out bool rightIsDiffer, out bool leftIsDiffer,
            bool checkLevel = false,
            bool checkSockets = false,
            bool checkRandomSeed = false,
            bool checkAmmoDataId = false,
            bool checkAmmoAmount = false,
            bool checkDurability = false,
            bool checkAmount = false)
        {
            rightIsDiffer = data.rightHand.IsDiffer(anotherData.rightHand, checkLevel, checkSockets, checkRandomSeed, checkAmmoDataId, checkAmmoAmount, checkDurability, checkAmount);
            leftIsDiffer = data.leftHand.IsDiffer(anotherData.leftHand, checkLevel, checkSockets, checkRandomSeed, checkAmmoDataId, checkAmmoAmount, checkDurability, checkAmount);
            return rightIsDiffer || leftIsDiffer;
        }

        public static IWeaponItem GetRightHandWeaponItem(this EquipWeapons equipWeapons)
        {
            if (equipWeapons.IsEmptyRightHandSlot())
                return null;
            return equipWeapons.rightHand.GetWeaponItem();
        }

        public static IEquipmentItem GetRightHandEquipmentItem(this EquipWeapons equipWeapons)
        {
            if (equipWeapons.IsEmptyRightHandSlot())
                return null;
            return equipWeapons.rightHand.GetEquipmentItem();
        }

        public static BaseItem GetRightHandItem(this EquipWeapons equipWeapons)
        {
            if (equipWeapons.IsEmptyRightHandSlot())
                return null;
            return equipWeapons.rightHand.GetItem();
        }

        public static IWeaponItem GetLeftHandWeaponItem(this EquipWeapons equipWeapons)
        {
            if (equipWeapons.IsEmptyLeftHandSlot())
                return null;
            return equipWeapons.leftHand.GetWeaponItem();
        }

        public static IShieldItem GetLeftHandShieldItem(this EquipWeapons equipWeapons)
        {
            if (equipWeapons.IsEmptyLeftHandSlot())
                return null;
            return equipWeapons.leftHand.GetShieldItem();
        }

        public static IEquipmentItem GetLeftHandEquipmentItem(this EquipWeapons equipWeapons)
        {
            if (equipWeapons.IsEmptyLeftHandSlot())
                return null;
            return equipWeapons.leftHand.GetEquipmentItem();
        }

        public static BaseItem GetLeftHandItem(this EquipWeapons equipWeapons)
        {
            if (equipWeapons.IsEmptyLeftHandSlot())
                return null;
            return equipWeapons.leftHand.GetItem();
        }

        public static bool IsEmptyRightHandSlot(this EquipWeapons equipWeapons)
        {
            return equipWeapons.rightHand.IsEmptySlot();
        }

        public static bool IsEmptyLeftHandSlot(this EquipWeapons equipWeapons)
        {
            return equipWeapons.leftHand.IsEmptySlot();
        }

        public static bool NotEmptyRightHandSlot(this EquipWeapons equipWeapons)
        {
            return !equipWeapons.IsEmptyRightHandSlot();
        }

        public static bool NotEmptyLeftHandSlot(this EquipWeapons equipWeapons)
        {
            return !equipWeapons.IsEmptyLeftHandSlot();
        }

        public static int IndexOfEmptyItemSlot(this IList<CharacterItem> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].IsEmptySlot())
                    return i;
            }
            return -1;
        }
    }
}
