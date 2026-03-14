using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct HarvestEffectiveness
    {
        public WeaponType weaponType;
        [Tooltip("This will multiply with harvest damage amount")]
        [Range(0.01f, 10f)]
        public float damageEffectiveness;
        [ArrayElementTitle("item")]
        public ItemDropForHarvestable[] items;
    }
}