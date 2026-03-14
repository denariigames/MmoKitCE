using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public partial class PlayerCharacterItemLockAndExpireComponent : BaseGameEntityComponent<BasePlayerCharacterEntity>, IManagedUpdate
    {
        public const float ITEM_UPDATE_DURATION = 1f;

        private float _updatingTime;
        private float _deltaTime;

        private void OnEnable()
        {
            UpdateManager.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public void ManagedUpdate()
        {
            if (!Entity.IsServer)
                return;

            _deltaTime = Time.unscaledDeltaTime;
            _updatingTime += _deltaTime;

            if (Entity.IsDead())
                return;

            if (_updatingTime >= ITEM_UPDATE_DURATION)
            {
                // Removing non-equip items if it should
                long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                bool haveRemovedItems = false;
                CharacterItem tempItem;
                for (int i = Entity.NonEquipItems.Count - 1; i >= 0; --i)
                {
                    tempItem = Entity.NonEquipItems[i];
                    if (tempItem.IsEmptySlot())
                    {
                        // Skip empty slot
                        continue;
                    }
                    if (tempItem.ShouldRemove(currentTime))
                    {
                        Entity.NonEquipItems.RemoveOrPlaceEmptySlot(i);
                        haveRemovedItems = true;
                    }
                    else if (tempItem.IsLocked())
                    {
                        tempItem.Update(_updatingTime);
                        Entity.NonEquipItems[i] = tempItem;
                    }
                }
                if (haveRemovedItems)
                {
                    Entity.FillEmptySlots();
                }
                for (int i = Entity.EquipItems.Count - 1; i >= 0; --i)
                {
                    tempItem = Entity.EquipItems[i];
                    if (!tempItem.IsEmptySlot() && tempItem.ShouldRemove(currentTime))
                    {
                        Entity.EquipItems.RemoveAt(i);
                    }
                }
                for (int i = Entity.SelectableWeaponSets.Count - 1; i >= 0; --i)
                {
                    bool hasUpdate = false;
                    EquipWeapons tempEquipWeapons = Entity.SelectableWeaponSets[i];
                    tempItem = tempEquipWeapons.leftHand;
                    if (!tempItem.IsEmptySlot() && tempItem.ShouldRemove(currentTime))
                    {
                        tempEquipWeapons.leftHand = new CharacterItem();
                        hasUpdate = true;
                    }
                    tempItem = tempEquipWeapons.rightHand;
                    if (!tempItem.IsEmptySlot() && tempItem.ShouldRemove(currentTime))
                    {
                        tempEquipWeapons.rightHand = new CharacterItem();
                        hasUpdate = true;
                    }
                    if (hasUpdate)
                    {
                        Entity.SelectableWeaponSets[i] = tempEquipWeapons;
                    }
                }
                _updatingTime = 0;
            }
        }
    }
}