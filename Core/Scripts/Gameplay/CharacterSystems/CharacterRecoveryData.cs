using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterRecoveryData
    {
        public BaseCharacterEntity CharacterEntity { get; private set; }
        public float RecoveryingHp { get; set; } = 0f;
        public float RecoveryingMp { get; set; } = 0f;
        public float RecoveryingStamina { get; set; } = 0f;
        public float RecoveryingFood { get; set; } = 0f;
        public float RecoveryingWater { get; set; } = 0f;
        public float DecreasingHp { get; set; } = 0f;
        public float DecreasingMp { get; set; } = 0f;
        public float DecreasingStamina { get; set; } = 0f;
        public float DecreasingFood { get; set; } = 0f;
        public float DecreasingWater { get; set; } = 0f;
        public float TotalDamageOverTime { get; set; } = 0f;
        public Dictionary<DamageElement, MinMaxFloat> DamageOverTimes { get; set; } = new Dictionary<DamageElement, MinMaxFloat>();

        private float _calculatingRecoveryingHp = 0f;
        private float _calculatingRecoveryingMp = 0f;
        private float _calculatingRecoveryingStamina = 0f;
        private float _calculatingRecoveryingFood = 0f;
        private float _calculatingRecoveryingWater = 0f;
        private float _calculatingDecreasingHp = 0f;
        private float _calculatingDecreasingMp = 0f;
        private float _calculatingDecreasingStamina = 0f;
        private float _calculatingDecreasingFood = 0f;
        private float _calculatingDecreasingWater = 0f;
        private float _calculatingTotalDamageOverTime = 0f;
        private CharacterBuff _buff;

        public CharacterRecoveryData(BaseCharacterEntity characterEntity)
        {
            CharacterEntity = characterEntity;
        }

        public void SetupByBuff(CharacterBuff buff, CalculatedBuff calculatedBuff)
        {
            _buff = buff;
            // Damage over time
            TotalDamageOverTime = 0f;
            DamageOverTimes = calculatedBuff.GetDamageOverTimes();
            foreach (KeyValuePair<DamageElement, MinMaxFloat> damageOverTime in DamageOverTimes)
            {
                TotalDamageOverTime += damageOverTime.Key.GetDamageReducedByResistance(CharacterEntity.CachedData.Resistances, CharacterEntity.CachedData.Armors, damageOverTime.Value.Random(Random.Range(0, 255)));
            }
            int tempAmount;
            // Hp recovery
            tempAmount = calculatedBuff.GetRecoveryHp();
            if (tempAmount > 0)
                RecoveryingHp += tempAmount;
            else if (tempAmount < 0)
                DecreasingHp += -tempAmount;
            // Mp recovery
            tempAmount = calculatedBuff.GetRecoveryMp();
            if (tempAmount > 0)
                RecoveryingMp += tempAmount;
            else if (tempAmount < 0)
                DecreasingMp += -tempAmount;
            // Stamina recovery
            tempAmount = calculatedBuff.GetRecoveryStamina();
            if (tempAmount > 0)
                RecoveryingStamina += tempAmount;
            else if (tempAmount < 0)
                DecreasingStamina += -tempAmount;
            // Food recovery
            tempAmount = calculatedBuff.GetRecoveryFood();
            if (tempAmount > 0)
                RecoveryingFood += tempAmount;
            else if (tempAmount < 0)
                DecreasingFood += -tempAmount;
            // Water recovery
            tempAmount = calculatedBuff.GetRecoveryWater();
            if (tempAmount > 0)
                RecoveryingWater += tempAmount;
            else if (tempAmount < 0)
                DecreasingWater += -tempAmount;
        }

        public void Clear()
        {
            _buff = CharacterBuff.Empty;
            RecoveryingHp = 0f;
            RecoveryingMp = 0f;
            RecoveryingStamina = 0f;
            RecoveryingFood = 0f;
            RecoveryingWater = 0f;
            DecreasingHp = 0f;
            DecreasingMp = 0f;
            DecreasingStamina = 0f;
            DecreasingFood = 0f;
            DecreasingWater = 0f;
            TotalDamageOverTime = 0f;
            DamageOverTimes.Clear();
            _calculatingRecoveryingHp = 0f;
            _calculatingRecoveryingMp = 0f;
            _calculatingRecoveryingStamina = 0f;
            _calculatingRecoveryingFood = 0f;
            _calculatingRecoveryingWater = 0f;
            _calculatingDecreasingHp = 0f;
            _calculatingDecreasingMp = 0f;
            _calculatingDecreasingStamina = 0f;
            _calculatingDecreasingFood = 0f;
            _calculatingDecreasingWater = 0f;
            _calculatingTotalDamageOverTime = 0f;
        }

        public void Apply(float rate)
        {
            EntityInfo instigator = !_buff.IsEmpty() ? _buff.BuffApplier : EntityInfo.Empty;

            int tempAmount;
            // Hp
            if (CharacterEntity.CurrentHp < CharacterEntity.MaxHp)
            {
                _calculatingRecoveryingHp += RecoveryingHp * rate;
                if (_calculatingRecoveryingHp >= 1)
                {
                    tempAmount = (int)_calculatingRecoveryingHp;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffHpRecovery(instigator, tempAmount);
                    _calculatingRecoveryingHp -= tempAmount;
                }
            }

            // Decrease Hp
            if (CharacterEntity.CurrentHp > 0)
            {
                _calculatingDecreasingHp += DecreasingHp * rate;
                if (_calculatingDecreasingHp >= 1)
                {
                    tempAmount = (int)_calculatingDecreasingHp;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffHpDecrease(instigator, tempAmount);
                    _calculatingDecreasingHp -= tempAmount;
                }
            }

            // Mp
            if (CharacterEntity.CurrentMp < CharacterEntity.MaxMp)
            {
                _calculatingRecoveryingMp += RecoveryingMp * rate;
                if (_calculatingRecoveryingMp >= 1)
                {
                    tempAmount = (int)_calculatingRecoveryingMp;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffMpRecovery(instigator, tempAmount);
                    _calculatingRecoveryingMp -= tempAmount;
                }
            }

            // Decrease Mp
            if (CharacterEntity.CurrentMp > 0)
            {
                _calculatingDecreasingMp += DecreasingMp * rate;
                if (_calculatingDecreasingMp >= 1)
                {
                    tempAmount = (int)_calculatingDecreasingMp;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffMpDecrease(instigator, tempAmount);
                    _calculatingDecreasingMp -= tempAmount;
                }
            }

            // Stamina
            if (CharacterEntity.CurrentStamina < CharacterEntity.MaxStamina)
            {
                _calculatingRecoveryingStamina += RecoveryingStamina * rate;
                if (_calculatingRecoveryingStamina >= 1)
                {
                    tempAmount = (int)_calculatingRecoveryingStamina;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffStaminaRecovery(instigator, tempAmount);
                    _calculatingRecoveryingStamina -= tempAmount;
                }
            }

            // Decrease Stamina
            if (CharacterEntity.CurrentStamina > 0)
            {
                _calculatingDecreasingStamina += DecreasingStamina * rate;
                if (_calculatingDecreasingStamina >= 1)
                {
                    tempAmount = (int)_calculatingDecreasingStamina;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffStaminaDecrease(instigator, tempAmount);
                    _calculatingDecreasingStamina -= tempAmount;
                }
            }

            // Food
            if (CharacterEntity.CurrentFood < CharacterEntity.MaxFood)
            {
                _calculatingRecoveryingFood += RecoveryingFood * rate;
                if (_calculatingRecoveryingFood >= 1)
                {
                    tempAmount = (int)_calculatingRecoveryingFood;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffFoodRecovery(instigator, tempAmount);
                    _calculatingRecoveryingFood -= tempAmount;
                }
            }

            // Decrease Food
            if (CharacterEntity.CurrentFood > 0)
            {
                _calculatingDecreasingFood += DecreasingFood * rate;
                if (_calculatingDecreasingFood >= 1)
                {
                    tempAmount = (int)_calculatingDecreasingFood;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffFoodDecrease(instigator, tempAmount);
                    _calculatingDecreasingFood -= tempAmount;
                }
            }

            // Water
            if (CharacterEntity.CurrentWater < CharacterEntity.MaxWater)
            {
                _calculatingRecoveryingWater += RecoveryingWater * rate;
                if (_calculatingRecoveryingWater >= 1)
                {
                    tempAmount = (int)_calculatingRecoveryingWater;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffWaterRecovery(instigator, tempAmount);
                    _calculatingRecoveryingWater -= tempAmount;
                }
            }

            // Decrease Water
            if (CharacterEntity.CurrentWater > 0)
            {
                _calculatingDecreasingWater += DecreasingWater * rate;
                if (_calculatingDecreasingWater >= 1)
                {
                    tempAmount = (int)_calculatingDecreasingWater;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.OnBuffWaterDecrease(instigator, tempAmount);
                    _calculatingDecreasingWater -= tempAmount;
                }
            }

            // Validate and do something if character dead
            CharacterEntity.ValidateRecovery(instigator);

            // Apply damage overtime
            if (CharacterEntity.CurrentHp > 0)
            {
                _calculatingTotalDamageOverTime += TotalDamageOverTime * rate;
                if (_calculatingTotalDamageOverTime >= 1)
                {
                    CharacterItem weapon = CharacterItem.Empty;
                    BaseSkill skill = null;
                    int skillLevel = 0;
                    if (!_buff.IsEmpty())
                    {
                        weapon = _buff.BuffApplierWeapon;
                        skill = _buff.GetSkill();
                        skillLevel = _buff.level;
                    }
                    tempAmount = (int)_calculatingTotalDamageOverTime;
                    if (tempAmount < 0)
                        tempAmount = 0;
                    CharacterEntity.CurrentHp -= tempAmount;
                    CharacterEntity.ReceivedDamage(HitBoxPosition.None, CharacterEntity.EntityTransform.position, instigator, DamageOverTimes, CombatAmountType.NormalDamage, tempAmount, weapon, skill, skillLevel, _buff, true);
                    _calculatingTotalDamageOverTime -= tempAmount;
                }
            }
        }
    }
}
