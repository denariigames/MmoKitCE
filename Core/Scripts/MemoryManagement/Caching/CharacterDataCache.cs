using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    public class CharacterDataCache
    {
        private static ProfilerMarker s_profilerMarker = new ProfilerMarker("CharacterDataCache - GetCaches");

        public bool IsRecaching { get; private set; }
        private CharacterStats _stats;
        public CharacterStats Stats => _stats;
        public Dictionary<Attribute, float> Attributes { get; private set; }
        public Dictionary<DamageElement, float> Resistances { get; private set; }
        public Dictionary<DamageElement, float> Armors { get; private set; }
        public CharacterItem RightHandItem { get; private set; }
        public DamageInfo RightHandDamageInfo { get; private set; }
        public bool IsRightHandItemAvailable { get; private set; }
        public Dictionary<DamageElement, MinMaxFloat> RightHandDamages { get; private set; }
        public KeyValuePair<DamageElement, MinMaxFloat>? RightHandWeaponDamage { get; private set; }
        public CharacterItem LeftHandItem { get; private set; }
        public DamageInfo LeftHandDamageInfo { get; private set; }
        public bool IsLeftHandItemAvailable { get; private set; }
        public Dictionary<DamageElement, MinMaxFloat> LeftHandDamages { get; private set; }
        public KeyValuePair<DamageElement, MinMaxFloat>? LeftHandWeaponDamage { get; private set; }
        public Dictionary<DamageElement, MinMaxFloat> IncreaseDamages { get; private set; }
        public Dictionary<DamageElement, MinMaxFloat> IncreaseDamagesRate { get; private set; }
        public Dictionary<BaseSkill, int> Skills { get; private set; }
        public Dictionary<StatusEffect, float> StatusEffectResistances { get; private set; }
        public Dictionary<EquipmentSet, int> EquipmentSets { get; private set; }
        public List<BaseWeaponAbility> RightHandWeaponAbilities { get; private set; }
        public Dictionary<string, int> RightHandWeaponAbilityIndexes { get; private set; }
        public List<BaseWeaponAbility> LeftHandWeaponAbilities { get; private set; }
        public Dictionary<string, int> LeftHandWeaponAbilityIndexes { get; private set; }
        public int MaxHp => (int)_stats.hp;
        public int MaxMp => (int)_stats.mp;
        public int MaxStamina => (int)_stats.stamina;
        public int MaxFood => (int)_stats.food;
        public int MaxWater => (int)_stats.water;
        public float AtkSpeed => _stats.atkSpeed;
        public float MoveSpeed => _stats.moveSpeed;
        public float SprintSpeed => _stats.sprintSpeed;
        public float GoldRate => _stats.goldRate;
        public float ExpRate => _stats.expRate;
        public float ItemDropRate => _stats.itemDropRate;
        public float JumpHeight => _stats.jumpHeight;
        public float HeadDamageAbsorbs => _stats.headDamageAbsorbs;
        public float BodyDamageAbsorbs => _stats.bodyDamageAbsorbs;
        public float FallDamageAbsorbs => _stats.fallDamageAbsorbs;
        public float GravityRate => _stats.gravityRate;
        public float ProtectedSlotLimit => _stats.protectedSlotLimit;
        public float AmmoCapacity => _stats.ammoCapacity;
        public float RecoilModifier => _stats.recoilModifier;
        public float RecoilRate => _stats.recoilRate;
        public float RateOfFire => _stats.rateOfFire;
        public float ReloadDuration => _stats.reloadDuration;
        public float FireSpreadRangeRate => _stats.fireSpreadRangeRate;
        public float FireSpread => _stats.fireSpread;
        public float DecreaseFoodDecreation => _stats.decreaseFoodDecreation;
        public float DecreaseWaterDecreation => _stats.decreaseWaterDecreation;
        public float DecreaseStaminaDecreation => _stats.decreaseStaminaDecreation;
        public float BuyItemPriceRate => _stats.buyItemPriceRate;
        public float SellItemPriceRate => _stats.sellItemPriceRate;
        public float BaseMoveSpeed { get; private set; }
        public float BaseSprintSpeed { get; private set; }
        public float TotalItemWeight { get; private set; }
        public int TotalItemSlot { get; private set; }
        public float LimitItemWeight { get; private set; }
        public int LimitItemSlot { get; private set; }
        public bool DisallowMove { get; private set; }
        public bool DisallowSprint { get; private set; }
        public bool DisallowWalk { get; private set; }
        public bool DisallowJump { get; private set; }
        public bool DisallowDash { get; private set; }
        public bool DisallowCrouch { get; private set; }
        public bool DisallowCrawl { get; private set; }
        public bool DisallowAttack { get; private set; }
        public bool DisallowUseSkill { get; private set; }
        public bool DisallowUseItem { get; private set; }
        public bool FreezeAnimation { get; private set; }
        public bool IsHide { get; private set; }
        public bool IsRevealsHide { get; private set; }
        public bool IsBlind { get; private set; }
        public bool MuteFootstepSound { get; private set; }
        public bool IsOverweight { get; private set; }
        public bool HavingChanceToRemoveBuffWhenAttack { get; private set; }
        public bool HavingChanceToRemoveBuffWhenAttacked { get; private set; }
        public bool HavingChanceToRemoveBuffWhenUseSkill { get; private set; }
        public bool HavingChanceToRemoveBuffWhenUseItem { get; private set; }
        public bool HavingChanceToRemoveBuffWhenPickupItem { get; private set; }
        public int BattlePoints { get; private set; }

        public CharacterDataCache()
        {
            RightHandWeaponAbilities = new List<BaseWeaponAbility>();
            RightHandWeaponAbilityIndexes = new Dictionary<string, int>();
            LeftHandWeaponAbilities = new List<BaseWeaponAbility>();
            LeftHandWeaponAbilityIndexes = new Dictionary<string, int>();
        }

        ~CharacterDataCache()
        {
            CleanCacheData();
            RightHandWeaponAbilities.Clear();
            RightHandWeaponAbilities = null;
            RightHandWeaponAbilityIndexes.Clear();
            RightHandWeaponAbilityIndexes = null;
            LeftHandWeaponAbilities.Clear();
            LeftHandWeaponAbilities = null;
            LeftHandWeaponAbilityIndexes.Clear();
            LeftHandWeaponAbilityIndexes = null;
        }

        private void CleanCacheData()
        {
            // Release buffs
            if (IncreaseDamages != null)
            {
                CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(IncreaseDamages);
                IncreaseDamages = null;
            }
            if (IncreaseDamagesRate != null)
            {
                CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(IncreaseDamagesRate);
                IncreaseDamagesRate = null;
            }

            // Release results            
            if (Attributes != null)
            {
                CollectionPool<Dictionary<Attribute, float>, KeyValuePair<Attribute, float>>.Release(Attributes);
                Attributes = null;
            }
            if (Resistances != null)
            {
                CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(Resistances);
                Resistances = null;
            }
            if (Armors != null)
            {
                CollectionPool<Dictionary<DamageElement, float>, KeyValuePair<DamageElement, float>>.Release(Armors);
                Armors = null;
            }
            if (StatusEffectResistances != null)
            {
                CollectionPool<Dictionary<StatusEffect, float>, KeyValuePair<StatusEffect, float>>.Release(StatusEffectResistances);
                StatusEffectResistances = null;
            }
            if (Skills != null)
            {
                CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Release(Skills);
                Skills = null;
            }
            if (RightHandDamages != null)
            {
                CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(RightHandDamages);
                RightHandDamages = null;
            }
            if (LeftHandDamages != null)
            {
                CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Release(LeftHandDamages);
                LeftHandDamages = null;
            }
            if (EquipmentSets != null)
            {
                CollectionPool<Dictionary<EquipmentSet, int>, KeyValuePair<EquipmentSet, int>>.Release(EquipmentSets);
                EquipmentSets = null;
            }
        }

        public CharacterDataCache MarkToMakeCaches()
        {
            IsRecaching = true;
            return this;
        }

        private void SetStats(CharacterStats stats)
        {
            _stats = stats;
        }

        private void SetAttributes(Dictionary<Attribute, float> attributes)
        {
            Attributes = attributes;
        }

        private void SetResistances(Dictionary<DamageElement, float> resistances)
        {
            Resistances = resistances;
        }

        private void SetArmors(Dictionary<DamageElement, float> armors)
        {
            Armors = armors;
        }

        private void SetRightHandDamages(Dictionary<DamageElement, MinMaxFloat> rightHandDamages)
        {
            RightHandDamages = rightHandDamages;
        }

        private void SetRightHandWeaponDamage(KeyValuePair<DamageElement, MinMaxFloat> rightHandDamage)
        {
            RightHandWeaponDamage = rightHandDamage;
        }

        private void SetLeftHandDamages(Dictionary<DamageElement, MinMaxFloat> leftHandDamages)
        {
            LeftHandDamages = leftHandDamages;
        }

        private void SetLeftHandWeaponDamage(KeyValuePair<DamageElement, MinMaxFloat> leftHandDamage)
        {
            LeftHandWeaponDamage = leftHandDamage;
        }

        private void SetIncreaseDamages(Dictionary<DamageElement, MinMaxFloat> increaseDamages)
        {
            IncreaseDamages = increaseDamages;
        }

        private void SetIncreaseDamagesRate(Dictionary<DamageElement, MinMaxFloat> increaseDamagesRate)
        {
            IncreaseDamagesRate = increaseDamagesRate;
        }

        private void SetSkills(Dictionary<BaseSkill, int> skills)
        {
            Skills = skills;
        }

        private void SetStatusEffectResistances(Dictionary<StatusEffect, float> statusEffectResistances)
        {
            StatusEffectResistances = statusEffectResistances;
        }

        private void SetEquipmentSets(Dictionary<EquipmentSet, int> equipmentSets)
        {
            EquipmentSets = equipmentSets;
        }

        public CharacterDataCache GetCaches(ICharacterData characterData)
        {
            using (s_profilerMarker.Auto())
            {
                // Don't make cache if not needed
                if (!IsRecaching)
                    return this;

                IsRecaching = false;
                CleanCacheData();
                RightHandWeaponAbilities.Clear();
                RightHandWeaponAbilityIndexes.Clear();
                LeftHandWeaponAbilities.Clear();
                LeftHandWeaponAbilityIndexes.Clear();

                int oldBattlePoints = BattlePoints;

                CalculatedBuff tempCalculatedBuff;
                bool isOverrideDamageInfo = false;
                DamageInfo overrideDamageInfo = null;
                bool isOverrideSkills = false;
                for (int i = characterData.Buffs.Count - 1; i >= 0; --i)
                {
                    tempCalculatedBuff = characterData.Buffs[i].GetBuff();
                    if (!isOverrideDamageInfo && tempCalculatedBuff.IsOverrideDamageInfo())
                    {
                        isOverrideDamageInfo = true;
                        overrideDamageInfo = tempCalculatedBuff.GetOverrideDamageInfo();
                    }
                    if (!isOverrideSkills && tempCalculatedBuff.IsOverrideSkills())
                    {
                        isOverrideSkills = true;
                        Skills = CollectionPool<Dictionary<BaseSkill, int>, KeyValuePair<BaseSkill, int>>.Get();
                        GameDataHelpers.CombineSkills(Skills, tempCalculatedBuff.GetOverrideSkills());
                    }
                    if (isOverrideDamageInfo && isOverrideSkills)
                        break;
                }

                characterData.GetAllStats(true, true, true,
                    SetStats,
                    SetAttributes,
                    SetResistances,
                    SetArmors,
                    SetRightHandDamages,
                    SetRightHandWeaponDamage,
                    SetLeftHandDamages,
                    SetLeftHandWeaponDamage,
                    isOverrideSkills ? null : SetSkills,
                    SetStatusEffectResistances,
                    SetEquipmentSets,
                    onGetIncreasingDamages: SetIncreaseDamages,
                    onGetIncreasingDamagesRate: SetIncreaseDamagesRate,
                    willReleaseAttributes: false,
                    willReleaseResistances: false,
                    willReleaseArmors: false,
                    willReleaseRightHandDamages: false,
                    willReleaseLeftHandDamages: false,
                    willReleaseSkills: isOverrideSkills,
                    willReleaseStatusEffectResistances: false,
                    willReleaseEquipmentSets: false,
                    willReleaseBuffDamages: false,
                    willReleaseBuffDamagesRate: false);

                if (characterData.GetDatabase() != null)
                    BaseMoveSpeed = characterData.GetDatabase().Stats.baseStats.moveSpeed;

                if (characterData.GetDatabase() != null)
                    BaseSprintSpeed = characterData.GetDatabase().Stats.baseStats.sprintSpeed;

                TotalItemWeight = GameInstance.Singleton.GameplayRule.GetTotalWeight(characterData, _stats);
                TotalItemSlot = GameInstance.Singleton.GameplayRule.GetTotalSlot(characterData, _stats);
                LimitItemWeight = GameInstance.Singleton.GameplayRule.GetLimitWeight(characterData, _stats);
                LimitItemSlot = GameInstance.Singleton.GameplayRule.GetLimitSlot(characterData, _stats);

                IsOverweight = (GameInstance.Singleton.IsLimitInventorySlot && TotalItemSlot > LimitItemSlot) || (GameInstance.Singleton.IsLimitInventoryWeight && TotalItemWeight > LimitItemWeight);
                DisallowMove = false;
                DisallowSprint = false;
                DisallowWalk = false;
                DisallowJump = false;
                DisallowDash = false;
                DisallowCrouch = false;
                DisallowCrawl = false;
                DisallowAttack = false;
                DisallowUseSkill = false;
                DisallowUseItem = false;
                FreezeAnimation = false;
                IsHide = false;
                IsRevealsHide = false;
                IsBlind = false;
                MuteFootstepSound = false;
                HavingChanceToRemoveBuffWhenAttack = false;
                HavingChanceToRemoveBuffWhenAttacked = false;
                HavingChanceToRemoveBuffWhenUseSkill = false;
                HavingChanceToRemoveBuffWhenUseItem = false;
                HavingChanceToRemoveBuffWhenPickupItem = false;

                bool allAilmentsWereApplied = false;
                if (characterData is BasePlayerCharacterEntity playerCharacterEntity)
                {
                    if (!allAilmentsWereApplied && playerCharacterEntity.PassengingVehicleEntity != null)
                    {
                        UpdateAppliedAilments(playerCharacterEntity.PassengingVehicleEntity.GetBuff());
                        allAilmentsWereApplied = AllAilmentsWereApplied();
                    }
                }

                if (!allAilmentsWereApplied)
                {
                    foreach (CharacterBuff characterBuff in characterData.Buffs)
                    {
                        UpdateAppliedAilments(characterBuff.GetBuff());
                        allAilmentsWereApplied = AllAilmentsWereApplied();
                        if (allAilmentsWereApplied)
                            break;
                    }
                }

                if (!allAilmentsWereApplied)
                {
                    foreach (CharacterSummon characterBuff in characterData.Summons)
                    {
                        UpdateAppliedAilments(characterBuff.GetBuff());
                        allAilmentsWereApplied = AllAilmentsWereApplied();
                        if (allAilmentsWereApplied)
                            break;
                    }
                }

                float tempTotalBattlePoint = 0f;

                foreach (Attribute tempAttribute in Attributes.Keys)
                {
                    if (tempAttribute == null)
                        continue;
                    float amount = Attributes[tempAttribute];
                    tempTotalBattlePoint += tempAttribute.BattlePointScore * amount;
                }

                foreach (BaseSkill tempSkill in Skills.Keys)
                {
                    if (tempSkill == null)
                        continue;
                    int skillLevel = Skills[tempSkill];
                    tempTotalBattlePoint += tempSkill.battlePointScore * skillLevel;
                    // Apply ailments by passive buff only
                    if (!allAilmentsWereApplied && tempSkill.IsPassive && tempSkill.TryGetBuff(out Buff tempBuff))
                    {
                        UpdateAppliedAilments(new CalculatedBuff(tempBuff, skillLevel));
                        allAilmentsWereApplied = AllAilmentsWereApplied();
                    }
                }

                foreach (DamageElement tempDamageElement in Resistances.Keys)
                {
                    if (tempDamageElement == null)
                        continue;
                    float amount = Resistances[tempDamageElement];
                    tempTotalBattlePoint += tempDamageElement.ResistanceBattlePointScore * amount;
                }

                foreach (DamageElement tempDamageElement in Armors.Keys)
                {
                    if (tempDamageElement == null)
                        continue;
                    float amount = Armors[tempDamageElement];
                    tempTotalBattlePoint += tempDamageElement.ArmorBattlePointScore * amount;
                }

                foreach (DamageElement tempDamageElement in RightHandDamages.Keys)
                {
                    if (tempDamageElement == null)
                        continue;
                    MinMaxFloat amount = RightHandDamages[tempDamageElement];
                    tempTotalBattlePoint += tempDamageElement.DamageBattlePointScore * (amount.min + amount.max) * 0.5f;
                }

                foreach (DamageElement tempDamageElement in LeftHandDamages.Keys)
                {
                    if (tempDamageElement == null)
                        continue;
                    MinMaxFloat amount = LeftHandDamages[tempDamageElement];
                    tempTotalBattlePoint += tempDamageElement.DamageBattlePointScore * (amount.min + amount.max) * 0.5f;
                }

                if (characterData.EquipWeapons.NotEmptyRightHandSlot())
                {
                    tempTotalBattlePoint += characterData.EquipWeapons.rightHand.GetWeaponDamageBattlePoints();
                }

                if (characterData.EquipWeapons.NotEmptyLeftHandSlot())
                {
                    tempTotalBattlePoint += characterData.EquipWeapons.leftHand.GetWeaponDamageBattlePoints();
                }

                tempTotalBattlePoint += GameInstance.Singleton.GameplayRule.GetBattlePointFromCharacterStats(Stats);
                BattlePoints = Mathf.CeilToInt(tempTotalBattlePoint);

                if (characterData == GameInstance.PlayingCharacter)
                {
                    int battlePointChange = BattlePoints - oldBattlePoints;
                    if (battlePointChange != 0)
                        ClientGenericActions.NotifyBattlePointsChanged(battlePointChange);
                }

                RightHandDamageInfo = null;
                IsRightHandItemAvailable = false;
                LeftHandDamageInfo = null;
                IsLeftHandItemAvailable = false;

                IWeaponItem rightWeaponItem = characterData.EquipWeapons.GetRightHandWeaponItem();
                if (rightWeaponItem != null)
                {
                    IsRightHandItemAvailable = true;
                    RightHandItem = characterData.EquipWeapons.rightHand;
                    if (isOverrideDamageInfo)
                        RightHandDamageInfo = overrideDamageInfo;
                    else
                        RightHandDamageInfo = rightWeaponItem.WeaponType.DamageInfo;
                    if (rightWeaponItem.WeaponAbilities != null && rightWeaponItem.WeaponAbilities.Length > 0)
                        RightHandWeaponAbilities.AddRange(rightWeaponItem.WeaponAbilities);
                    AddOrReplaceWeaponAbilities(RightHandWeaponAbilities, RightHandWeaponAbilityIndexes, RightHandItem.sockets);
                }

                IWeaponItem leftWeaponItem = characterData.EquipWeapons.GetLeftHandWeaponItem();
                if (leftWeaponItem != null)
                {
                    IsLeftHandItemAvailable = true;
                    LeftHandItem = characterData.EquipWeapons.leftHand;
                    if (isOverrideDamageInfo)
                        LeftHandDamageInfo = overrideDamageInfo;
                    else
                        LeftHandDamageInfo = leftWeaponItem.WeaponType.DamageInfo;
                    if (leftWeaponItem.WeaponAbilities != null && leftWeaponItem.WeaponAbilities.Length > 0)
                        LeftHandWeaponAbilities.AddRange(leftWeaponItem.WeaponAbilities);
                    AddOrReplaceWeaponAbilities(LeftHandWeaponAbilities, LeftHandWeaponAbilityIndexes, LeftHandItem.sockets);
                }

                if (!IsLeftHandItemAvailable && !IsRightHandItemAvailable)
                {
                    if (characterData is BaseMonsterCharacterEntity monsterCharacterEntity)
                    {
                        // Monster has its own damage info set to game database
                        IsRightHandItemAvailable = true;
                        RightHandItem = CharacterItem.CreateMonsterWeapon();
                        if (isOverrideDamageInfo)
                            RightHandDamageInfo = overrideDamageInfo;
                        else
                            RightHandDamageInfo = monsterCharacterEntity.CharacterDatabase.DamageInfo;
                    }
                    else
                    {
                        // No equipped weapon?, use default one
                        IsRightHandItemAvailable = true;
                        RightHandItem = CharacterItem.CreateDefaultWeapon();
                        if (isOverrideDamageInfo)
                            RightHandDamageInfo = overrideDamageInfo;
                        else
                            RightHandDamageInfo = RightHandItem.GetWeaponItem().WeaponType.DamageInfo;
                    }
                }

                return this;
            }
        }

        private static void AddOrReplaceWeaponAbilities(List<BaseWeaponAbility> weaponAbilities, Dictionary<string, int> weaponAbilityIndexes, List<int> sockets)
        {
            weaponAbilityIndexes.Clear();
            int i;
            for (i = 0; i < weaponAbilities.Count; ++i)
            {
                if (weaponAbilities[i] == null)
                    continue;
                if (weaponAbilityIndexes.ContainsKey(weaponAbilities[i].AbilityKey))
                    continue;
                weaponAbilityIndexes[weaponAbilities[i].AbilityKey] = i;
            }

            for (i = 0; i < sockets.Count; ++i)
            {
                if (!GameInstance.Items.TryGetValue(sockets[i], out BaseItem item) || !item.IsSocketEnhancer())
                    continue;

                ISocketEnhancerItem socketEnhancerItem = item as ISocketEnhancerItem;
                if (socketEnhancerItem.WeaponAbilities == null || socketEnhancerItem.WeaponAbilities.Length == 0)
                    continue;

                for (int j = 0; j < socketEnhancerItem.WeaponAbilities.Length; ++j)
                {
                    if (socketEnhancerItem.WeaponAbilities[j] == null)
                        continue;

                    string abilityKey = socketEnhancerItem.WeaponAbilities[j].AbilityKey;
                    if (!weaponAbilityIndexes.ContainsKey(abilityKey))
                    {
                        // Add a new ability
                        weaponAbilities.Add(socketEnhancerItem.WeaponAbilities[j]);
                        weaponAbilityIndexes[abilityKey] = weaponAbilities.Count - 1;
                        continue;
                    }
                    else
                    {
                        // Change old ability
                        weaponAbilities[weaponAbilityIndexes[abilityKey]] = socketEnhancerItem.WeaponAbilities[j];
                    }
                }
            }
        }

        public bool TryGetWeaponAbility(bool isLeftHand, string key, out BaseWeaponAbility ability)
        {
            return isLeftHand ? TryGetLeftHandWeaponAbility(key, out ability) : TryGetRightHandWeaponAbility(key, out ability);
        }

        public bool TryGetRightHandWeaponAbility(string key, out BaseWeaponAbility ability)
        {
            return TryGetWeaponAbility(RightHandWeaponAbilities, RightHandWeaponAbilityIndexes, key, out ability);
        }

        public bool TryGetLeftHandWeaponAbility(string key, out BaseWeaponAbility ability)
        {
            return TryGetWeaponAbility(LeftHandWeaponAbilities, LeftHandWeaponAbilityIndexes, key, out ability);
        }

        public static bool TryGetWeaponAbility(List<BaseWeaponAbility> weaponAbilities, Dictionary<string, int> weaponAbilityIndexes, string key, out BaseWeaponAbility ability)
        {
            if (weaponAbilityIndexes.TryGetValue(key, out int index))
            {
                ability = weaponAbilities[index];
                return true;
            }
            ability = null;
            return false;
        }

        public void ClearChanceToRemoveBuffWhenAttack()
        {
            HavingChanceToRemoveBuffWhenAttack = false;
        }

        public void ClearChanceToRemoveBuffWhenAttacked()
        {
            HavingChanceToRemoveBuffWhenAttacked = false;
        }

        public void ClearChanceToRemoveBuffWhenUseSkill()
        {
            HavingChanceToRemoveBuffWhenUseSkill = false;
        }

        public void ClearChanceToRemoveBuffWhenUseItem()
        {
            HavingChanceToRemoveBuffWhenUseItem = false;
        }

        public void ClearChanceToRemoveBuffWhenPickupItem()
        {
            HavingChanceToRemoveBuffWhenPickupItem = false;
        }

        #region Helper functions to get stats amount
        public float GetAttribute(string nameId)
        {
            return GetAttribute(nameId.GenerateHashId());
        }

        public float GetAttribute(int dataId)
        {
            Attribute data;
            float result;
            if (GameInstance.Attributes.TryGetValue(dataId, out data) &&
                Attributes.TryGetValue(data, out result))
                return result;
            return 0f;
        }

        public float GetResistance(string nameId)
        {
            return GetResistance(nameId.GenerateHashId());
        }

        public float GetResistance(int dataId)
        {
            DamageElement data;
            float result;
            if (GameInstance.DamageElements.TryGetValue(dataId, out data) &&
                Resistances.TryGetValue(data, out result))
                return result;
            return 0f;
        }

        public float GetArmor(string nameId)
        {
            return GetArmor(nameId.GenerateHashId());
        }

        public float GetArmor(int dataId)
        {
            DamageElement data;
            float result;
            if (GameInstance.DamageElements.TryGetValue(dataId, out data) &&
                Armors.TryGetValue(data, out result))
                return result;
            return 0f;
        }

        public MinMaxFloat GetRightHandDamages(string nameId)
        {
            return GetRightHandDamages(nameId.GenerateHashId());
        }

        public MinMaxFloat GetRightHandDamages(int dataId)
        {
            DamageElement data;
            MinMaxFloat result;
            if (GameInstance.DamageElements.TryGetValue(dataId, out data) &&
                RightHandDamages.TryGetValue(data, out result))
                return result;
            return default;
        }

        public MinMaxFloat GetLeftHandDamages(string nameId)
        {
            return GetLeftHandDamages(nameId.GenerateHashId());
        }

        public MinMaxFloat GetLeftHandDamages(int dataId)
        {
            DamageElement data;
            MinMaxFloat result;
            if (GameInstance.DamageElements.TryGetValue(dataId, out data) &&
                LeftHandDamages.TryGetValue(data, out result))
                return result;
            return default;
        }

        public int GetSkill(string nameId)
        {
            return GetSkill(nameId.GenerateHashId());
        }

        public int GetSkill(int dataId)
        {
            BaseSkill data;
            int result;
            if (GameInstance.Skills.TryGetValue(dataId, out data) &&
                Skills.TryGetValue(data, out result))
                return result;
            return 0;
        }

        public float GetStatusEffectResistance(string nameId)
        {
            return GetStatusEffectResistance(nameId.GenerateHashId());
        }

        public float GetStatusEffectResistance(int dataId)
        {
            StatusEffect data;
            float result;
            if (GameInstance.StatusEffects.TryGetValue(dataId, out data) &&
                StatusEffectResistances.TryGetValue(data, out result))
                return result;
            return 0f;
        }

        public int GetEquipmentSet(string nameId)
        {
            return GetEquipmentSet(nameId.GenerateHashId());
        }

        public int GetEquipmentSet(int dataId)
        {
            EquipmentSet data;
            int result;
            if (GameInstance.EquipmentSets.TryGetValue(dataId, out data) &&
                EquipmentSets.TryGetValue(data, out result))
                return result;
            return 0;
        }

        public void UpdateAppliedAilments(CalculatedBuff buff)
        {
            Buff tempBuff = buff.GetBuff();
            switch (tempBuff.ailment)
            {
                case AilmentPresets.Stun:
                    DisallowMove = true;
                    DisallowSprint = true;
                    DisallowWalk = true;
                    DisallowJump = true;
                    DisallowDash = true;
                    DisallowCrouch = true;
                    DisallowCrawl = true;
                    DisallowAttack = true;
                    DisallowUseSkill = true;
                    DisallowUseItem = true;
                    break;
                case AilmentPresets.Mute:
                    DisallowUseSkill = true;
                    break;
                case AilmentPresets.Freeze:
                    DisallowMove = true;
                    DisallowSprint = true;
                    DisallowWalk = true;
                    DisallowJump = true;
                    DisallowDash = true;
                    DisallowCrouch = true;
                    DisallowCrawl = true;
                    DisallowAttack = true;
                    DisallowUseSkill = true;
                    DisallowUseItem = true;
                    FreezeAnimation = true;
                    break;
                default:
                    if (tempBuff.disallowMove)
                        DisallowMove = true;
                    if (tempBuff.disallowSprint)
                        DisallowSprint = true;
                    if (tempBuff.disallowWalk)
                        DisallowWalk = true;
                    if (tempBuff.disallowJump)
                        DisallowJump = true;
                    if (tempBuff.disallowDash)
                        DisallowDash = true;
                    if (tempBuff.disallowCrouch)
                        DisallowCrouch = true;
                    if (tempBuff.disallowCrawl)
                        DisallowCrawl = true;
                    if (tempBuff.disallowAttack)
                        DisallowAttack = true;
                    if (tempBuff.disallowUseSkill)
                        DisallowUseSkill = true;
                    if (tempBuff.disallowUseItem)
                        DisallowUseItem = true;
                    if (tempBuff.freezeAnimation)
                        FreezeAnimation = true;
                    break;
            }
            if (tempBuff.isHide)
                IsHide = true;
            if (tempBuff.isRevealsHide)
                IsRevealsHide = true;
            if (tempBuff.isBlind)
                IsBlind = true;
            if (tempBuff.muteFootstepSound)
                MuteFootstepSound = true;
            if (buff.GetRemoveBuffWhenAttackChance() > 0f)
                HavingChanceToRemoveBuffWhenAttack = true;
            if (buff.GetRemoveBuffWhenAttackedChance() > 0f)
                HavingChanceToRemoveBuffWhenAttacked = true;
            if (buff.GetRemoveBuffWhenUseSkillChance() > 0f)
                HavingChanceToRemoveBuffWhenUseSkill = true;
            if (buff.GetRemoveBuffWhenUseItemChance() > 0f)
                HavingChanceToRemoveBuffWhenUseItem = true;
            if (buff.GetRemoveBuffWhenPickupItemChance() > 0f)
                HavingChanceToRemoveBuffWhenPickupItem = true;
        }

        public bool AllAilmentsWereApplied()
        {
            return DisallowMove &&
                DisallowSprint &&
                DisallowWalk &&
                DisallowJump &&
                DisallowCrouch &&
                DisallowCrawl &&
                DisallowAttack &&
                DisallowUseSkill &&
                DisallowUseItem &&
                FreezeAnimation &&
                IsHide &&
                IsRevealsHide &&
                IsBlind &&
                MuteFootstepSound &&
                HavingChanceToRemoveBuffWhenAttack &&
                HavingChanceToRemoveBuffWhenAttacked &&
                HavingChanceToRemoveBuffWhenUseSkill &&
                HavingChanceToRemoveBuffWhenUseItem &&
                HavingChanceToRemoveBuffWhenPickupItem;
        }
        #endregion
    }
}
