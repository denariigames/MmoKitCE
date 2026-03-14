using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SIMPLE_WARP_TO_TARGET_SKILL_FILE, menuName = GameDataMenuConsts.SIMPLE_WARP_TO_TARGET_SKILL_MENU, order = GameDataMenuConsts.SIMPLE_WARP_TO_TARGET_SKILL_ORDER)]
    public class SimpleWarpToTargetSkill : BaseAreaSkill
    {
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
            // Teleport to aim position
            skillUser.Teleport(aimPosition.position, skillUser.MovementTransform.rotation, false);
        }
    }
}
