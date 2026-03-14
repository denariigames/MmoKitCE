using System.Collections.Generic;
using System.Linq;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterItem
    {
        public const byte CURRENT_VERSION = 2;

        public bool TryGetSocketEnhancerItemDataId(int index, out int dataId)
        {
            dataId = 0;
            if (index >= 0 && sockets != null && index < sockets.Count && sockets[index] != 0)
            {
                dataId = sockets[index];
                return true;
            }
            return false;
        }

        public bool TryGetSocketEnhancerItem(int index, out BaseItem data)
        {
            data = null;
            return TryGetSocketEnhancerItemDataId(index, out int dataId) && GameInstance.Items.TryGetValue(dataId, out data);
        }

        public BaseItem GetItem()
        {
            return MemoryManager.CharacterItems.GetItem(in this);
        }

        public IUsableItem GetUsableItem()
        {
            return MemoryManager.CharacterItems.GetUsableItem(in this);
        }

        public IEquipmentItem GetEquipmentItem()
        {
            return MemoryManager.CharacterItems.GetEquipmentItem(in this);
        }

        public IDefendEquipmentItem GetDefendItem()
        {
            return MemoryManager.CharacterItems.GetDefendItem(in this);
        }

        public IArmorItem GetArmorItem()
        {
            return MemoryManager.CharacterItems.GetArmorItem(in this);
        }

        public IWeaponItem GetWeaponItem()
        {
            return MemoryManager.CharacterItems.GetWeaponItem(in this);
        }

        public IShieldItem GetShieldItem()
        {
            return MemoryManager.CharacterItems.GetShieldItem(in this);
        }

        public IPotionItem GetPotionItem()
        {
            return MemoryManager.CharacterItems.GetPotionItem(in this);
        }

        public IAmmoItem GetAmmoItem()
        {
            return MemoryManager.CharacterItems.GetAmmoItem(in this);
        }

        public IBuildingItem GetBuildingItem()
        {
            return MemoryManager.CharacterItems.GetBuildingItem(in this);
        }

        public IPetItem GetPetItem()
        {
            return MemoryManager.CharacterItems.GetPetItem(in this);
        }

        public ISocketEnhancerItem GetSocketEnhancerItem()
        {
            return MemoryManager.CharacterItems.GetSocketEnhancerItem(in this);
        }

        public IMountItem GetMountItem()
        {
            return MemoryManager.CharacterItems.GetMountItem(in this);
        }

        public ISkillItem GetSkillItem()
        {
            return MemoryManager.CharacterItems.GetSkillItem(in this);
        }

        public int GetMaxStack()
        {
            return GetItem() == null ? 0 : GetItem().MaxStack;
        }

        public float GetMaxDurability()
        {
            return GetEquipmentItem() == null ? 0f : GetEquipmentItem().MaxDurability;
        }

        public bool GetDestroyIfBroken()
        {
            return GetEquipmentItem() == null ? false : GetEquipmentItem().DestroyIfBroken;
        }

        public bool IsFull()
        {
            return amount >= GetMaxStack();
        }

        public bool IsBroken()
        {
            return GetMaxDurability() > 0 && durability <= 0;
        }

        public bool IsLocked()
        {
            return lockRemainsDuration > 0;
        }

        public bool IsAmmoEmpty()
        {
            IWeaponItem item = GetWeaponItem();
            if (item == null || item.AmmoCapacity <= 0)
                return false;
            return ammo <= 0;
        }

        public bool IsAmmoFull(ICharacterData characterData)
        {
            IWeaponItem item = GetWeaponItem();
            if (item == null || item.AmmoCapacity <= 0)
                return true;
            return ammo >= GetAmmoCapacity(characterData);
        }

        public bool IsRewardingItem()
        {
            BaseItem item = GetItem();
            return item is RandomRewardingItem || item is SpecificRewardingItem;
        }

        public bool IsCharacterNameChangeItem()
        {
            return GetItem() is CharacterNameChangeItem;
        }

        public bool HasAmmoToReload(ICharacterData character, out int reloadingAmmoDataId, out int amountInInventory)
        {
            reloadingAmmoDataId = 0;
            amountInInventory = 0;
            IWeaponItem item = GetWeaponItem();
            if (item == null)
                return false;
            if (item.AmmoItemIds.Count > 0)
            {
                CharacterItem tempCharacterItem;
                // Try find from inventory which it is the same kind with in the weapon
                if (item.AmmoItemIds.Contains(ammoDataId))
                {
                    reloadingAmmoDataId = ammoDataId;
                    for (int i = 0; i < character.NonEquipItems.Count; ++i)
                    {
                        tempCharacterItem = character.NonEquipItems[i];
                        if (tempCharacterItem.IsEmptySlot())
                            continue;
                        if (tempCharacterItem.dataId != ammoDataId)
                            continue;
                        amountInInventory += tempCharacterItem.amount;
                    }
                    if (item.NoAmmoDataIdChange)
                        reloadingAmmoDataId = -1;
                    if (amountInInventory > 0)
                        return true;
                }
                reloadingAmmoDataId = 0;
                // Try find other ammo items from inventory
                for (int i = 0; i < character.NonEquipItems.Count; ++i)
                {
                    tempCharacterItem = character.NonEquipItems[i];
                    if (tempCharacterItem.IsEmptySlot())
                        continue;
                    if (!item.AmmoItemIds.Contains(tempCharacterItem.dataId))
                        continue;
                    if (reloadingAmmoDataId == 0)
                        reloadingAmmoDataId = tempCharacterItem.dataId;
                    if (reloadingAmmoDataId == tempCharacterItem.dataId)
                        amountInInventory += tempCharacterItem.amount;
                }
                if (item.NoAmmoDataIdChange)
                    reloadingAmmoDataId = 0;
                if (amountInInventory > 0)
                    return true;
            }
            if (item.WeaponType.AmmoType != null)
            {
                // Find ammo item by ammo type
                amountInInventory = character.CountAmmos(item.WeaponType.AmmoType, out reloadingAmmoDataId);
                return true;
            }
            return false;
        }

        public int GetAmmoCapacity(ICharacterData characterData)
        {
            IWeaponItem item = GetWeaponItem();
            if (item == null)
                return 0;
            if (ammoDataId != 0 && !item.NoAmmoCapacityOverriding &&
                GameInstance.Items.TryGetValue(ammoDataId, out BaseItem prevAmmoItem) &&
                prevAmmoItem.OverrideAmmoCapacity > 0)
            {
                return prevAmmoItem.OverrideAmmoCapacity + (int)characterData.GetCaches().AmmoCapacity;
            }
            return item.AmmoCapacity + (int)characterData.GetCaches().AmmoCapacity;
        }

        public void Lock(float duration)
        {
            lockRemainsDuration = duration;
        }

        public bool ShouldRemove(long currentTime)
        {
            return expireTime > 0 && expireTime < currentTime;
        }

        public void Update(float deltaTime)
        {
            lockRemainsDuration -= deltaTime;
        }

        public float GetEquipmentStatsRate()
        {
            return GameInstance.Singleton.GameplayRule.GetEquipmentStatsRate(this);
        }

        public KeyValuePair<DamageElement, float> GetArmorAmount()
        {
            IDefendEquipmentItem item = GetDefendItem();
            if (item == null)
                return new KeyValuePair<DamageElement, float>();
            return item.GetArmorAmount(level, GetEquipmentStatsRate());
        }

        public KeyValuePair<DamageElement, MinMaxFloat> GetDamageAmount()
        {
            IWeaponItem item = GetWeaponItem();
            if (item == null)
                return new KeyValuePair<DamageElement, MinMaxFloat>();
            return item.GetDamageAmount(level, GetEquipmentStatsRate());
        }

        public float GetWeaponDamageBattlePoints()
        {
            if (GetWeaponItem() == null)
                return 0f;
            KeyValuePair<DamageElement, MinMaxFloat> kv = GetDamageAmount();
            DamageElement tempDamageElement = kv.Key;
            if (tempDamageElement == null)
                tempDamageElement = GameInstance.Singleton.DefaultDamageElement;
            MinMaxFloat amount = kv.Value;
            return tempDamageElement.DamageBattlePointScore * (amount.min + amount.max) * 0.5f;
        }

        public CalculatedItemBuff GetBuff()
        {
            return MemoryManager.CharacterItems.GetBuff(in this);
        }

        public void UpdateDurability(ICharacterData characterData, float amount)
        {
            float oldDurability = durability;
            float max = GetMaxDurability();
            bool destroying = false;
            durability += amount;
            if (durability > max)
                durability = max;
            if (durability <= 0)
            {
                durability = 0;
                if (GetDestroyIfBroken())
                    destroying = true;
            }
            if (characterData is IPlayerCharacterData playerCharacterData)
            {
                // Will write log messages only if character is player character
                GameInstance.ServerLogHandlers.LogItemDurabilityChanged(playerCharacterData, this, oldDurability, durability, destroying);
            }
        }

        public static CharacterItem Create(BaseItem item, int level = 1, int amount = 1, int? randomSeed = null)
        {
            return Create(item.DataId, level, amount, randomSeed);
        }

        public static CharacterItem Create(int dataId, int level = 1, int amount = 1, int? randomSeed = null)
        {
            CharacterItem newItem = new CharacterItem();
            newItem.id = GenericUtils.GetUniqueId();
            newItem.dataId = dataId;
            if (level <= 0)
                level = 1;
            newItem.level = level;
            newItem.amount = amount;
            newItem.durability = 0f;
            newItem.exp = 0;
            newItem.lockRemainsDuration = 0f;
            newItem.ammo = 0;
            newItem.ammoDataId = 0;
            newItem.sockets = new List<int>();
            if (GameInstance.Items.TryGetValue(dataId, out BaseItem tempItem))
            {
                if (tempItem.IsEquipment() && tempItem is IEquipmentItem equipmentItem)
                {
                    newItem.durability = equipmentItem.MaxDurability;
                    newItem.lockRemainsDuration = tempItem.LockDuration;
                    if (randomSeed.HasValue)
                    {
                        newItem.randomSeed = randomSeed.Value;
                    }
                    else
                    {
                        newItem.randomSeed = GenericUtils.RandomInt(int.MinValue, int.MaxValue);
                    }

                    if (tempItem.IsWeapon() && tempItem is IWeaponItem weaponItem)
                    {
                        // Set default ammo amount
                        if (weaponItem.AmmoItemIds.Count > 0)
                        {
                            newItem.ammoDataId = weaponItem.AmmoItemIds.First();
                            if (GameInstance.Items.TryGetValue(newItem.ammoDataId, out BaseItem ammoItem))
                            {
                                if (!weaponItem.NoAmmoCapacityOverriding && ammoItem.OverrideAmmoCapacity > 0)
                                    newItem.ammo = ammoItem.OverrideAmmoCapacity;
                                else
                                    newItem.ammo = weaponItem.AmmoCapacity;
                            }
                        }
                        else
                        {
                            newItem.ammo = weaponItem.AmmoCapacity;
                        }
                    }
                }
                if (tempItem.ExpireDuration > 0)
                {
                    switch (tempItem.ExpireDurationUnit)
                    {
                        case ETimeUnits.Days:
                            newItem.expireTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (tempItem.ExpireDuration * 60 * 60 * 24);
                            break;
                        case ETimeUnits.Hours:
                            newItem.expireTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (tempItem.ExpireDuration * 60 * 60);
                            break;
                        case ETimeUnits.Minutes:
                            newItem.expireTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (tempItem.ExpireDuration * 60);
                            break;
                        case ETimeUnits.Seconds:
                            newItem.expireTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds() + tempItem.ExpireDuration;
                            break;
                    }
                }
            }
            newItem.version = CURRENT_VERSION;
            return newItem;
        }

        public static CharacterItem CreateEmptySlot()
        {
            return Create(0, 1, 0);
        }

        public static CharacterItem CreateDefaultWeapon()
        {
            CharacterItem characterItem = Create(GameInstance.Singleton.DefaultWeaponItem.DataId, 1, 1, 0);
            characterItem.id = GameDataConst.DEFAULT_WEAPON_ID;
            return characterItem;
        }

        public static CharacterItem CreateMonsterWeapon()
        {
            CharacterItem characterItem = Create(GameInstance.Singleton.MonsterWeaponItem.DataId, 1, 1, 0);
            characterItem.id = GameDataConst.DEFAULT_WEAPON_ID;
            return characterItem;
        }
    }

    [System.Serializable]
    public class SyncListCharacterItem : LiteNetLibSyncList<CharacterItem>
    {
    }
}
