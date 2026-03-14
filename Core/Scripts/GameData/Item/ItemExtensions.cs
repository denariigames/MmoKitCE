using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class ItemExtensions
    {
        #region Item Type Extension

        public static bool IsDefendEquipment<T>(this T item)
            where T : IItem
        {
            return item != null && (item.IsArmor() || item.IsShield());
        }

        public static bool IsEquipment<T>(this T item)
            where T : IItem
        {
            return item != null && (item.IsDefendEquipment() || item.IsWeapon());
        }

        public static bool IsUsable<T>(this T item)
            where T : IItem
        {
            return item != null && (item.IsPotion() || item.IsBuilding() || item.IsPet() || item.IsMount() || item.IsSkill());
        }

        public static bool IsJunk<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Junk;
        }

        public static bool IsArmor<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Armor;
        }

        public static bool IsShield<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Shield;
        }

        public static bool IsWeapon<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Weapon;
        }

        public static bool IsPotion<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Potion;
        }

        public static bool IsAmmo<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Ammo;
        }

        public static bool IsBuilding<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Building;
        }

        public static bool IsPet<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Pet;
        }

        public static bool IsSocketEnhancer<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.SocketEnhancer;
        }

        public static bool IsMount<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Mount;
        }

        public static bool IsSkill<T>(this T item)
            where T : IItem
        {
            return item != null && item.ItemType == ItemType.Skill;
        }
        #endregion

        #region Ammo Extension
        public static void GetIncreaseDamages(this IAmmoItem ammoItem, Dictionary<DamageElement, MinMaxFloat> result)
        {
            result.Clear();
            if (ammoItem != null && ammoItem.IsAmmo())
                GameDataHelpers.CombineDamages(ammoItem.IncreaseDamages, result, 1, 1f);
        }
        #endregion

        #region Equipment Extension
        public static CharacterStats GetIncreaseStats<T>(this T equipmentItem, int level)
            where T : IEquipmentItem
        {
            if (equipmentItem == null || !equipmentItem.IsEquipment())
                return new CharacterStats();
            return equipmentItem.IncreaseStats.GetCharacterStats(level);
        }

        public static CharacterStats GetIncreaseStatsRate<T>(this T equipmentItem, int level)
            where T : IEquipmentItem
        {
            if (equipmentItem == null || !equipmentItem.IsEquipment())
                return new CharacterStats();
            return equipmentItem.IncreaseStatsRate.GetCharacterStats(level);
        }

        public static void GetIncreaseAttributes<T>(this T equipmentItem, int level, Dictionary<Attribute, float> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineAttributes(equipmentItem.IncreaseAttributes, result, level, 1f);
        }

        public static void GetIncreaseAttributesRate<T>(this T equipmentItem, int level, Dictionary<Attribute, float> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineAttributes(equipmentItem.IncreaseAttributesRate, result, level, 1f);
        }

        public static void GetIncreaseResistances<T>(this T equipmentItem, int level, Dictionary<DamageElement, float> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineResistances(equipmentItem.IncreaseResistances, result, level, 1f);
        }

        public static void GetIncreaseArmors<T>(this T equipmentItem, int level, Dictionary<DamageElement, float> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineArmors(equipmentItem.IncreaseArmors, result, level, 1f);
        }

        public static void GetIncreaseArmorsRate<T>(this T equipmentItem, int level, Dictionary<DamageElement, float> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineArmors(equipmentItem.IncreaseArmorsRate, result, level, 1f);
        }

        public static void GetIncreaseDamages<T>(this T equipmentItem, int level, Dictionary<DamageElement, MinMaxFloat> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineDamages(equipmentItem.IncreaseDamages, result, level, 1f);
        }

        public static void GetIncreaseDamagesRate<T>(this T equipmentItem, int level, Dictionary<DamageElement, MinMaxFloat> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineDamages(equipmentItem.IncreaseDamagesRate, result, level, 1f);
        }

        public static void GetIncreaseSkills<T>(this T equipmentItem, int level, Dictionary<BaseSkill, int> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineSkills(equipmentItem.IncreaseSkills, result, level, 1f);
        }

        public static void GetIncreaseStatusEffectResistances<T>(this T equipmentItem, int level, Dictionary<StatusEffect, float> result)
            where T : IEquipmentItem
        {
            result.Clear();
            if (equipmentItem != null && equipmentItem.IsEquipment())
                GameDataHelpers.CombineStatusEffectResistances(equipmentItem.IncreaseStatusEffectResistances, result, level, 1f);
        }

        public static void ApplySelfStatusEffectsWhenAttacking<T>(this T equipmentItem, int level, EntityInfo applier, CharacterItem weapon, BaseCharacterEntity target)
            where T : IEquipmentItem
        {
            if (level <= 0 || target == null || equipmentItem == null || !equipmentItem.IsEquipment())
                return;
            equipmentItem.SelfStatusEffectsWhenAttacking.ApplyStatusEffect(level, applier, weapon, target);
        }

        public static void ApplyEnemyStatusEffectsWhenAttacking<T>(this T equipmentItem, int level, EntityInfo applier, CharacterItem weapon, BaseCharacterEntity target)
            where T : IEquipmentItem
        {
            if (level <= 0 || target == null || equipmentItem == null || !equipmentItem.IsEquipment())
                return;
            equipmentItem.EnemyStatusEffectsWhenAttacking.ApplyStatusEffect(level, applier, weapon, target);
        }

        public static void ApplySelfStatusEffectsWhenAttacked<T>(this T equipmentItem, int level, EntityInfo applier, BaseCharacterEntity target)
            where T : IEquipmentItem
        {
            if (level <= 0 || target == null || equipmentItem == null || !equipmentItem.IsEquipment())
                return;
            equipmentItem.SelfStatusEffectsWhenAttacked.ApplyStatusEffect(level, applier, CharacterItem.Empty, target);
        }

        public static void ApplyEnemyStatusEffectsWhenAttacked<T>(this T equipmentItem, int level, EntityInfo applier, BaseCharacterEntity target)
            where T : IEquipmentItem
        {
            if (level <= 0 || target == null || equipmentItem == null || !equipmentItem.IsEquipment())
                return;
            equipmentItem.EnemyStatusEffectsWhenAttacked.ApplyStatusEffect(level, applier, CharacterItem.Empty, target);
        }

        public static int IndexOfSocket<T>(this T equipmentItem, SocketEnhancerType type)
            where T : IEquipmentItem
        {
            if (equipmentItem.AvailableSocketEnhancerTypes == null ||
                equipmentItem.AvailableSocketEnhancerTypes.Length == 0)
            {
                return -1;
            }
            for (int i = 0; i < equipmentItem.AvailableSocketEnhancerTypes.Length; ++i)
            {
                if (equipmentItem.AvailableSocketEnhancerTypes[i] == type)
                    return i;
            }
            return -1;
        }
        #endregion

        #region Armor/Shield Extension
        public static KeyValuePair<DamageElement, float> GetArmorAmount<T>(this T defendItem, int level, float rate)
            where T : IDefendEquipmentItem
        {
            if (defendItem == null || !defendItem.IsDefendEquipment())
                return new KeyValuePair<DamageElement, float>();
            return GameDataHelpers.ToKeyValuePair(defendItem.ArmorAmount, level, rate);
        }

        public static string GetEquipPosition<T>(this T armorItem)
            where T : IArmorItem
        {
            if (armorItem == null || armorItem.ArmorType == null)
                return string.Empty;
            return armorItem.ArmorType.EquipPosition;
        }
        #endregion

        #region Weapon Extension
        public static WeaponItemEquipType GetEquipType<T>(this T weaponItem)
            where T : IWeaponItem
        {
            if (weaponItem == null || !weaponItem.IsWeapon() || !weaponItem.WeaponType)
                return WeaponItemEquipType.MainHandOnly;
            return weaponItem.WeaponType.EquipType;
        }

        public static DualWieldRestriction GetDualWieldRestriction<T>(this T weaponItem)
            where T : IWeaponItem
        {
            if (weaponItem == null || !weaponItem.IsWeapon() || !weaponItem.WeaponType)
                return DualWieldRestriction.None;
            return weaponItem.WeaponType.DualWieldRestriction;
        }

        public static List<byte> GetEquippableSetIndexes<T>(this T weaponItem)
            where T : IWeaponItem
        {
            if (weaponItem == null || !weaponItem.IsWeapon() || !weaponItem.WeaponType)
                return null;
            return weaponItem.WeaponType.EquippableSetIndexes;
        }

        public static KeyValuePair<DamageElement, MinMaxFloat> GetDamageAmount<T>(this T weaponItem, int itemLevel, float statsRate)
            where T : IWeaponItem
        {
            if (weaponItem == null || !weaponItem.IsWeapon())
                return new KeyValuePair<DamageElement, MinMaxFloat>();
            return GameDataHelpers.ToKeyValuePair(weaponItem.DamageAmount, itemLevel, statsRate);
        }

        public static bool TryGetWeaponItemEquipType<T>(this T weaponItem, out WeaponItemEquipType equipType)
            where T : IWeaponItem
        {
            equipType = WeaponItemEquipType.MainHandOnly;
            if (weaponItem == null || !weaponItem.IsWeapon())
                return false;
            equipType = weaponItem.GetEquipType();
            return true;
        }

        public static bool TryGetWeaponItemDualWieldRestriction<T>(this T weaponItem, out DualWieldRestriction dualWieldRestriction)
            where T : IWeaponItem
        {
            dualWieldRestriction = DualWieldRestriction.None;
            if (weaponItem == null || !weaponItem.IsWeapon())
                return false;
            dualWieldRestriction = weaponItem.GetDualWieldRestriction();
            return true;
        }

        public static WeaponType GetWeaponTypeOrDefault<T>(this T weaponItem)
            where T : IWeaponItem
        {
            if (weaponItem == null || !weaponItem.IsWeapon())
                return GameInstance.Singleton.DefaultWeaponType;
            return weaponItem.WeaponType;
        }

        public static bool IsReloadable<T>(this T weaponItem)
            where T : IWeaponItem
        {
            if (weaponItem == null || !weaponItem.IsWeapon())
                return false;
            return weaponItem.AmmoCapacity > 0;
        }
        #endregion

        #region Socket Enhancer Extension
        public static void ApplySelfStatusEffectsWhenAttacking<T>(this T socketEnhancerItem, EntityInfo applier, CharacterItem weapon, BaseCharacterEntity target)
            where T : ISocketEnhancerItem
        {
            if (target == null || socketEnhancerItem == null || !socketEnhancerItem.IsSocketEnhancer())
                return;
            socketEnhancerItem.SelfStatusEffectsWhenAttacking.ApplyStatusEffect(1, applier, weapon, target);
        }

        public static void ApplyEnemyStatusEffectsWhenAttacking<T>(this T socketEnhancerItem, EntityInfo applier, CharacterItem weapon, BaseCharacterEntity target)
            where T : ISocketEnhancerItem
        {
            if (target == null || socketEnhancerItem == null || !socketEnhancerItem.IsSocketEnhancer())
                return;
            socketEnhancerItem.EnemyStatusEffectsWhenAttacking.ApplyStatusEffect(1, applier, weapon, target);
        }

        public static void ApplySelfStatusEffectsWhenAttacked<T>(this T socketEnhancerItem, EntityInfo applier, BaseCharacterEntity target)
            where T : ISocketEnhancerItem
        {
            if (target == null || socketEnhancerItem == null || !socketEnhancerItem.IsSocketEnhancer())
                return;
            socketEnhancerItem.SelfStatusEffectsWhenAttacked.ApplyStatusEffect(1, applier, CharacterItem.Empty, target);
        }

        public static void ApplyEnemyStatusEffectsWhenAttacked<T>(this T socketEnhancerItem, EntityInfo applier, BaseCharacterEntity target)
            where T : ISocketEnhancerItem
        {
            if (target == null || socketEnhancerItem == null || !socketEnhancerItem.IsSocketEnhancer())
                return;
            socketEnhancerItem.EnemyStatusEffectsWhenAttacked.ApplyStatusEffect(1, applier, CharacterItem.Empty, target);
        }
        #endregion

        public static bool CanEquip<T>(this T item, ICharacterData character, int level, out UITextKeys gameMessage)
             where T : IEquipmentItem
        {
            gameMessage = UITextKeys.NONE;
            if (!item.IsEquipment() || character == null)
                return false;

            if (character.Level < item.Requirement.level)
            {
                gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_LEVEL;
                return false;
            }

            if (character is IPlayerCharacterData playerCharacter)
            {
                if (!item.Requirement.ClassIsAvailable(playerCharacter.DataId))
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_MATCH_CHARACTER_CLASS;
                    return false;
                }

                if (!item.Requirement.FactionIsAvailable(playerCharacter.FactionId))
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_MATCH_CHARACTER_FACTION;
                    return false;
                }
            }

            if (!character.HasEnoughAttributeAmounts(item.RequireAttributeAmounts, true, out gameMessage, out _, willReleaseAttributes: true))
                return false;

            return true;
        }

        public static bool CanAttack<T>(this T item, BaseCharacterEntity character)
             where T : IWeaponItem
        {
            if (!item.IsWeapon() || character == null)
                return false;

            AmmoType requireAmmoType = item.WeaponType.AmmoType;
            return requireAmmoType == null || character.IndexOfAmmoItem(requireAmmoType) >= 0;
        }
    }
}
