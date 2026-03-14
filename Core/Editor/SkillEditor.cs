using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(Skill))]
    [CanEditMultipleObjects]
    public class SkillEditor : BaseGameDataEditor
    {
        protected override void SetFieldCondition()
        {
            Skill skill = target as Skill;
            // Skill type
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.availableWeapons));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.consumeHp));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.consumeHpRate));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.consumeMp));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.consumeMpRate));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.consumeStamina));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.consumeStaminaRate));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.coolDownDuration));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.skillAttackType));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.skillBuffType));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.moveSpeedRateWhileUsingSkill));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), "skillCastEffects");
#if !DISABLE_ADDRESSABLES
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), "addressableSkillCastEffects");
#endif
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.castDuration));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.canBeInterruptedWhileCasting));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), "skillActivateEffects");
#if !DISABLE_ADDRESSABLES
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), "addressableSkillActivateEffects");
#endif
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.requireShield));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.availableWeapons));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.availableArmors));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.availableVehicles));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.requireItems));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.requireAmmoType));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.requireAmmoAmount));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.requireAmmos));
            // Normal Attack skill
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.damageHitEffects));
#if !DISABLE_ADDRESSABLES
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.addressableDamageHitEffects));
#endif
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.damageInfo));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.damageAmount));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.effectivenessAttributes));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.weaponDamageInflictions));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.additionalDamageAmounts));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.increaseDamageAmountsWithBuffs));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.isDebuff));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.attackStatusEffects));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.Normal), nameof(skill.harvestType));
            // Based On Weapon Attack skill
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.damageHitEffects));
#if !DISABLE_ADDRESSABLES
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.addressableDamageHitEffects));
#endif
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.weaponDamageInflictions));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.additionalDamageAmounts));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.increaseDamageAmountsWithBuffs));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.isDebuff));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.attackStatusEffects));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(Skill.SkillAttackType.BasedOnWeapon), nameof(skill.harvestType));
            // Harvest
            ShowOnEnum(nameof(skill.harvestType), nameof(HarvestType.BasedOnSkill), nameof(skill.harvestDamageAmount));
            // Debuff
            ShowOnBool(nameof(skill.isDebuff), true, nameof(skill.debuff));
            // Buff distance
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToNearbyAllies), nameof(skill.buffDistance));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToNearbyCharacters), nameof(skill.buffDistance));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToTarget), nameof(skill.buffDistance));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToAlly), nameof(skill.buffDistance));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToEnemy), nameof(skill.buffDistance));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToNearbyPartyMembers), nameof(skill.buffDistance));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToPartyMember), nameof(skill.buffDistance));
            // Buff to user if no target
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToTarget), nameof(skill.buffToUserIfNoTarget));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToAlly), nameof(skill.buffToUserIfNoTarget));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToPartyMember), nameof(skill.buffToUserIfNoTarget));
            // Buff
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToUser), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToNearbyAllies), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToNearbyCharacters), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToTarget), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.Toggle), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToAlly), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToEnemy), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToNearbyPartyMembers), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillBuffType), nameof(Skill.SkillBuffType.BuffToPartyMember), nameof(skill.buff));
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Passive), nameof(skill.buff));
            // Summon
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.summon));
            // Mount
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.mount));
            // Craft
            ShowOnEnum(nameof(skill.skillType), nameof(SkillType.Active), nameof(skill.itemCraft));
        }
    }
}
