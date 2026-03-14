using System.Collections.Generic;

namespace MultiplayerARPG
{
    public interface ICacheAnimations { }

    public class CacheAnimations<TWeaponAnims, TSkillAnims> : ICacheAnimations
        where TWeaponAnims : IWeaponAnims
        where TSkillAnims : ISkillAnims
    {
        public Dictionary<int, TWeaponAnims> CacheWeaponAnimations { get; protected set; }
        public Dictionary<int, TSkillAnims> CacheSkillAnimations { get; protected set; }

        public CacheAnimations(IEnumerable<TWeaponAnims> weaponAnimations, IEnumerable<TSkillAnims> skillAnimations)
        {
            CacheWeaponAnimations = new Dictionary<int, TWeaponAnims>();
            foreach (TWeaponAnims weaponAnimation in weaponAnimations)
            {
                if (weaponAnimation.Data == null) continue;
                CacheWeaponAnimations[weaponAnimation.Data.DataId] = weaponAnimation;
            }

            CacheSkillAnimations = new Dictionary<int, TSkillAnims>();
            foreach (TSkillAnims skillAnimation in skillAnimations)
            {
                if (skillAnimation.Data == null) continue;
                CacheSkillAnimations[skillAnimation.Data.DataId] = skillAnimation;
            }
        }
    }
}