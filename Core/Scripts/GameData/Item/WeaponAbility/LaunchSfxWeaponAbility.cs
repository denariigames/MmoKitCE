using UnityEngine;

namespace MultiplayerARPG
{
    [NotPatchable]
    [CreateAssetMenu(fileName = GameDataMenuConsts.LAUNCH_SFX_WEAPON_ABILITY_FILE, menuName = GameDataMenuConsts.LAUNCH_SFX_WEAPON_ABILITY_MENU, order = GameDataMenuConsts.LAUNCH_SFX_WEAPON_ABILITY_ORDER)]
    public class LaunchSfxWeaponAbility : BaseWeaponAbility
    {
        [SerializeField]
        private AudioClipWithVolumeSettings launchClip = new AudioClipWithVolumeSettings();
        public AudioClipWithVolumeSettings LaunchClip => launchClip;

        public const string KEY = "LAUNCH_SFX_WEAPON_ABILITY";
        public override string AbilityKey => KEY;

        public override void OnPreActivate()
        {

        }

        public override void OnPreDeactivate()
        {

        }

        public override WeaponAbilityState UpdateActivation(WeaponAbilityState state, float deltaTime)
        {
            return WeaponAbilityState.Deactivated;
        }
    }
}
