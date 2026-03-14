using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial interface IAmmoItem : IItem
    {
        /// <summary>
        /// Ammo type data
        /// </summary>
        AmmoType AmmoType { get; }
        /// <summary>
        /// Increasing damages stats while attacking by weapon which put this item
        /// </summary>
        DamageIncremental[] IncreaseDamages { get; }
#if UNITY_EDITOR || !UNITY_SERVER
        /// <summary>
        /// Get attach model (eg. magazine)
        /// </summary>
        /// <returns></returns>
        UniTask<GameObject> GetAmmoAttachModel();
#endif
    }
}
