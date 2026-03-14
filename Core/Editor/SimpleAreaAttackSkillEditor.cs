using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(SimpleAreaAttackSkill))]
    [CanEditMultipleObjects]
    public class SimpleAreaAttackSkillEditor : BaseGameDataEditor
    {
        protected override void SetFieldCondition()
        {
            SimpleAreaAttackSkill skill = CreateInstance<SimpleAreaAttackSkill>();
            // Normal Attack skill
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.damageHitEffects));
#if !DISABLE_ADDRESSABLES
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.addressableDamageHitEffects));
#endif
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.damageAmount));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.effectivenessAttributes));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.weaponDamageInflictions));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.additionalDamageAmounts));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.increaseDamageAmountsWithBuffs));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.Normal), nameof(skill.isDebuff));
            // Based On Weapon Attack skill
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.BasedOnWeapon), nameof(skill.damageHitEffects));
#if !DISABLE_ADDRESSABLES
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.BasedOnWeapon), nameof(skill.addressableDamageHitEffects));
#endif
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.BasedOnWeapon), nameof(skill.weaponDamageInflictions));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.BasedOnWeapon), nameof(skill.additionalDamageAmounts));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.BasedOnWeapon), nameof(skill.increaseDamageAmountsWithBuffs));
            ShowOnEnum(nameof(skill.skillAttackType), nameof(SimpleAreaAttackSkill.SkillAttackType.BasedOnWeapon), nameof(skill.isDebuff));
            // Harvest
            ShowOnEnum(nameof(skill.harvestType), nameof(HarvestType.BasedOnSkill), nameof(skill.harvestDamageAmount));
            // Debuff
            ShowOnBool(nameof(skill.isDebuff), true, nameof(skill.debuff));
        }
    }
}
