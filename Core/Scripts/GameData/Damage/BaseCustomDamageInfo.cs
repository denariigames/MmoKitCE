using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseCustomDamageInfo : ScriptableObject, IDamageInfo
    {
        public abstract UniTask LaunchDamageEntity(
            BaseCharacterEntity attacker,
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            Vector3 fireSpreadRange,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            BaseSkill skill,
            int skillLevel,
            AimPosition aimPosition);
        public abstract Transform GetDamageTransform(BaseCharacterEntity attacker, bool isLeftHand);
        public abstract float GetDistance();
        public abstract float GetFov();
        public abstract bool IsHitValid(HitValidateData hitValidateData, HitRegisterData hitData, DamageableHitBox hitBox);
        public virtual bool IsHeadshotInstantDeath()
        {
            return false;
        }

        public virtual void PrepareRelatesData()
        {

        }
    }
}
