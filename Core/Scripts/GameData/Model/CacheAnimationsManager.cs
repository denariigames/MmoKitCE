using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class CacheAnimationsManager
    {
        private static readonly Dictionary<int, ICacheAnimations> CacheAnims = new Dictionary<int, ICacheAnimations>();

        /// <summary>
        /// Create and set new `CacheAnimations` which created by `weaponAnimations` and `skillAnimations` data
        /// </summary>
        /// <typeparam name="TWeaponAnims"></typeparam>
        /// <typeparam name="TSkillAnims"></typeparam>
        /// <param name="id"></param>
        /// <param name="weaponAnimations"></param>
        /// <param name="skillAnimations"></param>
        public static void SetCacheAnimations<TWeaponAnims, TSkillAnims>(int id, IEnumerable<TWeaponAnims> weaponAnimations, IEnumerable<TSkillAnims> skillAnimations)
            where TWeaponAnims : IWeaponAnims
            where TSkillAnims : ISkillAnims
        {
            CacheAnims[id] = new CacheAnimations<TWeaponAnims, TSkillAnims>(weaponAnimations, skillAnimations);
        }

        /// <summary>
        /// Get `CacheAnimations` by specific ID
        /// </summary>
        /// <typeparam name="TWeaponAnims"></typeparam>
        /// <typeparam name="TSkillAnims"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CacheAnimations<TWeaponAnims, TSkillAnims> GetCacheAnimations<TWeaponAnims, TSkillAnims>(int id)
            where TWeaponAnims : IWeaponAnims
            where TSkillAnims : ISkillAnims
        {
            return CacheAnims[id] as CacheAnimations<TWeaponAnims, TSkillAnims>;
        }

        /// <summary>
        /// Create and set new `CacheAnimations` (If not exists), `CacheAnimations` will be created by `weaponAnimations` and `skillAnimations` data
        /// Then return `CacheAnimations`
        /// </summary>
        /// <typeparam name="TWeaponAnims"></typeparam>
        /// <typeparam name="TSkillAnims"></typeparam>
        /// <param name="id"></param>
        /// <param name="weaponAnimations"></param>
        /// <param name="skillAnimations"></param>
        /// <returns></returns>
        public static CacheAnimations<TWeaponAnims, TSkillAnims> SetAndGetCacheAnimations<TWeaponAnims, TSkillAnims>(int id, IEnumerable<TWeaponAnims> weaponAnimations, IEnumerable<TSkillAnims> skillAnimations)
            where TWeaponAnims : IWeaponAnims
            where TSkillAnims : ISkillAnims
        {
            if (!CacheAnims.ContainsKey(id))
                SetCacheAnimations(id, weaponAnimations, skillAnimations);
            return GetCacheAnimations<TWeaponAnims, TSkillAnims>(id);
        }

        /// <summary>
        /// Create and set new `CacheAnimations` (If not exists), `CacheAnimations` will be created by `weaponAnimations` and `skillAnimations` data
        /// Then return `WeaponAnimations`
        /// </summary>
        /// <typeparam name="TWeaponAnims"></typeparam>
        /// <typeparam name="TSkillAnims"></typeparam>
        /// <param name="id"></param>
        /// <param name="weaponAnimations"></param>
        /// <param name="skillAnimations"></param>
        /// <param name="dataId"></param>
        /// <param name="anims"></param>
        /// <returns></returns>
        public static bool SetAndTryGetCacheWeaponAnimations<TWeaponAnims, TSkillAnims>(int id, IEnumerable<TWeaponAnims> weaponAnimations, IEnumerable<TSkillAnims> skillAnimations, int dataId, out TWeaponAnims anims)
            where TWeaponAnims : IWeaponAnims
            where TSkillAnims : ISkillAnims
        {
            return SetAndGetCacheAnimations(id, weaponAnimations, skillAnimations).CacheWeaponAnimations.TryGetValue(dataId, out anims);
        }

        /// <summary>
        /// Create and set new `CacheAnimations` (If not exists), `CacheAnimations` will be created by `weaponAnimations` and `skillAnimations` data
        /// Then return `SkillAnimations`
        /// </summary>
        /// <typeparam name="TWeaponAnims"></typeparam>
        /// <typeparam name="TSkillAnims"></typeparam>
        /// <param name="id"></param>
        /// <param name="weaponAnimations"></param>
        /// <param name="skillAnimations"></param>
        /// <param name="dataId"></param>
        /// <param name="anims"></param>
        /// <returns></returns>
        public static bool SetAndTryGetCacheSkillAnimations<TWeaponAnims, TSkillAnims>(int id, IEnumerable<TWeaponAnims> weaponAnimations, IEnumerable<TSkillAnims> skillAnimations, int dataId, out TSkillAnims anims)
            where TWeaponAnims : IWeaponAnims
            where TSkillAnims : ISkillAnims
        {
            return SetAndGetCacheAnimations(id, weaponAnimations, skillAnimations).CacheSkillAnimations.TryGetValue(dataId, out anims);
        }
    }
}
