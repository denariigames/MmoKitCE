using Insthync.ManagedUpdating;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public class CharacterSkillAndBuffComponent : BaseGameEntityComponent<BaseCharacterEntity>, IManagedUpdate
    {
        public const float SKILL_BUFF_UPDATE_DURATION = 1f;
        public const string KEY_VEHICLE_BUFF = "<VEHICLE_BUFF>";

        private float _updatingTime;
        private float _deltaTime;
        private Dictionary<string, CharacterRecoveryData> _recoveryBuffs;

        private void Start()
        {
            _recoveryBuffs = new Dictionary<string, CharacterRecoveryData>();
        }

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

            if (_updatingTime >= SKILL_BUFF_UPDATE_DURATION)
            {
                float tempDuration;
                CalculatedBuff tempCalculatedBuff;
                CharacterRecoveryData tempRecoveryData;
                int tempCount;
                // Removing mount if it should
                if (Entity.PassengingVehicleEntity != null)
                {
                    CharacterMount mount = Entity.Mount;
                    tempCalculatedBuff = Entity.PassengingVehicleEntity.GetBuff();
                    if (mount.ShouldRemove(Entity))
                    {
                        _recoveryBuffs.Remove(KEY_VEHICLE_BUFF);
                        Entity.ExitVehicleAndForget();
                    }
                    else
                    {
                        mount.Update(Entity.PassengingVehicleEntity, _updatingTime);
                        Entity.Mount = mount;
                        tempDuration = tempCalculatedBuff.GetDuration();
                        // If duration is 0, damages / recoveries will applied immediately, so don't apply it here
                        if (tempDuration > 0f)
                        {
                            if (!_recoveryBuffs.TryGetValue(KEY_VEHICLE_BUFF, out tempRecoveryData))
                            {
                                tempRecoveryData = new CharacterRecoveryData(Entity);
                                tempRecoveryData.SetupByBuff(CharacterBuff.Empty, tempCalculatedBuff);
                                _recoveryBuffs.Add(KEY_VEHICLE_BUFF, tempRecoveryData);
                            }
                            tempRecoveryData.Apply(1 / tempDuration * _updatingTime);
                        }
                    }
                }
                // Removing summons if it should
                if (!Entity.IsDead())
                {
                    tempCount = Entity.Summons.Count;
                    CharacterSummon summon;
                    for (int i = tempCount - 1; i >= 0; --i)
                    {
                        summon = Entity.Summons[i];
                        tempCalculatedBuff = summon.GetBuff();
                        if (summon.ShouldRemove(Entity))
                        {
                            _recoveryBuffs.Remove(summon.id);
                            Entity.Summons.RemoveAt(i);
                            summon.UnSummon(Entity);
                        }
                        else
                        {
                            summon.Update(Entity, _updatingTime);
                            Entity.Summons[i] = summon;
                            tempDuration = tempCalculatedBuff.GetDuration();
                            // If duration is 0, damages / recoveries will applied immediately, so don't apply it here
                            if (tempDuration > 0f)
                            {
                                if (!_recoveryBuffs.TryGetValue(summon.id, out tempRecoveryData))
                                {
                                    tempRecoveryData = new CharacterRecoveryData(Entity);
                                    tempRecoveryData.SetupByBuff(CharacterBuff.Empty, tempCalculatedBuff);
                                    _recoveryBuffs.Add(summon.id, tempRecoveryData);
                                }
                                tempRecoveryData.Apply(1 / tempDuration * _updatingTime);
                            }
                        }
                        // Don't update next buffs if character dead
                        if (Entity.IsDead())
                            break;
                    }
                }
                // Can mount by buffs, so prepare data here
                bool foundBuffMount = false;
                // Removing buffs if it should
                if (!Entity.IsDead())
                {
                    tempCount = Entity.Buffs.Count;
                    CharacterBuff buff;
                    for (int i = tempCount - 1; i >= 0; --i)
                    {
                        buff = Entity.Buffs[i];
                        tempCalculatedBuff = buff.GetBuff();
                        if (buff.ShouldRemove())
                        {
                            _recoveryBuffs.Remove(buff.id);
                            Entity.OnRemoveBuff(buff, BuffRemoveReasons.Timeout);
                            Entity.Buffs.RemoveAt(i);
                        }
                        else
                        {
                            buff.Update(_updatingTime);
                            Entity.Buffs[i] = buff;
                            tempDuration = tempCalculatedBuff.GetDuration();
                            // If duration is 0, damages / recoveries will applied immediately, so don't apply it here
                            if (tempDuration > 0f)
                            {
                                if (!_recoveryBuffs.TryGetValue(buff.id, out tempRecoveryData))
                                {
                                    tempRecoveryData = new CharacterRecoveryData(Entity);
                                    tempRecoveryData.SetupByBuff(buff, tempCalculatedBuff);
                                    _recoveryBuffs.Add(buff.id, tempRecoveryData);
                                }
                                tempRecoveryData.Apply(1 / tempDuration * _updatingTime);
                            }
                            // Mount
                            if (!foundBuffMount && tempCalculatedBuff.TryGetMount(out BuffMount tempBuffMount))
                            {
                                foundBuffMount = true;
                                int tempMountLevel = tempCalculatedBuff.GetMountLevel();
                                if (Entity.IsDifferMount(MountType.Buff, buff.id, tempMountLevel))
                                    Entity.SpawnMount(MountType.Buff, buff.id, 0f, tempMountLevel, 0);
                            }
                        }
                        // Don't update next buffs if character dead
                        if (Entity.IsDead())
                            break;
                    }
                }
                // Removing skill usages if it should
                if (!Entity.IsDead())
                {
                    tempCount = Entity.SkillUsages.Count;
                    CharacterSkillUsage skillUsage;
                    for (int i = tempCount - 1; i >= 0; --i)
                    {
                        skillUsage = Entity.SkillUsages[i];
                        if (skillUsage.ShouldRemove())
                        {
                            Entity.SkillUsages.RemoveAt(i);
                        }
                        else
                        {
                            skillUsage.Update(_updatingTime);
                            Entity.SkillUsages[i] = skillUsage;
                        }
                    }
                }
                _updatingTime = 0;
            }
        }

        public void OnAttack()
        {
            CharacterDataCache cache = Entity.CachedData;
            if (!cache.HavingChanceToRemoveBuffWhenAttack)
                return;
            bool stillHavingChance = false;
            CharacterBuff buff;
            float chance;
            for (int i = Entity.Buffs.Count - 1; i >= 0; --i)
            {
                buff = Entity.Buffs[i];
                chance = buff.GetBuff().GetRemoveBuffWhenAttackChance();
                if (chance > 0)
                {
                    if (Random.value > chance)
                    {
                        stillHavingChance = true;
                        continue;
                    }
                    Entity.OnRemoveBuff(buff, BuffRemoveReasons.RemoveByAttackRemoveChance);
                    Entity.Buffs.RemoveAt(i);
                }
            }
            if (!stillHavingChance)
                cache.ClearChanceToRemoveBuffWhenAttack();
        }

        public void OnAttacked()
        {
            CharacterDataCache cache = Entity.CachedData;
            if (!cache.HavingChanceToRemoveBuffWhenAttacked)
                return;
            bool stillHavingChance = false;
            CharacterBuff buff;
            float chance;
            for (int i = Entity.Buffs.Count - 1; i >= 0; --i)
            {
                buff = Entity.Buffs[i];
                chance = buff.GetBuff().GetRemoveBuffWhenAttackedChance();
                if (chance > 0)
                {
                    if (Random.value > chance)
                    {
                        stillHavingChance = true;
                        continue;
                    }
                    Entity.OnRemoveBuff(buff, BuffRemoveReasons.RemoveByAttackedRemoveChance);
                    Entity.Buffs.RemoveAt(i);
                }
            }
            if (!stillHavingChance)
                cache.ClearChanceToRemoveBuffWhenAttacked();
        }

        public void OnUseSkill()
        {
            CharacterDataCache cache = Entity.CachedData;
            if (!cache.HavingChanceToRemoveBuffWhenUseSkill)
                return;
            bool stillHavingChance = false;
            CharacterBuff buff;
            float chance;
            for (int i = Entity.Buffs.Count - 1; i >= 0; --i)
            {
                buff = Entity.Buffs[i];
                chance = buff.GetBuff().GetRemoveBuffWhenUseSkillChance();
                if (chance > 0)
                {
                    if (Random.value > chance)
                    {
                        stillHavingChance = true;
                        continue;
                    }
                    Entity.OnRemoveBuff(buff, BuffRemoveReasons.RemoveByUseSkillRemoveChance);
                    Entity.Buffs.RemoveAt(i);
                }
            }
            if (!stillHavingChance)
                cache.ClearChanceToRemoveBuffWhenUseSkill();
        }

        public void OnUseItem()
        {
            CharacterDataCache cache = Entity.CachedData;
            if (!cache.HavingChanceToRemoveBuffWhenUseItem)
                return;
            bool stillHavingChance = false;
            CharacterBuff buff;
            float chance;
            for (int i = Entity.Buffs.Count - 1; i >= 0; --i)
            {
                buff = Entity.Buffs[i];
                chance = buff.GetBuff().GetRemoveBuffWhenUseItemChance();
                if (chance > 0)
                {
                    if (Random.value > chance)
                    {
                        stillHavingChance = true;
                        continue;
                    }
                    Entity.OnRemoveBuff(buff, BuffRemoveReasons.RemoveByUseItemRemoveChance);
                    Entity.Buffs.RemoveAt(i);
                }
            }
            if (!stillHavingChance)
                cache.ClearChanceToRemoveBuffWhenUseItem();
        }

        public void OnPickupItem()
        {
            CharacterDataCache cache = Entity.CachedData;
            if (!cache.HavingChanceToRemoveBuffWhenPickupItem)
                return;
            bool stillHavingChance = false;
            CharacterBuff buff;
            float chance;
            for (int i = Entity.Buffs.Count - 1; i >= 0; --i)
            {
                buff = Entity.Buffs[i];
                chance = buff.GetBuff().GetRemoveBuffWhenPickupItemChance();
                if (chance > 0)
                {
                    if (Random.value > chance)
                    {
                        stillHavingChance = true;
                        continue;
                    }
                    Entity.OnRemoveBuff(buff, BuffRemoveReasons.RemoveByPickupChance);
                    Entity.Buffs.RemoveAt(i);
                }
            }
            if (!stillHavingChance)
                cache.ClearChanceToRemoveBuffWhenPickupItem();
        }
    }
}
