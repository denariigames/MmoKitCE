using Insthync.UnityEditorUtils;
using UnityEngine;
using Playables = MultiplayerARPG.GameData.Model.Playables;

namespace MultiplayerARPG
{
    public partial class BaseSkill
    {
        [System.Serializable]
        public struct PlayableCharacterModelSettingsData
        {
            [Tooltip("Apply animations to all playable character models or not?, don't have to set `skill` data")]
            public bool applySkillAnimations;
            public Playables.SkillAnimations skillAnimations;
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