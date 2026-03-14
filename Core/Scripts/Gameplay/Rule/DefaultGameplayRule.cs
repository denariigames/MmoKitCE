using Cysharp.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.DEFAULT_GAMEPLAY_RULE_FILE, menuName = GameDataMenuConsts.DEFAULT_GAMEPLAY_RULE_MENU, order = GameDataMenuConsts.DEFAULT_GAMEPLAY_RULE_ORDER)]
    public partial class DefaultGameplayRule : BaseGameplayRule
    {
        [Header("Levelling/Stat/Skill")]
        public float increaseStatPointEachLevel = 5;
        public float increaseSkillPointEachLevel = 1;
        [Tooltip("If this > `0`, it will increase stat point until character reached max level. If it's `20`, it will increase stat point until character reached level `20`")]
        public int increaseStatPointsUntilReachedLevel = 0;
        [Tooltip("If this > `0`, it will increase skill point until character reached max level. If it's `20`, it will increase skill point until character reached level `20`")]
        public int increaseSkillPointsUntilReachedLevel = 0;
        [Range(0f, 100f)]
        public float expLostPercentageWhenDeath = 2f;

        [Header("Stamina/Sprint")]
        public float staminaRecoveryPerSeconds = 5;
        public float staminaDecreasePerSeconds = 5;
        public float moveSpeedRateWhileOverweight = 0.05f;
        [FormerlySerializedAs("moveSpeedRateWhileSprint")]
        public float moveSpeedRateWhileSprinting = 1.5f;
        public float moveSpeedIncrementWhileSprinting = 0f;

        [Header("Walk")]
        public float moveSpeedRateWhileWalking = 0.5f;
        public float moveSpeedIncrementWhileWalking = 0f;

        [Header("Crouch")]
        public float moveSpeedRateWhileCrouching = 0.35f;
        public float moveSpeedIncrementWhileCrouching = 0f;

        [Header("Crawl")]
        public float moveSpeedRateWhileCrawling = 0.15f;
        public float moveSpeedIncrementWhileCrawling = 0f;

        [Header("Swim")]
        public float moveSpeedRateWhileSwimming = 0.5f;
        public float moveSpeedIncrementWhileSwimming = 0f;

        [Header("Hp/Mp/Food/Water")]
        public int hungryWhenFoodLowerThan = 40;
        public int thirstyWhenWaterLowerThan = 40;
        public float foodDecreasePerSeconds = 4;
        public float waterDecreasePerSeconds = 2;
        [Range(0f, 1f)]
        public float hpRecoveryRatePerSeconds = 0.05f;
        [Range(0f, 1f)]
        public float mpRecoveryRatePerSeconds = 0.05f;
        [Range(0f, 1f)]
        public float hpDecreaseRatePerSecondsWhenHungry = 0.05f;
        [Range(0f, 1f)]
        public float mpDecreaseRatePerSecondsWhenHungry = 0.05f;
        [Range(0f, 1f)]
        public float hpDecreaseRatePerSecondsWhenThirsty = 0.05f;
        [Range(0f, 1f)]
        public float mpDecreaseRatePerSecondsWhenThirsty = 0.05f;

        [Header("Durability")]
        public float normalDecreaseWeaponDurability = 0.5f;
        public float normalDecreaseShieldDurability = 0.5f;
        public float normalDecreaseArmorDurability = 0.1f;
        public float blockedDecreaseWeaponDurability = 0.5f;
        public float blockedDecreaseShieldDurability = 0.75f;
        public float blockedDecreaseArmorDurability = 0.15f;
        public float criticalDecreaseWeaponDurability = 0.75f;
        public float criticalDecreaseShieldDurability = 0.5f;
        public float criticalDecreaseArmorDurability = 0.15f;
        public float missDecreaseWeaponDurability = 0f;
        public float missDecreaseShieldDurability = 0;
        public float missDecreaseArmorDurability = 0f;

        [Header("Damage Occurs")]
        public bool alwaysHitWhenCriticalOccurs = true;

        [Header("Fall Damage")]
        [Tooltip("Character will receive damage 1% of Max Hp, when fall distance = min distance")]
        public float fallDamageMinDistance = 5;
        [Tooltip("Character will receive damage 100% of Max Hp, when fall distance >= max distance")]
        public float fallDamageMaxDistance = 20;

        [Header("Level Up")]
        public bool recoverHpWhenLevelUp;
        public bool recoverMpWhenLevelUp;
        public bool recoverFoodWhenLevelUp;
        public bool recoverWaterWhenLevelUp;
        public bool recoverStaminaWhenLevelUp;

        [Header("Battle Points Score")]
        public float hpBattlePointScore = 5;
        public float hpRecoveryBattlePointScore = 2;
        public float hpLeechRateBattlePointScore = 3;
        public float mpBattlePointScore = 5;
        public float mpRecoveryBattlePointScore = 2;
        public float mpLeechRateBattlePointScore = 3;
        public float staminaBattlePointScore = 2;
        public float staminaRecoveryBattlePointScore = 2;
        public float staminaLeechRateBattlePointScore = 3;
        public float foodBattlePointScore = 2;
        public float waterBattlePointScore = 2;
        public float accuracyBattlePointScore = 10;
        public float evasionBattlePointScore = 10;
        public float criRateBattlePointScore = 5;
        public float criDmgRateBattlePointScore = 5;
        public float blockRateBattlePointScore = 5;
        public float blockDmgRateBattlePointScore = 5;
        public float moveSpeedBattlePointScore = 10;
        public float atkSpeedBattlePointScore = 10;

        [Header("Player Death")]
        public int itemDecreaseOnDeadMin;
        public int itemDecreaseOnDeadMax;

        [Header("Monster Appearances")]
        public int monsterTitleColorChangeLevel = 5;
        public Color monsterHighLevelTitleColor = Color.red;
        public Color monsterLowLevelTitleColor = Color.cyan;

        [Header("Dealing")]
        [Min(0.01f)]
        public float taxByItemPriceRate = 0.05f;
        public bool calculateTaxByStackedItems = false;

        [Header("PK")]
        public int minLevelToTurnPkOn = 10;
        public int pkPointEachKills = 10;
        public int hoursBeforeTurnPkOff = 48;
        public PkData[] pkDatas = new PkData[0];

        [System.Serializable]
        public struct PkData
        {
            public int minPkPoint;
            public Color nameColor;
            [Header("Dead Punishment")]
            public float expDecreasePercentMin;
            public float expDecreasePercentMax;
            public int goldDecreaseMin;
            public int goldDecreaseMax;
            public int itemDecreaseMin;
            public int itemDecreaseMax;
            [Range(0f, 1f)]
            public float goldReductionRate;
            [Range(0f, 1f)]
            public float expReductionRate;
        }

        [System.NonSerialized]
        private List<PkData> _sortedPkDatas;
        public List<PkData> SortedPkDatas
        {
            get
            {
                if (_sortedPkDatas == null)
                    _sortedPkDatas = pkDatas.OrderBy(o => o.minPkPoint).ToList();
                return _sortedPkDatas;
            }
        }

        public override bool RandomAttackHitOccurs(Vector3 fromPosition, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed, out bool isCritical, out bool isBlocked)
        {
            isCritical = false;
            isBlocked = false;
            if (attacker == null)
                return true;
            isCritical = Random.value <= GetCriticalChance(attacker, damageReceiver);
            bool isHit = Random.value <= GetHitChance(attacker, damageReceiver);
            if (!isHit && isCritical && alwaysHitWhenCriticalOccurs)
                isHit = true;
            isBlocked = Random.value <= GetBlockChance(attacker, damageReceiver);
            return isHit;
        }

        public override float RandomAttackDamage(Vector3 fromPosition, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, DamageElement damageElement, MinMaxFloat damageAmount, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed)
        {
            return damageAmount.Random(randomSeed);
        }

        public virtual float GetHitChance(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            // Attacker stats
            CharacterStats attackerStats = attacker.GetCaches().Stats;
            // Damage receiver stats
            CharacterStats dmgReceiverStats = damageReceiver.GetCaches().Stats;
            // Calculate chance to hit
            float attackerAcc = attackerStats.accuracy;
            float dmgReceiverEva = dmgReceiverStats.evasion;
            int attackerLvl = attacker.Level;
            int dmgReceiverLvl = damageReceiver.Level;
            float hitChance = 2f;

            if (attackerAcc != 0 && dmgReceiverEva != 0)
                hitChance *= (attackerAcc / (attackerAcc + dmgReceiverEva));

            if (attackerLvl != 0 && dmgReceiverLvl != 0)
                hitChance *= ((float)attackerLvl / (float)(attackerLvl + dmgReceiverLvl));

            // Minimum hit chance is 5%
            if (hitChance < 0.05f)
                hitChance = 0.05f;
            // Maximum hit chance is 95%
            if (hitChance > 0.95f)
                hitChance = 0.95f;
            return hitChance;
        }

        public override float GetDamageReducedByResistance(Dictionary<DamageElement, float> damageReceiverResistances, Dictionary<DamageElement, float> damageReceiverArmors, float damageAmount, DamageElement damageElement)
        {
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            // Reduce damage by resistance
            float resistanceAmount;
            if (damageReceiverResistances.TryGetValue(damageElement, out resistanceAmount))
            {
                if (resistanceAmount > damageElement.MaxResistanceAmount)
                    resistanceAmount = damageElement.MaxResistanceAmount;
                damageAmount -= damageAmount * resistanceAmount; // If resistance is minus damage will be increased
            }
            // Reduce damage by armor
            float armorAmount;
            if (damageReceiverArmors.TryGetValue(damageElement, out armorAmount))
            {
                // Formula: Attack * 100 / (100 + Defend)
                damageAmount *= 100f / (100f + Mathf.Max(0f, armorAmount));
            }
            return damageAmount;
        }

        public virtual float GetCriticalChance(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            float criRate = attacker.GetCaches().Stats.criRate;
            // Minimum critical chance is 5%
            if (criRate < 0.05f)
                criRate = 0.05f;
            // Maximum critical chance is 95%
            if (criRate > 0.95f)
                criRate = 0.95f;
            return criRate;
        }

        public override float GetCriticalDamage(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, float damage)
        {
            return damage * attacker.GetCaches().Stats.criDmgRate;
        }

        public virtual float GetBlockChance(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            float blockRate = damageReceiver.GetCaches().Stats.blockRate;
            // Minimum block chance is 5%
            if (blockRate < 0.05f)
                blockRate = 0.05f;
            // Maximum block chance is 95%
            if (blockRate > 0.95f)
                blockRate = 0.95f;
            return blockRate;
        }

        public override float GetBlockDamage(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, float damage)
        {
            float blockDmgRate = damageReceiver.GetCaches().Stats.blockDmgRate;
            // Minimum block damage is 5%
            if (blockDmgRate < 0.05f)
                blockDmgRate = 0.05f;
            // Maximum block damage is 95%
            if (blockDmgRate > 0.95f)
                blockDmgRate = 0.95f;
            return damage - (damage * blockDmgRate);
        }

        public override int GetTotalDamage(Vector3 fromPosition, EntityInfo instigator, DamageableEntity damageReceiver, float totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel)
        {
            return Mathf.FloorToInt(totalDamage);
        }

        public override float GetRecoveryHpPerSeconds(BaseCharacterEntity character)
        {
            if (IsHungry(character))
                return 0;
            return (character.GetCaches().MaxHp * hpRecoveryRatePerSeconds) + character.GetCaches().Stats.hpRecovery;
        }

        public override float GetRecoveryMpPerSeconds(BaseCharacterEntity character)
        {
            if (IsThirsty(character))
                return 0;
            return (character.GetCaches().MaxMp * mpRecoveryRatePerSeconds) + character.GetCaches().Stats.mpRecovery;
        }

        public override float GetRecoveryStaminaPerSeconds(BaseCharacterEntity character)
        {
            return staminaRecoveryPerSeconds + character.GetCaches().Stats.staminaRecovery;
        }

        public override float GetDecreasingHpPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            float result = 0f;
            if (IsHungry(character))
                result += character.GetCaches().MaxHp * hpDecreaseRatePerSecondsWhenHungry;
            if (IsThirsty(character))
                result += character.GetCaches().MaxHp * hpDecreaseRatePerSecondsWhenThirsty;
            return result;
        }

        public override float GetDecreasingMpPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            float result = 0f;
            if (IsHungry(character))
                result += character.GetCaches().MaxMp * mpDecreaseRatePerSecondsWhenHungry;
            if (IsThirsty(character))
                result += character.GetCaches().MaxMp * mpDecreaseRatePerSecondsWhenThirsty;
            return result;
        }

        public override float GetDecreasingStaminaPerSeconds(BaseCharacterEntity character)
        {
            if (character.ExtraMovementState != ExtraMovementState.IsSprinting ||
                !character.MovementState.Has(MovementState.IsGrounded) ||
                (!character.MovementState.Has(MovementState.Forward) &&
                !character.MovementState.Has(MovementState.Backward) &&
                !character.MovementState.Has(MovementState.Left) &&
                !character.MovementState.Has(MovementState.Right)))
                return 0f;
            float amount = staminaDecreasePerSeconds - character.CachedData.DecreaseStaminaDecreation;
            if (amount < 0)
                amount = 0;
            return amount;
        }

        public override float GetDecreasingFoodPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            float amount = foodDecreasePerSeconds - character.CachedData.DecreaseFoodDecreation;
            if (amount < 0)
                amount = 0;
            return amount;
        }

        public override float GetDecreasingWaterPerSeconds(BaseCharacterEntity character)
        {
            if (character is BaseMonsterCharacterEntity)
                return 0f;
            float amount = waterDecreasePerSeconds - character.CachedData.DecreaseWaterDecreation;
            if (amount < 0)
                amount = 0;
            return amount;
        }

        public override void GetPlayerDeadPunishment(DeadPunishmentType type, BasePlayerCharacterEntity player, BaseCharacterEntity attacker, out int decreaseExp, out int decreaseGold, out int decreaseItems, out int attackerPkPoint)
        {
            decreaseExp = 0;
            decreaseGold = 0;
            decreaseItems = 0;
            attackerPkPoint = 0;
            float decreaseExpPercent = 0f;
            bool attackerIsPlayer = attacker is BasePlayerCharacterEntity;
            switch (type)
            {
                case DeadPunishmentType.PK:
                    if (attackerIsPlayer && TryGetPkData(player, out PkData pkData))
                    {
                        // Decrease Exp
                        decreaseExpPercent += Random.Range(pkData.expDecreasePercentMin, pkData.expDecreasePercentMax);
                        // Decrease Gold
                        decreaseGold += Random.Range(pkData.goldDecreaseMin, pkData.goldDecreaseMax);
                        // Decrease Item
                        decreaseItems += Random.Range(pkData.itemDecreaseMin, pkData.itemDecreaseMax);
                    }
                    if (attackerIsPlayer)
                    {
                        // PK point
                        attackerPkPoint = pkPointEachKills;
                    }
                    break;
                default:
                    if (!attackerIsPlayer)
                    {
                        // Decrease Exp
                        decreaseExpPercent += expLostPercentageWhenDeath;
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(player.GuildId, out GuildData guildData))
                            decreaseExpPercent -= decreaseExpPercent * guildData.DecreaseExpLostPercentage;
                        // Decrease Item
                        decreaseItems += Random.Range(itemDecreaseOnDeadMin, itemDecreaseOnDeadMax);
                    }
                    break;
            }
            decreaseExp += Mathf.CeilToInt(player.GetNextLevelExp() * decreaseExpPercent * 0.01f);
        }

        public override float GetOverweightMoveSpeedRate(BaseGameEntity gameEntity)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return moveSpeedRateWhileOverweight;
        }

        public override float GetSprintMoveSpeed(BaseGameEntity gameEntity, CharacterStats stats)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return (stats.moveSpeed * moveSpeedRateWhileSprinting) + moveSpeedIncrementWhileSprinting + stats.sprintSpeed;
        }

        public override float GetWalkMoveSpeed(BaseGameEntity gameEntity, CharacterStats stats)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return (stats.moveSpeed * moveSpeedRateWhileWalking) + moveSpeedIncrementWhileWalking;
        }

        public override float GetCrouchMoveSpeed(BaseGameEntity gameEntity, CharacterStats stats)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return (stats.moveSpeed * moveSpeedRateWhileCrouching) + moveSpeedIncrementWhileCrouching;
        }

        public override float GetCrawlMoveSpeed(BaseGameEntity gameEntity, CharacterStats stats)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return (stats.moveSpeed * moveSpeedRateWhileCrawling) + moveSpeedIncrementWhileCrawling;
        }

        public override float GetSwimMoveSpeed(BaseGameEntity gameEntity, CharacterStats stats)
        {
            // For some gameplay rule, move speed rate may difference for specific entiy type.
            return (stats.moveSpeed * moveSpeedRateWhileSwimming) + moveSpeedIncrementWhileSwimming;
        }

        public override float GetLimitWeight(ICharacterData character, CharacterStats stats)
        {
            return stats.weightLimit;
        }

        public override float GetTotalWeight(ICharacterData character, CharacterStats stats)
        {
            float result = character.EquipItems.GetTotalItemWeight() + character.NonEquipItems.GetTotalItemWeight();
            // Weight from right hand equipment
            if (!character.EquipWeapons.rightHand.IsEmptySlot())
                result += character.EquipWeapons.rightHand.GetItem().Weight;
            // Weight from left hand equipment
            if (!character.EquipWeapons.leftHand.IsEmptySlot())
                result += character.EquipWeapons.leftHand.GetItem().Weight;
            return result;
        }

        public override int GetLimitSlot(ICharacterData character, CharacterStats stats)
        {
            return (int)(stats.slotLimit + GameInstance.Singleton.baseSlotLimit);
        }

        public override int GetTotalSlot(ICharacterData character, CharacterStats stats)
        {
            return character.NonEquipItems.GetTotalItemSlot();
        }

        public override bool IsHungry(BaseCharacterEntity character)
        {
            return foodDecreasePerSeconds > 0 && character.CurrentFood < hungryWhenFoodLowerThan;
        }

        public override bool IsThirsty(BaseCharacterEntity character)
        {
            return waterDecreasePerSeconds > 0 && character.CurrentWater < thirstyWhenWaterLowerThan;
        }

        public override bool RewardExp(BaseCharacterEntity character, int exp, float multiplier, RewardGivenType rewardGivenType, int giverLevel, int sourceLevel, out int rewardedExp)
        {
            rewardedExp = 0;
            if (character is BaseMonsterCharacterEntity monsterCharacterEntity && monsterCharacterEntity.SummonType != SummonType.PetItem)
            {
                // If it's monster and not pet, do not increase exp
                return false;
            }

            bool isLevelUp = false;
            BasePlayerCharacterEntity playerCharacter = character as BasePlayerCharacterEntity;
            if (playerCharacter != null)
            {
                GuildData guildData;
                PkData pkData;
                switch (rewardGivenType)
                {
                    case RewardGivenType.KillMonster:
                        exp = Mathf.CeilToInt(exp * multiplier * (ExpRate - (TryGetPkData(playerCharacter, out pkData) ? pkData.expReductionRate : 0f) + playerCharacter.GetCaches().ExpRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            exp += Mathf.CeilToInt(exp * guildData.IncreaseExpGainPercentage * 0.01f);
                        break;
                    case RewardGivenType.PartyShare:
                        exp = Mathf.CeilToInt(exp * multiplier * (ExpRate - (TryGetPkData(playerCharacter, out pkData) ? pkData.expReductionRate : 0f) + playerCharacter.GetCaches().ExpRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            exp += Mathf.CeilToInt(exp * guildData.IncreaseShareExpGainPercentage * 0.01f);
                        break;
                }
            }

            int nextLevelExp = character.GetNextLevelExp();
            if (nextLevelExp > 0)
            {
                // Increasing level if character not reached max level yet
                character.Exp = character.Exp.Increase(exp);
                while (nextLevelExp > 0 && character.Exp >= nextLevelExp)
                {
                    character.Exp = character.Exp - nextLevelExp;
                    ++character.Level;
                    nextLevelExp = character.GetNextLevelExp();
                    if (playerCharacter != null)
                    {
                        try
                        {
                            if (increaseStatPointsUntilReachedLevel == 0 ||
                                character.Level + 1 < increaseStatPointsUntilReachedLevel)
                            {
                                checked
                                {
                                    playerCharacter.StatPoint += increaseStatPointEachLevel;
                                }
                            }
                        }
                        catch (System.OverflowException)
                        {
                            playerCharacter.StatPoint = float.MaxValue;
                        }

                        try
                        {
                            if (increaseSkillPointsUntilReachedLevel == 0 ||
                                character.Level + 1 < increaseSkillPointsUntilReachedLevel)
                            {
                                checked
                                {
                                    playerCharacter.SkillPoint += increaseSkillPointEachLevel;
                                }
                            }
                        }
                        catch (System.OverflowException)
                        {
                            playerCharacter.SkillPoint = float.MaxValue;
                        }
                    }
                    isLevelUp = true;
                }

            }

            if (nextLevelExp <= 0)
            {
                // Don't collect exp if character reached max level
                character.Exp = 0;
            }

            if (isLevelUp && !character.IsDead())
            {
                if (recoverHpWhenLevelUp)
                    character.CurrentHp = character.MaxHp;
                if (recoverMpWhenLevelUp)
                    character.CurrentMp = character.MaxMp;
                if (recoverFoodWhenLevelUp)
                    character.CurrentFood = character.MaxFood;
                if (recoverWaterWhenLevelUp)
                    character.CurrentWater = character.MaxWater;
                if (recoverStaminaWhenLevelUp)
                    character.CurrentStamina = character.MaxStamina;
            }
            rewardedExp = exp;
            return isLevelUp;
        }

        public override void RewardGold(BaseCharacterEntity character, int gold, float multiplier, RewardGivenType rewardGivenType, int giverLevel, int sourceLevel, out int rewardedGold)
        {
            rewardedGold = 0;
            if (character is BaseMonsterCharacterEntity)
            {
                // Don't give reward currencies to monsters
                return;
            }

            BasePlayerCharacterEntity playerCharacter = character as BasePlayerCharacterEntity;
            if (playerCharacter != null)
            {
                GuildData guildData;
                PkData pkData;
                switch (rewardGivenType)
                {
                    case RewardGivenType.KillMonster:
                        gold = Mathf.CeilToInt(gold * multiplier * (GoldRate - (TryGetPkData(playerCharacter, out pkData) ? pkData.goldReductionRate : 0f) + playerCharacter.GetCaches().GoldRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            gold += Mathf.CeilToInt(gold * guildData.IncreaseGoldGainPercentage * 0.01f);
                        break;
                    case RewardGivenType.PartyShare:
                        gold = Mathf.CeilToInt(gold * multiplier * (GoldRate - (TryGetPkData(playerCharacter, out pkData) ? pkData.goldReductionRate : 0f) + playerCharacter.GetCaches().GoldRate));
                        if (GameInstance.ServerGuildHandlers.TryGetGuild(playerCharacter.GuildId, out guildData))
                            gold += Mathf.CeilToInt(gold * guildData.IncreaseShareGoldGainPercentage * 0.01f);
                        break;
                }

                playerCharacter.Gold = playerCharacter.Gold.Increase(gold);
                rewardedGold = gold;
            }
        }

        public override void RewardCurrencies(BaseCharacterEntity character, IEnumerable<CurrencyAmount> currencies, float multiplier, RewardGivenType rewardGivenType, int giverLevel, int sourceLevel)
        {
            if (character is BaseMonsterCharacterEntity)
            {
                // Don't give reward currencies to monsters
                return;
            }

            BasePlayerCharacterEntity playerCharacter = character as BasePlayerCharacterEntity;
            if (playerCharacter != null)
                playerCharacter.IncreaseCurrencies(currencies, multiplier);
        }

        public override float GetEquipmentStatsRate(CharacterItem characterItem)
        {
            if (characterItem.GetMaxDurability() <= 0)
                return 1;
            float durabilityRate = characterItem.durability / characterItem.GetMaxDurability();
            if (durabilityRate > 0.5f)
                return 1f;
            else if (durabilityRate > 0.3f)
                return 0.75f;
            else if (durabilityRate > 0.15f)
                return 0.5f;
            else if (durabilityRate > 0.05f)
                return 0.25f;
            else
                return 0f;
        }

        public override void OnCharacterRespawn(ICharacterData character)
        {
            character.CurrentHp = character.GetCaches().MaxHp;
            character.CurrentMp = character.GetCaches().MaxMp;
            character.CurrentStamina = character.GetCaches().MaxStamina;
            character.CurrentFood = character.GetCaches().MaxFood;
            character.CurrentWater = character.GetCaches().MaxWater;
        }

        public override void OnCharacterReceivedDamage(BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver, CombatAmountType combatAmountType, int damage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime)
        {
            if (isDamageOverTime)
                return;
            GetDecreaseDurabilityAmount(combatAmountType, out float decreaseWeaponDurability, out float decreaseShieldDurability, out float decreaseArmorDurability);
            if (attacker != null)
            {
                // Decrease Weapon Durability
                DecreaseEquipWeaponsDurability(attacker, decreaseWeaponDurability);
                CharacterStats stats = attacker.GetCaches().Stats;
                // Hp Leeching, don't decrease damage receiver's Hp again
                if (!attacker.IsDead())
                {
                    int leechAmount = Mathf.CeilToInt(damage * stats.hpLeechRate);
                    if (leechAmount != 0)
                    {
                        attacker.CurrentHp += leechAmount;
                        attacker.CurrentHp = Mathf.Clamp(attacker.CurrentHp, 0, attacker.MaxHp);
                        attacker.CallRpcAppendCombatText(CombatAmountType.HpLeech, HitEffectsSourceType.None, 0, leechAmount);
                    }
                    // Mp Leeching
                    leechAmount = Mathf.CeilToInt(damage * stats.mpLeechRate);
                    if (leechAmount != 0)
                    {
                        attacker.CurrentMp += leechAmount;
                        attacker.CurrentMp = Mathf.Clamp(attacker.CurrentMp, 0, attacker.MaxMp);
                        attacker.CallRpcAppendCombatText(CombatAmountType.MpLeech, HitEffectsSourceType.None, 0, leechAmount);
                        damageReceiver.CurrentMp -= leechAmount;
                        damageReceiver.CurrentMp = Mathf.Clamp(damageReceiver.CurrentMp, 0, damageReceiver.MaxMp);
                    }
                    // Stamina Leeching
                    leechAmount = Mathf.CeilToInt(damage * stats.staminaLeechRate);
                    if (leechAmount != 0)
                    {
                        attacker.CurrentStamina += leechAmount;
                        attacker.CurrentStamina = Mathf.Clamp(attacker.CurrentStamina, 0, attacker.MaxStamina);
                        attacker.CallRpcAppendCombatText(CombatAmountType.StaminaLeech, HitEffectsSourceType.None, 0, leechAmount);
                        damageReceiver.CurrentStamina -= leechAmount;
                        damageReceiver.CurrentStamina = Mathf.Clamp(damageReceiver.CurrentStamina, 0, damageReceiver.MaxStamina);
                    }
                }
                // Applies status effects
                int i;
                IEquipmentItem tempEquipmentItem;
                CharacterItem tempCharacterItem;
                CharacterBuff tempCharacterBuff;
                Buff tempBuff;
                BaseSkill tempSkill;
                EntityInfo attackerInfo = attacker.GetInfo();
                EntityInfo damageReceiverInfo = damageReceiver.GetInfo();
                // Attacker
                if (!attacker.IsDead())
                {
                    // Armors
                    for (i = 0; i < attacker.EquipItems.Count; ++i)
                    {
                        tempCharacterItem = attacker.EquipItems[i];
                        tempEquipmentItem = tempCharacterItem.GetEquipmentItem();
                        ApplyStatusEffectsWhenAttacking(tempCharacterItem, tempEquipmentItem, attackerInfo, attacker, damageReceiver);
                        if (attacker.IsDead())
                            break;
                    }
                }
                if (!attacker.IsDead())
                {
                    // Right-hand weapon
                    tempEquipmentItem = attacker.EquipWeapons.GetRightHandEquipmentItem();
                    if (tempEquipmentItem != null)
                    {
                        ApplyStatusEffectsWhenAttacking(attacker.EquipWeapons.rightHand, tempEquipmentItem, attackerInfo, attacker, damageReceiver);
                    }
                }
                if (!attacker.IsDead())
                {
                    // Left-hand weapon / shield
                    tempEquipmentItem = attacker.EquipWeapons.GetLeftHandEquipmentItem();
                    if (tempEquipmentItem != null)
                    {
                        ApplyStatusEffectsWhenAttacking(attacker.EquipWeapons.leftHand, tempEquipmentItem, attackerInfo, attacker, damageReceiver);
                    }
                }
                if (!attacker.IsDead())
                {
                    // Buffs/Debuffs
                    for (i = 0; i < attacker.Buffs.Count; ++i)
                    {
                        tempCharacterBuff = attacker.Buffs[i];
                        tempBuff = tempCharacterBuff.GetBuff().GetBuff();
                        tempBuff.ApplySelfStatusEffectsWhenAttacking(tempCharacterBuff.level, attackerInfo, attacker);
                        tempBuff.ApplyEnemyStatusEffectsWhenAttacking(tempCharacterBuff.level, attackerInfo, damageReceiver);
                        if (attacker.IsDead())
                            break;
                    }
                }
                if (!attacker.IsDead())
                {
                    // Passive skills
                    foreach (KeyValuePair<BaseSkill, int> characterSkill in attacker.GetCaches().Skills)
                    {
                        tempSkill = characterSkill.Key;
                        if (!tempSkill.IsPassive)
                            continue;
                        if (tempSkill.TryGetBuff(out tempBuff))
                        {
                            tempBuff.ApplySelfStatusEffectsWhenAttacking(characterSkill.Value, attackerInfo, attacker);
                            tempBuff.ApplyEnemyStatusEffectsWhenAttacking(characterSkill.Value, attackerInfo, damageReceiver);
                        }
                        if (attacker.IsDead())
                            break;
                    }
                }
                // Damage Receiver
                if (!damageReceiver.IsDead())
                {
                    // Armors
                    for (i = 0; i < damageReceiver.EquipItems.Count; ++i)
                    {
                        tempCharacterItem = damageReceiver.EquipItems[i];
                        tempEquipmentItem = tempCharacterItem.GetEquipmentItem();
                        ApplyStatusEffectsWhenAttacked(tempCharacterItem, tempEquipmentItem, damageReceiverInfo, attacker, damageReceiver);
                        if (damageReceiver.IsDead())
                            break;
                    }
                }
                if (!damageReceiver.IsDead())
                {
                    // Right-hand weapon
                    tempEquipmentItem = damageReceiver.EquipWeapons.GetRightHandEquipmentItem();
                    ApplyStatusEffectsWhenAttacked(damageReceiver.EquipWeapons.rightHand, tempEquipmentItem, damageReceiverInfo, attacker, damageReceiver);
                }
                if (!damageReceiver.IsDead())
                {
                    // Left-hand weapon / shield
                    tempEquipmentItem = damageReceiver.EquipWeapons.GetLeftHandEquipmentItem();
                    ApplyStatusEffectsWhenAttacked(damageReceiver.EquipWeapons.leftHand, tempEquipmentItem, damageReceiverInfo, attacker, damageReceiver);
                }
                if (!damageReceiver.IsDead())
                {
                    // Buffs/Debuffs
                    for (i = 0; i < damageReceiver.Buffs.Count; ++i)
                    {
                        tempCharacterBuff = damageReceiver.Buffs[i];
                        tempBuff = tempCharacterBuff.GetBuff().GetBuff();
                        tempBuff.ApplySelfStatusEffectsWhenAttacked(tempCharacterBuff.level, damageReceiverInfo, damageReceiver);
                        tempBuff.ApplyEnemyStatusEffectsWhenAttacked(tempCharacterBuff.level, damageReceiverInfo, attacker);
                        if (damageReceiver.IsDead())
                            break;
                    }
                }
                if (!damageReceiver.IsDead())
                {
                    // Passive skills
                    foreach (KeyValuePair<BaseSkill, int> characterSkill in damageReceiver.GetCaches().Skills)
                    {
                        tempSkill = characterSkill.Key;
                        if (!tempSkill.IsPassive)
                            continue;
                        if (tempSkill.TryGetBuff(out tempBuff))
                        {
                            tempBuff.ApplySelfStatusEffectsWhenAttacked(characterSkill.Value, damageReceiverInfo, damageReceiver);
                            tempBuff.ApplyEnemyStatusEffectsWhenAttacked(characterSkill.Value, damageReceiverInfo, attacker);
                        }
                        if (damageReceiver.IsDead())
                            break;
                    }
                }
            }
            // Decrease Shield Durability
            DecreaseEquipShieldsDurability(damageReceiver, decreaseShieldDurability);
            // Decrease Armor Durability
            DecreaseEquipItemsDurability(damageReceiver, decreaseArmorDurability);
        }

        private void ApplyStatusEffectsWhenAttacking(CharacterItem characterItem, IEquipmentItem equipmentItem, EntityInfo attackerInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            if (equipmentItem == null)
                return;
            bool isWeapon = equipmentItem is IWeaponItem;
            equipmentItem.ApplySelfStatusEffectsWhenAttacking(characterItem.level, attackerInfo, isWeapon ? characterItem : CharacterItem.Empty, attacker);
            equipmentItem.ApplyEnemyStatusEffectsWhenAttacking(characterItem.level, attackerInfo, isWeapon ? characterItem : CharacterItem.Empty, damageReceiver);
            if (characterItem.sockets == null || characterItem.sockets.Count == 0)
                return;
            foreach (int socketItemDataId in characterItem.sockets)
            {
                ApplyStatusEffectsWhenAttacking(isWeapon ? characterItem : CharacterItem.Empty, socketItemDataId, attackerInfo, attacker, damageReceiver);
            }
        }

        private void ApplyStatusEffectsWhenAttacking(CharacterItem weapon, int socketItemDataId, EntityInfo attackerInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            if (!GameInstance.Items.TryGetValue(socketItemDataId, out BaseItem item))
                return;
            ISocketEnhancerItem tempSocketEnhancerItem = item as ISocketEnhancerItem;
            tempSocketEnhancerItem.ApplySelfStatusEffectsWhenAttacking(attackerInfo, weapon, attacker);
            tempSocketEnhancerItem.ApplyEnemyStatusEffectsWhenAttacking(attackerInfo, weapon, damageReceiver);
        }

        private void ApplyStatusEffectsWhenAttacked(CharacterItem characterItem, IEquipmentItem equipmentItem, EntityInfo damageReceiverInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            if (equipmentItem == null)
                return;
            equipmentItem.ApplySelfStatusEffectsWhenAttacked(characterItem.level, damageReceiverInfo, damageReceiver);
            equipmentItem.ApplyEnemyStatusEffectsWhenAttacked(characterItem.level, damageReceiverInfo, attacker);
            if (characterItem.sockets == null || characterItem.sockets.Count == 0)
                return;
            foreach (int socketItemDataId in characterItem.sockets)
            {
                ApplyStatusEffectsWhenAttacked(socketItemDataId, damageReceiverInfo, attacker, damageReceiver);
            }
        }

        private void ApplyStatusEffectsWhenAttacked(int socketItemDataId, EntityInfo damageReceiverInfo, BaseCharacterEntity attacker, BaseCharacterEntity damageReceiver)
        {
            if (!GameInstance.Items.TryGetValue(socketItemDataId, out BaseItem item))
                return;
            ISocketEnhancerItem tempSocketEnhancerItem = item as ISocketEnhancerItem;
            tempSocketEnhancerItem.ApplySelfStatusEffectsWhenAttacked(damageReceiverInfo, damageReceiver);
            tempSocketEnhancerItem.ApplyEnemyStatusEffectsWhenAttacked(damageReceiverInfo, attacker);
        }

        public override void OnHarvestableReceivedDamage(BaseCharacterEntity attacker, HarvestableEntity damageReceiver, CombatAmountType combatAmountType, int damage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime)
        {
            if (isDamageOverTime)
                return;
            GetDecreaseDurabilityAmount(combatAmountType, out float decreaseWeaponDurability, out float decreaseShieldDurability, out float decreaseArmorDurability);
            if (attacker != null)
            {
                // Decrease Weapon Durability
                DecreaseEquipWeaponsDurability(attacker, decreaseWeaponDurability);
            }
        }

        private void GetDecreaseDurabilityAmount(CombatAmountType combatAmountType, out float decreaseWeaponDurability, out float decreaseShieldDurability, out float decreaseArmorDurability)
        {
            decreaseWeaponDurability = normalDecreaseWeaponDurability;
            decreaseShieldDurability = normalDecreaseShieldDurability;
            decreaseArmorDurability = normalDecreaseArmorDurability;
            switch (combatAmountType)
            {
                case CombatAmountType.BlockedDamage:
                    decreaseWeaponDurability = blockedDecreaseWeaponDurability;
                    decreaseShieldDurability = blockedDecreaseShieldDurability;
                    decreaseArmorDurability = blockedDecreaseArmorDurability;
                    break;
                case CombatAmountType.CriticalDamage:
                    decreaseWeaponDurability = criticalDecreaseWeaponDurability;
                    decreaseShieldDurability = criticalDecreaseShieldDurability;
                    decreaseArmorDurability = criticalDecreaseArmorDurability;
                    break;
                case CombatAmountType.Miss:
                    decreaseWeaponDurability = missDecreaseWeaponDurability;
                    decreaseShieldDurability = missDecreaseShieldDurability;
                    decreaseArmorDurability = missDecreaseArmorDurability;
                    break;
            }
        }

        private void DecreaseEquipWeaponsDurability(BaseCharacterEntity entity, float decreaseDurability)
        {
            bool tempDestroy;
            EquipWeapons equipWeapons = entity.EquipWeapons;
            CharacterItem rightHand = equipWeapons.rightHand;
            CharacterItem leftHand = equipWeapons.leftHand;
            if (rightHand.GetWeaponItem() != null && rightHand.GetMaxDurability() > 0)
            {
                rightHand = DecreaseDurability(entity, rightHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.rightHand = CharacterItem.Empty;
                else
                    equipWeapons.rightHand = rightHand;
            }
            if (leftHand.GetWeaponItem() != null && leftHand.GetMaxDurability() > 0)
            {
                leftHand = DecreaseDurability(entity, leftHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.leftHand = CharacterItem.Empty;
                else
                    equipWeapons.leftHand = leftHand;
            }
            entity.EquipWeapons = equipWeapons;
        }

        private void DecreaseEquipShieldsDurability(BaseCharacterEntity entity, float decreaseDurability)
        {
            bool tempDestroy;
            EquipWeapons equipWeapons = entity.EquipWeapons;
            CharacterItem rightHand = equipWeapons.rightHand;
            CharacterItem leftHand = equipWeapons.leftHand;
            if (rightHand.GetShieldItem() != null && rightHand.GetMaxDurability() > 0)
            {
                rightHand = DecreaseDurability(entity, rightHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.rightHand = CharacterItem.Empty;
                else
                    equipWeapons.rightHand = rightHand;
            }
            if (leftHand.GetShieldItem() != null && leftHand.GetMaxDurability() > 0)
            {
                leftHand = DecreaseDurability(entity, leftHand, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    equipWeapons.leftHand = CharacterItem.Empty;
                else
                    equipWeapons.leftHand = leftHand;
            }
            entity.EquipWeapons = equipWeapons;
        }

        private void DecreaseEquipItemsDurability(BaseCharacterEntity entity, float decreaseDurability)
        {
            bool tempDestroy;
            int count = entity.EquipItems.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                CharacterItem equipItem = entity.EquipItems[i];
                if (equipItem.GetMaxDurability() <= 0)
                    continue;
                equipItem = DecreaseDurability(entity, equipItem, decreaseDurability, out tempDestroy);
                if (tempDestroy)
                    entity.EquipItems.RemoveAt(i);
                else
                    entity.EquipItems[i] = equipItem;
            }
        }

        private CharacterItem DecreaseDurability(BaseCharacterEntity entity, CharacterItem characterItem, float decreaseDurability, out bool destroy)
        {
            destroy = false;
            IEquipmentItem item = characterItem.GetEquipmentItem();
            if (item != null)
            {
                if (characterItem.durability - decreaseDurability <= 0 && item.DestroyIfBroken)
                    destroy = true;
                characterItem.UpdateDurability(entity, -decreaseDurability);
            }
            return characterItem;
        }

        public override float GetRecoveryUpdateDuration()
        {
            return 1f;
        }

        public override void ApplyFallDamage(BaseCharacterEntity character, Vector3 lastGroundedPosition)
        {
            if (character.EntityTransform.position.y >= lastGroundedPosition.y)
                return;
            float dist = lastGroundedPosition.y - character.EntityTransform.position.y;
            if (dist < fallDamageMinDistance)
                return;
            float damage = character.MaxHp * (float)(dist - fallDamageMinDistance) / (float)(fallDamageMaxDistance - fallDamageMinDistance);
            damage = damage - (damage * character.CachedData.FallDamageAbsorbs);
            int intDamage = Mathf.CeilToInt(damage);
            if (intDamage < 0)
                intDamage = 0;
            character.CurrentHp -= intDamage;
            character.ReceivedDamage(HitBoxPosition.None, character.EntityTransform.position, EntityInfo.Empty, null, CombatAmountType.FallDamage, intDamage, CharacterItem.Empty, null, 0, CharacterBuff.Empty);
        }

        public override bool CanInteractEntity(BaseCharacterEntity character, uint objectId)
        {
            if (!character.CanDoActions())
                return false;
            BaseGameEntity interactingEntity;
            if (!character.Manager.Assets.TryGetSpawnedObject(objectId, out interactingEntity))
                return false;
            // This function will sort: near to far, so loop from 0
            float dist = Vector3.Distance(character.EntityTransform.position, interactingEntity.EntityTransform.position);
            Vector3 dir = (interactingEntity.EntityTransform.position - character.EntityTransform.position).normalized;
            // Find that the entity is behind the wall or not
            int count = character.FindPhysicFunctions.Raycast(character.MeleeDamageTransform.position, dir, dist, GameInstance.Singleton.buildingLayer.Mask, QueryTriggerInteraction.Ignore);
            IGameEntity gameEntity;
            for (int i = 0; i < count; ++i)
            {
                gameEntity = character.FindPhysicFunctions.GetRaycastObject(i).GetComponent<IGameEntity>();
                if (gameEntity.IsNull()) continue;
                if (gameEntity.GetObjectId() == objectId)
                {
                    // It's target entity, so interact it
                    return true;
                }
                if (gameEntity.Entity is BuildingEntity)
                {
                    // Cannot interact object behind the wall
                    return false;
                }
            }
            // Not hit anything, assume that it can interact
            return true;
        }

        public override Vector3 GetSummonPosition(BaseCharacterEntity character)
        {
            float halfDist = (GameInstance.Singleton.minSummonDistance + GameInstance.Singleton.maxSummonDistance) * 0.5f;
            Vector2 randomCircle = Random.insideUnitCircle * halfDist;
            // X
            if (randomCircle.x < 0 && Mathf.Abs(randomCircle.x) < halfDist)
                randomCircle.x = -halfDist;
            else if (randomCircle.x > 0 && randomCircle.x < halfDist)
                randomCircle.x = halfDist;
            // Y
            if (randomCircle.y < 0 && Mathf.Abs(randomCircle.y) < halfDist)
                randomCircle.y = -halfDist;
            else if (randomCircle.y > 0 && randomCircle.y < halfDist)
                randomCircle.y = halfDist;
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension2D)
                return character.EntityTransform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);
            return character.EntityTransform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }

        public override Quaternion GetSummonRotation(BaseCharacterEntity character)
        {
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension2D)
                return Quaternion.identity;
            return character.EntityTransform.rotation;
        }

        public override float GetBattlePointFromCharacterStats(CharacterStats stats)
        {
            float battlePoint = 0f;
            battlePoint += stats.hp * hpBattlePointScore;
            battlePoint += stats.hpRecovery * hpRecoveryBattlePointScore;
            battlePoint += stats.hpLeechRate * hpLeechRateBattlePointScore;
            battlePoint += stats.mp * mpBattlePointScore;
            battlePoint += stats.mpRecovery * mpRecoveryBattlePointScore;
            battlePoint += stats.mpLeechRate * mpLeechRateBattlePointScore;
            battlePoint += stats.stamina * staminaBattlePointScore;
            battlePoint += stats.staminaRecovery * staminaRecoveryBattlePointScore;
            battlePoint += stats.staminaLeechRate * staminaLeechRateBattlePointScore;
            battlePoint += stats.food * foodBattlePointScore;
            battlePoint += stats.water * waterBattlePointScore;
            battlePoint += stats.accuracy * accuracyBattlePointScore;
            battlePoint += stats.evasion * evasionBattlePointScore;
            battlePoint += stats.criRate * criRateBattlePointScore;
            battlePoint += stats.criDmgRate * criDmgRateBattlePointScore;
            battlePoint += stats.blockRate * blockRateBattlePointScore;
            battlePoint += stats.blockDmgRate * blockDmgRateBattlePointScore;
            battlePoint += stats.moveSpeed * moveSpeedBattlePointScore;
            battlePoint += stats.atkSpeed * atkSpeedBattlePointScore;
            return battlePoint;
        }

        public override string GetTurnPkOnWarningMessage()
        {
            return ZString.Format(LanguageManager.GetText(UIFormatKeys.UI_FORMAT_PK_TURN_ON_WARNING.ToString()), hoursBeforeTurnPkOff);
        }

        public override bool CanTurnPkOn(BasePlayerCharacterEntity player)
        {
            if (player.Level < minLevelToTurnPkOn)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(player.ConnectionId, UITextKeys.UI_ERROR_NOT_ENOUGH_LEVEL);
                return false;
            }
            return true;
        }

        public override bool CanTurnPkOff(BasePlayerCharacterEntity player)
        {
#if !DISABLE_CLASSIC_PK
            long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (player.LastPkOnTime <= 0 || currentTime - player.LastPkOnTime > (60 * 60 * hoursBeforeTurnPkOff))
                return true;
            long diff = currentTime - player.LastPkOnTime;
            double remainsHours = hoursBeforeTurnPkOff - ((double)diff / 60 / 60);
            GameInstance.ServerGameMessageHandlers.SendFormattedGameMessage(player.ConnectionId, UIFormatKeys.UI_FORMAT_PK_CAN_TURN_PK_AFTER_HOURS, remainsHours >= 1 ? remainsHours.ToString("N0") : remainsHours.ToString("N2"));
#endif
            return false;
        }

        public override bool GetEntityNameColor(BaseGameEntity entity, out Color color)
        {
            color = Color.white;
            if (GameInstance.PlayingCharacterEntity == null)
                return false;
            if (entity is BasePlayerCharacterEntity player)
            {
#if !DISABLE_CLASSIC_PK
                if (player.IsPkOn && TryGetPkData(player, out PkData pkData))
                {
                    color = pkData.nameColor;
                    return true;
                }
#endif
            }
            if (entity is BaseCharacterEntity character && !GameInstance.PlayingCharacterEntity.IsAlly(character.GetInfo()))
            {
                if (character.Level - GameInstance.PlayingCharacter.Level > monsterTitleColorChangeLevel)
                {
                    color = monsterHighLevelTitleColor;
                    return true;
                }
                if (GameInstance.PlayingCharacter.Level - character.Level > monsterTitleColorChangeLevel)
                {
                    color = monsterLowLevelTitleColor;
                    return true;
                }
            }
            return false;
        }

        public override int GetDealingFee(List<CharacterItem> dealingItems, int gold)
        {
            float totalFee = 0f;
            foreach (CharacterItem item in dealingItems)
            {
                BaseItem itemData = item.GetItem();
                float rate = (calculateTaxByStackedItems ? 1f : ((float)item.amount / (float)itemData.MaxStack)) * taxByItemPriceRate;
                totalFee += (float)itemData.SellPrice * rate;
            }
            return Mathf.CeilToInt(totalFee);
        }

        public override int GetPlayerBankDepositFee(int gold)
        {
            return 0;
        }

        public override int GetPlayerBankWithdrawFee(int gold)
        {
            return 0;
        }

        public override int GetGuildBankDepositFee(int gold)
        {
            return 0;
        }

        public override int GetGuildBankWithdrawFee(int gold)
        {
            return 0;
        }

        private bool TryGetPkData(BasePlayerCharacterEntity player, out PkData data)
        {
            data = default;
#if !DISABLE_CLASSIC_PK
            for (int i = SortedPkDatas.Count - 1; i >= 0; --i)
            {
                if (player.PkPoint >= SortedPkDatas[i].minPkPoint)
                {
                    data = SortedPkDatas[i];
                    return true;
                }
            }
#endif
            return false;
        }
    }
}
