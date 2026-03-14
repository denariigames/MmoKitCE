using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SIMPLE_RESURRECTION_SKILL_FILE, menuName = GameDataMenuConsts.SIMPLE_RESURRECTION_SKILL_MENU, order = GameDataMenuConsts.SIMPLE_RESURRECTION_SKILL_ORDER)]
    public class SimpleResurrectionSkill : BaseSkill
    {
        [Category("Buff")]
        public IncrementalFloat buffDistance;
        public Buff buff;
        [Range(0.01f, 1f)]
        public float resurrectHpRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectMpRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectStaminaRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectFoodRate = 0.1f;
        [Range(0.01f, 1f)]
        public float resurrectWaterRate = 0.1f;

        protected override void ApplySkillImplement(
            BaseCharacterEntity skillUser,
            int skillLevel,
            WeaponHandlingState weaponHandlingState,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            uint targetObjectId,
            AimPosition aimPosition)
        {
            // Resurrect target
            BasePlayerCharacterEntity targetEntity;
            if (!skillUser.CurrentGameManager.TryGetEntityByObjectId(targetObjectId, out targetEntity) || !targetEntity.IsDead())
                return;

            CharacterDataCache cachedData = targetEntity.CachedData;
            targetEntity.CurrentHp = Mathf.CeilToInt(cachedData.MaxHp * resurrectHpRate);
            targetEntity.CurrentMp = Mathf.CeilToInt(cachedData.MaxMp * resurrectMpRate);
            targetEntity.CurrentStamina = Mathf.CeilToInt(cachedData.MaxStamina * resurrectStaminaRate);
            targetEntity.CurrentFood = Mathf.CeilToInt(cachedData.MaxFood * resurrectFoodRate);
            targetEntity.CurrentWater = Mathf.CeilToInt(cachedData.MaxWater * resurrectWaterRate);
            targetEntity.StopMove();
            targetEntity.CallRpcOnRespawn();
            targetEntity.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel, skillUser.GetInfo(), weapon);
        }

        public override float GetCastDistance(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand)
        {
            return buffDistance.GetAmount(skillLevel);
        }

        public override SkillType SkillType
        {
            get { return SkillType.Active; }
        }

        public override bool RequiredTarget
        {
            get { return true; }
        }

        public override bool CanUse(BaseCharacterEntity character, int level, bool isLeftHand, uint targetObjectId, out UITextKeys gameMessage, bool isItem = false)
        {
            if (!base.CanUse(character, level, isLeftHand, targetObjectId, out gameMessage, isItem))
                return false;

            BasePlayerCharacterEntity targetEntity;
            if (!character.CurrentGameManager.TryGetEntityByObjectId(targetObjectId, out targetEntity) || !targetEntity.IsDead())
                return false;

            return true;
        }

        public override bool TryGetBuff(out Buff buff)
        {
            buff = this.buff;
            return true;
        }
    }
}
