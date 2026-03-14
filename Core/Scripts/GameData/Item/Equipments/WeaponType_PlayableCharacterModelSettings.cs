using Insthync.UnityEditorUtils;
using UnityEngine;
using Playables = MultiplayerARPG.GameData.Model.Playables;

namespace MultiplayerARPG
{
    public partial class WeaponType
    {
        [System.Serializable]
        public struct PlayableCharacterModelSettingsData
        {
            [Tooltip("Apply animations to all playable character models or not?, don't have to set `weaponType` data")]
            public bool applyWeaponAnimations;
            public Playables.WeaponAnimations weaponAnimations;
            [Space]
            [Tooltip("Apply animations to all playable character models or not?, don't have to set `weaponType` data")]
            public bool applyLeftHandWeaponAnimations;
            public Playables.WieldWeaponAnimations leftHandWeaponAnimations;
        }

        [Category(1000, "Character Model Settings")]
        [NotPatchable]
        [SerializeField]
        private PlayableCharacterModelSettingsData playableCharacterModelSettings;
        public PlayableCharacterModelSettingsData PlayableCharacterModelSettings
        {
            get { return playableCharacterModelSettings; }
        }
    }
}